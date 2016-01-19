using System;
using NUnit.Framework;
using Toubab.Beinder.Path;

namespace Toubab.Beinder.PathParser
{
    [TestFixture]
    public class CamelCasePathParserTest
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
                Path.Path result = _parser.Parse("Property");

                // Assert
                Assert.AreEqual("property", result.ToString());
            }

            {
                // Act
                Path.Path result = _parser.Parse("property");

                // Assert
                Assert.AreEqual("property", result.ToString());
            }
        }

        [Test]
        public void ParsePropertyCamelCased()
        {
            // Act
            Path.Path result = _parser.Parse("PropertyPropertyProperty");

            // Assert
            Assert.AreEqual("property/property/property", result.ToString());
        }

        [Test]
        public void ParsePropertyUpperAndCamelCased()
        {
            // Act
            Path.Path result = _parser.Parse("XYZPropertyPropertyProperty");

            // Assert
            Assert.AreEqual("xyzproperty/property/property", result.ToString());
        }
    }
}

