﻿using System;
using NUnit.Framework;
using Toubab.Beinder.Path;

namespace Toubab.Beinder.PathParser
{
    [TestFixture]
    public class UnderscorePathParserTest
    {
        UnderscorePathParser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new UnderscorePathParser();    
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
        public void ParsePropertyPropertyProperty()
        {
            {
                // Act
                Path.Path result = _parser.Parse("Property_Property_Property");

                // Assert
                Assert.AreEqual("property/property/property", result.ToString());
            }

            {
                // Act
                Path.Path result = _parser.Parse("property_property_property");

                // Assert
                Assert.AreEqual("property/property/property", result.ToString());
            }
        }

        [Test]
        public void ParseXYZPropertyPropertyProperty()
        {
            {
                // Act
                Path.Path result = _parser.Parse("XYZProperty_Property_Property");

                // Assert
                Assert.AreEqual("xyzproperty/property/property", result.ToString());
            }
        }
    }
}

