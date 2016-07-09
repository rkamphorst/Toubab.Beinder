namespace Toubab.Beinder.Paths
{
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class CamelCasePathParserTest
    {
        CamelCaseSyllableParser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new CamelCaseSyllableParser();    
        }

        [Test]
        public void ParseProperty()
        {
            {
                // Act
                Fragment result = _parser.Parse("Property");

                // Assert
                Assert.IsTrue(result.Equals(new Fragment( "property" )));
            }

            {
                // Act
                Fragment result = _parser.Parse("property");

                // Assert
                Assert.IsTrue(result.Equals(new Fragment( "property" )));
            }
        }

        [Test]
        public void ParsePropertyCamelCased()
        {
            // Act
            Fragment result = _parser.Parse("PropertyPropertyProperty");

            // Assert
            Assert.IsTrue(result.Equals(new Fragment( "property", "property", "property"  )));
        }

        [Test]
        public void ParsePropertyUpperAndCamelCased()
        {
            // Act
            Fragment result = _parser.Parse("XYZPropertyPropertyProperty");

            // Assert
            Assert.IsTrue(result.Equals(new Fragment( "xyzproperty", "property", "property"  )));
        }
    }
}

