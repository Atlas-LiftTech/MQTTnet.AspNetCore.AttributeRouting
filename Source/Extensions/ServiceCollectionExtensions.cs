// Copyright (c) Atlas Lift Tech Inc. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using MQTTnet.AspNetCore.AttributeRouting.Routing;
using MQTTnet.Server;
using System.Reflection;
using System.Runtime.CompilerServices;

// This is needed to make internal classes visible to UnitTesting projects
[assembly: InternalsVisibleTo("MQTTnet.AspNetCore.AttributeRouting.Tests, PublicKey=0024000004800000" +
    "94000000060200000024000052534131000400000100010091a5662c5f42234a9cd1cf5d80d8bfed8652694da25bc9c" +
    "3cbc0c160b41cb124fc6ad7896b40b82964d86ef0c1d2a21bf478988141e420a62ee172146a2e4396fa2638154e2cd4" +
    "a926ec3f6cef2ca1fbf52775aa63156a1c21efc904b07b5699088e8e1b82d8186911c34a580b3f6fe4b77506e297875" +
    "1110985c444a6968fcf")]

namespace MQTTnet.AspNetCore.AttributeRouting
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMqttControllers(this IServiceCollection services)
        {
            services.AddSingleton(c =>
            {
                // future enhancement: scan for other AppParts, if needed

                var assemblies = new Assembly[] { Assembly.GetEntryAssembly() };

                return MqttRouteTableFactory.Create(assemblies);
            });

            services.AddSingleton<ITypeActivatorCache>(new TypeActivatorCache());
            services.AddSingleton<MqttRouter>();

            return services;
        }

        public static AspNetMqttServerOptionsBuilder WithAttributeRouting(this AspNetMqttServerOptionsBuilder options, bool allowUnmatchedRoutes = false)
        {
            var router = options.ServiceProvider.GetRequiredService<MqttRouter>();
            var interceptor = new MqttServerApplicationMessageInterceptorDelegate(context => router.OnIncomingApplicationMessage(options, context, allowUnmatchedRoutes));

            options.WithApplicationMessageInterceptor(interceptor);

            return options;
        }
    }
}