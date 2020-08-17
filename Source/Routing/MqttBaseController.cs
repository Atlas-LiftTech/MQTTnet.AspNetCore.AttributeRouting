// Copyright (c) Atlas Lift Tech Inc. All rights reserved.

using Microsoft.AspNetCore.Mvc;
using MQTTnet.AspNetCore.AttributeRouting.Attributes;
using MQTTnet.AspNetCore.AttributeRouting.Routing;
using MQTTnet.Server;
using System.Threading.Tasks;

namespace MQTTnet.AspNetCore.AttributeRouting
{
    [MqttController]
    public abstract class MqttBaseController
    {
        /// <summary>
        /// Connection context is set by controller activator. If this class is instantiated directly, it will be null.
        /// </summary>
        public MqttApplicationMessageInterceptorContext MqttContext => ControllerContext.MqttContext;

        /// <summary>
        /// Gets the <see cref="MqttApplicationMessage"/> for the executing action.
        /// </summary>
        public MqttApplicationMessage Message => MqttContext.ApplicationMessage;

        /// <summary>
        /// Gets the <see cref="MqttServer"/> for the executing action.
        /// </summary>
        public IMqttServer Server => ControllerContext.MqttServer;

        /// <summary>
        /// ControllerContext is set by controller activator. If this class is instantiated directly, it will be null.
        /// </summary>
        [MqttControllerContext]
        public MqttControllerContext ControllerContext { get; set; }

        /// <summary>
        /// Create a result that accepts the given message and publishes it to all subscribers on the topic.
        /// </summary>
        /// <returns>The created <see cref="Task"/> for the reponse.</returns>
        [NonAction]
        public virtual Task Ok()
        {
            MqttContext.AcceptPublish = true;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Create a result that accepts the given message and publishes it to all subscribers on the topic. This is an
        /// alias for the <see cref="Ok"/> result.
        /// </summary>
        /// <returns>The created <see cref="Task"/> for the reponse.</returns>
        [NonAction]
        public virtual Task Accepted() => Ok();

        /// <summary>
        /// Create a result that rejects the given message and prevents publishing it to any subscribers.
        /// </summary>
        /// <returns>The created <see cref="Task"/> for the reponse.</returns>
        [NonAction]
        public virtual Task BadMessage()
        {
            MqttContext.AcceptPublish = false;
            return Task.CompletedTask;
        }
    }
}