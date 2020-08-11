using Microsoft.Extensions.DependencyInjection;
using MQTTnet.AspNetCore.Extensions;
using MQTTnet.Server;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MQTTnet.AspNetCore.AttributeRouting
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHostedMqttServerWithAttributeRouting(this IServiceCollection services, Action<AspNetMqttServerOptionsBuilder> configure)
        {
            var assemblies = new Assembly[] { Assembly.GetEntryAssembly() };
            var routeTable = MqttRouteTableFactory.Create(assemblies);

            services.AddSingleton(routeTable);

            services.AddHostedMqttServerWithServices(options =>
            {
                configure(options);

                var rt = options.ServiceProvider.GetRequiredService<MqttRouteTable>();

                options.WithApplicationMessageInterceptor(new MqttServerApplicationMessageInterceptorDelegate(context =>
                        {
                            var ctx = new MqttRouteContext(context.ApplicationMessage.Topic);

                            rt.Route(ctx);

                            if (ctx.Handler == null)
                            {
                                context.AcceptPublish = false;
                            }
                            else
                            {
                                object result = null;
                                ParameterInfo[] parameters = ctx.Handler.GetParameters();

                                using (var scope = options.ServiceProvider.CreateScope())
                                {
                                    object classInstance = ActivatorUtilities.GetServiceOrCreateInstance(scope.ServiceProvider, ctx.Handler.DeclaringType);

                                    // TODO: Handle instance where we get a non MqttBaseController for some reason
                                    ((MqttBaseController)classInstance).MqttContext = context;

                                    try
                                    {
                                        if (parameters.Length == 0)
                                        {
                                            result = ctx.Handler.Invoke(classInstance, null);
                                        }
                                        else
                                        {
                                            // TODO: Better error messages if parameters don't align properly
                                            object[] paramArray = parameters.Select(p => ctx.Parameters[p.Name]).ToArray();

                                            result = ctx.Handler.Invoke(classInstance, paramArray);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine(ex);
                                    }
                                }

                                context.AcceptPublish = true;

                                Debug.WriteLine($"Matched route ${context.ApplicationMessage.Topic} to handler ${ctx.Handler.DeclaringType.FullName}.{ctx.Handler.Name}");
                            }
                        }));
            });

            return services;
        }
    }
}