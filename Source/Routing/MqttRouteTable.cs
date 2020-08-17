// Copyright (c) .NET Foundation. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt
// in the project root for license information.

// Modifications Copyright (c) Atlas Lift Tech Inc. All rights reserved.

namespace MQTTnet.AspNetCore.AttributeRouting
{
    internal class MqttRouteTable
    {
        public MqttRouteTable(MqttRoute[] routes)
        {
            Routes = routes;
        }

        public MqttRoute[] Routes { get; }

        internal void Route(MqttRouteContext routeContext)
        {
            for (var i = 0; i < Routes.Length; i++)
            {
                Routes[i].Match(routeContext);

                if (routeContext.Handler != null)
                {
                    return;
                }
            }
        }
    }
}