// Copyright (c) Atlas Lift Tech Inc. All rights reserved.

using MQTTnet.Server;

namespace MQTTnet.AspNetCore.AttributeRouting.Routing
{
    public interface IMqttControllerContext
    {
        InterceptingPublishEventArgs MqttContext { get; set; }
        MqttServer MqttServer { get; set; }
    }
}