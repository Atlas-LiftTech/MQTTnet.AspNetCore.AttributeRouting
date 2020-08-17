// Copyright (c) Atlas Lift Tech Inc. All rights reserved.

using MQTTnet.Server;

namespace MQTTnet.AspNetCore.AttributeRouting.Routing
{
    public interface IMqttControllerContext
    {
        MqttApplicationMessageInterceptorContext MqttContext { get; set; }
        IMqttServer MqttServer { get; set; }
    }
}