// Copyright (c) Atlas Lift Tech Inc. All rights reserved.

using MQTTnet.AspNetCore.AttributeRouting.Attributes;
using MQTTnet.Server;

namespace MQTTnet.AspNetCore.AttributeRouting
{
    [MqttController]
    public abstract class MqttBaseController
    {
        /// <summary>
        /// Connection context is set by controller activator. If this class is instantiated directly, it will be null.
        /// </summary>
        [MqttControllerContext]
        public MqttApplicationMessageInterceptorContext MqttContext { get; set; }

        /// <summary>
        /// Gets the <see cref="MqttApplicationMessage"/> for the executing action.
        /// </summary>
        public MqttApplicationMessage Message => MqttContext.ApplicationMessage;
    }
}