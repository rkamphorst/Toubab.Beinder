using System;
using NUnit.Framework;

namespace Beinder.PropertyPathParsers
{
    [TestFixture]
    public class CamelCasePropertyPathParserTest
    {
        CamelCasePropertyPathParser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new CamelCasePropertyPathParser();    
        }

        [Test]
        public void ParseProperty()
        {
            {
                // Act
                PropertyPath result = _parser.Parse("Property");

                // Assert
                Assert.AreEqual("property", result.ToString());
            }

            {
                // Act
                PropertyPath result = _parser.Parse("property");

                // Assert
                Assert.AreEqual("property", result.ToString());
            }
        }

        [Test]
        public void ParsePropertyCamelCased()
        {
            // Act
            PropertyPath result = _parser.Parse("PropertyPropertyProperty");

            // Assert
            Assert.AreEqual("property/property/property", result.ToString());
        }

        [Test]
        public void ParsePropertyUpperAndCamelCased()
        {
            // Act
            PropertyPath result = _parser.Parse("XYZPropertyPropertyProperty");

            // Assert
            Assert.AreEqual("xyzproperty/property/property", result.ToString());
        }
    }
}

