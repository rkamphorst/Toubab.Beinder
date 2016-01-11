using System;
using NUnit.Framework;

namespace Toubab.Beinder.PathParser
{
    [TestFixture]
    public class CamelCasePropertyPathParserTest
    {
        CamelCasePathParser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new CamelCasePathParser();    
        }

        [Test]
        public void ParseProperty()
        {
            {
                // Act
                Path result = _parser.Parse("Property");

                // Assert
                Assert.AreEqual("property", result.ToString());
            }

            {
                // Act
                Path result = _parser.Parse("property");

                // Assert
                Assert.AreEqual("property", result.ToString());
            }
        }

        [Test]
        public void ParsePropertyCamelCased()
        {
            // Act
            Path result = _parser.Parse("PropertyPropertyProperty");

            // Assert
            Assert.AreEqual("property/property/property", result.ToString());
        }

        [Test]
        public void ParsePropertyUpperAndCamelCased()
        {
            // Act
            Path result = _parser.Parse("XYZPropertyPropertyProperty");

            // Assert
            Assert.AreEqual("xyzproperty/property/property", result.ToString());
        }
    }
}

