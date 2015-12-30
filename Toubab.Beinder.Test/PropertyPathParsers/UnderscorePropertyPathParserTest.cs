using System;
using NUnit.Framework;

namespace Toubab.Beinder.PropertyPathParsers
{
    [TestFixture]
    public class UnderscorePropertyPathParserTest
    {
        UnderscorePropertyPathParser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new UnderscorePropertyPathParser();    
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
        public void ParsePropertyPropertyProperty()
        {
            {
                // Act
                PropertyPath result = _parser.Parse("Property_Property_Property");

                // Assert
                Assert.AreEqual("property/property/property", result.ToString());
            }

            {
                // Act
                PropertyPath result = _parser.Parse("property_property_property");

                // Assert
                Assert.AreEqual("property/property/property", result.ToString());
            }
        }

        [Test]
        public void ParseXYZPropertyPropertyProperty()
        {
            {
                // Act
                PropertyPath result = _parser.Parse("XYZProperty_Property_Property");

                // Assert
                Assert.AreEqual("xyzproperty/property/property", result.ToString());
            }
        }
    }
}

