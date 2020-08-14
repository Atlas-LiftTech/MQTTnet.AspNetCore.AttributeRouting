using Microsoft.VisualStudio.TestTools.UnitTesting;
using MQTTnet.AspNetCore.AttributeRouting.Routing;
using System;
using System.Collections.Generic;

namespace MQTTnet.AspNetCore.AttributeRouting.Tests
{
    [TestClass]
    public class RouteTemplateParserTests
    {
        [TestMethod]
        public void Parse_SingleLiteral()
        {
            // Arrange
            var template = "cool";

            // Act
            var expected = new RouteTemplate(template, new List<TemplateSegment>());
            expected.Segments.Add(new TemplateSegment(template, "cool", false));

            var actual = TemplateParser.ParseTemplate(template);

            // Assert
            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void Parse_SingleParameter()
        {
            // Arrange
            var template = "{p}";

            // Act
            var expected = new RouteTemplate(template, new List<TemplateSegment>());
            expected.Segments.Add(new TemplateSegment(template, "p", true));

            var actual = TemplateParser.ParseTemplate(template);

            // Assert
            Assert.AreEqual(actual.TemplateText, template);
            Assert.IsTrue(actual.Segments.Count == 1);
        }

        [TestMethod]
        public void Parse_OptionalParameter()
        {
            // Arrange
            var template = "{p?}";

            // Act
            var expected = new RouteTemplate(template, new List<TemplateSegment>());
            expected.Segments.Add(new TemplateSegment(template, "p?", true));

            var actual = TemplateParser.ParseTemplate(template);

            // Assert
            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void Parse_CatchAllParameter()
        {
            // Arrange
            var template = "{*p}";

            // Act
            var expected = new RouteTemplate(template, new List<TemplateSegment>());
            expected.Segments.Add(new TemplateSegment(template, "*p", true));

            var actual = TemplateParser.ParseTemplate(template);

            // Assert
            Assert.AreEqual(actual, expected);
            Assert.IsTrue(actual.Segments[0].IsCatchAll);
        }

        [TestMethod]
        public void Parse_MultipleLiterals()
        {
            // Arrange
            var template = "cool/awesome/super";

            // Act
            var actual = TemplateParser.ParseTemplate(template);
            var expected = new RouteTemplate(template, new List<TemplateSegment>()
            {
                new TemplateSegment(template, "cool", false),
                new TemplateSegment(template, "awesome", false),
                new TemplateSegment(template, "super", false),
            });

            // Assert
            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void Parse_MultipleParamters()
        {
            // Arrange
            var template = "{cool}/{awesome}/{super}";

            // Act
            var actual = TemplateParser.ParseTemplate(template);
            var expected = new RouteTemplate(template, new List<TemplateSegment>()
            {
                new TemplateSegment(template, "cool", true),
                new TemplateSegment(template, "awesome", true),
                new TemplateSegment(template, "super", true),
            });

            // Assert
            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void Parse_OptionalParameterAtTheEnd()
        {
            // Arrange
            var template = "{cool}/{awesome}/{super?}";

            // Act
            var actual = TemplateParser.ParseTemplate(template);
            var expected = new RouteTemplate(template, new List<TemplateSegment>()
            {
                new TemplateSegment(template, "cool", true),
                new TemplateSegment(template, "awesome", true),
                new TemplateSegment(template, "super?", true),
            });

            // Assert
            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void Parse_EmptyRoutesShouldFail()
        {
            // Arrange

            // Act

            // Assert
            Assert.ThrowsException<InvalidOperationException>(() => TemplateParser.ParseTemplate(""));
            Assert.ThrowsException<InvalidOperationException>(() => TemplateParser.ParseTemplate("/"));
        }

        [TestMethod]
        public void Parse_EmptySegmentsShouldFail()
        {
            // Arrange
            var template = "super//awesome";

            // Act

            // Assert
            Assert.ThrowsException<InvalidOperationException>(() => TemplateParser.ParseTemplate(template));
        }

        [TestMethod]
        public void Parse_OptionalParameterInTheMiddleShouldFail()
        {
            // Arrange
            var template = "{cool}/{awesome?}/{super}";

            // Act

            // Assert
            Assert.ThrowsException<InvalidOperationException>(() => TemplateParser.ParseTemplate(template));
        }

        [TestMethod]
        public void Parse_CatchAllParameterInTheMiddleShouldFail()
        {
            // Arrange
            var template = "{cool}/{*awesome}/{super}";

            // Act

            // Assert
            Assert.ThrowsException<InvalidOperationException>(() => TemplateParser.ParseTemplate(template));
        }
    }
}