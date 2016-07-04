﻿namespace Toubab.Beinder.Paths
{
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class UnderscorePathParserTest
    {
        UnderscoreSyllableParser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new UnderscoreSyllableParser();    
        }

        [Test]
        public void ParseProperty()
        {
            {
                // Act
                Syllables result = _parser.Parse("Property");

                // Assert
                Assert.IsTrue(result.Equals(new Syllables( "property" )));
            }

            {
                // Act
                Syllables result = _parser.Parse("property");

                // Assert
                Assert.IsTrue(result.Equals(new Syllables( "property" )));
            }
        }

        [Test]
        public void ParsePropertyPropertyProperty()
        {
            {
                // Act
                Syllables result = _parser.Parse("Property_Property_Property");

                // Assert
                Assert.IsTrue(result.Equals(new Syllables( "property", "property", "property"  )));
            }

            {
                // Act
                Syllables result = _parser.Parse("property_property_property");

                // Assert
                Assert.IsTrue(result.Equals(new Syllables( "property", "property", "property"  )));
            }
        }

        [Test]
        public void ParseXYZPropertyPropertyProperty()
        {
            {
                // Act
                Syllables result = _parser.Parse("XYZProperty_Property_Property");

                // Assert
                Assert.IsTrue(result.Equals(new Syllables( "xyzproperty", "property", "property"  )));

            }
        }
    }
}

