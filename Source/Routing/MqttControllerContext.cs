// Copyright (c) Atlas Lift Tech Inc. All rights reserved.

using MQTTnet.Server;

namespace MQTTnet.AspNetCore.AttributeRouting.Routing
{
    public class MqttControllerContext : IMqttControllerContext
    {
        public MqttApplicationMessageInterceptorContext MqttContext { get; set; }
        public IMqttServer MqttServer { get; set; }
    }
}