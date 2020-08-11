// Copyright (c) Atlas Lift Tech Inc. All rights reserved.

using MQTTnet.Server;

namespace MQTTnet.AspNetCore.AttributeRouting
{
    [MqttController]
    public abstract class MqttBaseController
    {
        public MqttApplicationMessageInterceptorContext MqttContext { get; set; }

        public MqttApplicationMessage Request => MqttContext.ApplicationMessage;
    }
}