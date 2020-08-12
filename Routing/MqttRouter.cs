// Copyright (c) Atlas Lift Tech Inc. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet.AspNetCore.AttributeRouting.Attributes;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MQTTnet.AspNetCore.AttributeRouting.Routing
{
    internal class MqttRouter
    {
        private readonly ILogger<MqttRouter> logger;

        public MqttRouter(ILogger<MqttRouter> logger)
        {
            this.logger = logger;
        }

        internal void OnIncomingApplicationMessage(AspNetMqttServerOptionsBuilder options, MqttApplicationMessageInterceptorContext context)
        {
            var routeTable = options.ServiceProvider.GetRequiredService<MqttRouteTable>();
            var typeActivator = options.ServiceProvider.GetRequiredService<ITypeActivatorCache>();
            var routeContext = new MqttRouteContext(context.ApplicationMessage.Topic);

            routeTable.Route(routeContext);

            if (routeContext.Handler == null)
            {
                // Route not found
                logger.LogDebug($"Rejecting message publish because '{context.ApplicationMessage.Topic}' did not match any known routes.");

                context.AcceptPublish = false;
            }
            else
            {
                object result = null;

                using (var scope = options.ServiceProvider.CreateScope())
                {
                    var classInstance = typeActivator.CreateInstance<object>(scope.ServiceProvider, routeContext.Handler.DeclaringType);

                    // Potential perf improvement is to cache this reflection work in the future.
                    var activateProperties = routeContext.Handler.DeclaringType.GetRuntimeProperties()
                        .Where((property) =>
                        {
                            return
                                property.IsDefined(typeof(MqttControllerContextAttribute)) &&
                                property.GetIndexParameters().Length == 0 &&
                                property.SetMethod != null &&
                                !property.SetMethod.IsStatic;
                        })
                        .ToArray();

                    if (activateProperties.Length == 0)
                    {
                        logger.LogDebug($"MqttController '{routeContext.Handler.DeclaringType.FullName}' does not have a property that can accept a controller context.  You may want to add a [{nameof(MqttControllerContextAttribute)}] to a pubilc property.");
                    }

                    foreach (var property in activateProperties)
                    {
                        property.SetValue(classInstance, context);
                    }

                    ParameterInfo[] parameters = routeContext.Handler.GetParameters();

                    if (parameters.Length == 0)
                    {
                        result = routeContext.Handler.Invoke(classInstance, null);
                        context.AcceptPublish = true;
                    }
                    else
                    {
                        object[] paramArray;

                        try
                        {
                            paramArray = parameters.Select(p => MatchParameterOrThrow(p, routeContext.Parameters)).ToArray();

                            result = routeContext.Handler.Invoke(classInstance, paramArray);
                            context.AcceptPublish = true;
                        }
                        catch (ArgumentException ex)
                        {
                            logger.LogError(ex, $"Unable to match route parameters to all arguments. See inner exception for details.");

                            context.AcceptPublish = false;
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, $"Unhandled MQTT action exception. See inner exception for details.");

                            // This is an unandled exception from the invoked action
                            context.AcceptPublish = false;
                        }
                    }
                }
            }
        }

        private static object MatchParameterOrThrow(ParameterInfo param, IReadOnlyDictionary<string, object> availableParmeters)
        {
            if (!availableParmeters.TryGetValue(param.Name, out object value))
            {
                if (param.IsOptional)
                {
                    return null;
                }
                else
                {
                    throw new ArgumentException($"No matching route parameter for \"{param.ParameterType.Name} {param.Name}\"", param.Name);
                }
            }

            if (!param.ParameterType.IsAssignableFrom(value.GetType()))
            {
                throw new ArgumentException($"Cannot assign type \"{value.GetType()}\" to parameter \"{param.ParameterType.Name} {param.Name}\"", param.Name);
            }

            return value;
        }
    }
}