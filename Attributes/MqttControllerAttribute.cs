// Copyright (c) .NET Foundation. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt
// in the project root for license information.

// Modifications Copyright (c) Atlas Lift Tech Inc. All rights reserved.

using System;

namespace MQTTnet.AspNetCore.AttributeRouting
{
    /// <summary>
    /// Indicates that a type and all derived types are used to serve MQTT responses.
    /// <para>
    /// Controllers decorated with this attribute are configured with features and behavior targeted at improving the
    /// developer experience for building APIs.
    /// </para>
    /// <para>
    /// When decorated on an assembly, all controllers in the assembly will be treated as controllers with API behavior.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MqttControllerAttribute : Attribute
    {
    }
}