// Copyright (c) Atlas Lift Tech Inc. All rights reserved.

using MQTTnet.Server;

namespace MQTTnet.AspNetCore.AttributeRouting.Routing
{
    public class MqttControllerContext : IMqttControllerContext
    {
        public InterceptingPublishEventArgs MqttContext { get; set; }
        public MqttServer MqttServer { get; set; }
    }
}