// Copyright (c) Atlas Lift Tech Inc. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using MQTTnet.AspNetCore.AttributeRouting.Routing;
using MQTTnet.Server;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

// This is needed to make internal classes visible to UnitTesting projects
[assembly: InternalsVisibleTo("MQTTnet.AspNetCore.AttributeRouting.Tests, PublicKey=00240000048000009" +
    "4000000060200000024000052534131000400000100010089369e254b2bf47119265eb7514c522350b2e61beda20ccc9" +
    "a9ddc3f8dab153d59d23011476cc939860d9ae7d09d1bade2915961d01f9ec1f1852265e4d54b090f4c427756f7044e8" +
    "65ffcd47bf99f18af6361de42003808f7323d20d5d2c66fe494852b5e2438db793ec9fd845b80e1ce5c9b17ff053f386" +
    "bc0f06080e9d0ba")]

namespace MQTTnet.AspNetCore.AttributeRouting
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMqttControllers(this IServiceCollection services, Assembly[] fromAssemblies = null)
        {
            services.AddSingleton(_ =>
            {
                if (fromAssemblies != null && fromAssemblies.Length == 0)
                {
                    throw new ArgumentException("'fromAssemblies' cannot be an empty array. Pass null or a collection of 1 or more assemblies.", nameof(fromAssemblies));
                }

                var assemblies = fromAssemblies ?? new Assembly[] { Assembly.GetEntryAssembly() };

                return MqttRouteTableFactory.Create(assemblies);
            });

            services.AddSingleton<ITypeActivatorCache>(new TypeActivatorCache());
            services.AddSingleton<MqttRouter>();

            return services;
        }

        public static void WithAttributeRouting(this MqttServer server, IServiceProvider svcProvider, bool allowUnmatchedRoutes = false)
        {
            var router = svcProvider.GetRequiredService<MqttRouter>();
            server.InterceptingPublishAsync += async (args) =>
            {
                await router.OnIncomingApplicationMessage(svcProvider, args, allowUnmatchedRoutes);
            };
        }
    }
}