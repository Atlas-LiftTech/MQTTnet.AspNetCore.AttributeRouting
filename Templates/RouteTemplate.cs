// Copyright (c) .NET Foundation. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt
// in the project root for license information. // Modifications Copyright (c) Atlas Lift Tech Inc.

using System.Diagnostics;
using System.Linq;

namespace MQTTnet.AspNetCore.AttributeRouting
{
    [DebuggerDisplay("{TemplateText}")]
    internal class RouteTemplate
    {
        public RouteTemplate(string templateText, TemplateSegment[] segments)
        {
            TemplateText = templateText;
            Segments = segments;
            OptionalSegmentsCount = segments.Count(template => template.IsOptional);
            ContainsCatchAllSegment = segments.Any(template => template.IsCatchAll);
        }

        public string TemplateText { get; }

        public TemplateSegment[] Segments { get; }

        public int OptionalSegmentsCount { get; }

        public bool ContainsCatchAllSegment { get; }
    }
}