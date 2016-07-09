namespace Toubab.Beinder.Paths
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class PathTest
    {
        [Test]
        public void CompareSameStartFragments()
        {
            // Arrange
            var path1 = new Path(new Fragment("banaan", "appel", "meloen"));
            var path2 = new Path(new Fragment("banaan", "appel"));

            {
                // Act
                var cmp = Path.SyllableComparer.Compare(path1, path2);

                // Assert
                Assert.IsTrue(cmp > 0);

            }

            {
                // Act
                var cmp = Path.SyllableComparer.Compare(path2, path1);

                // Assert
                Assert.IsTrue(cmp < 0);
            }
        }

        [Test]
        public void CompareSameFragmentsDifferentDistribution()
        {
            // Arrange
            var path1 = new Path(new Fragment("banaan", "appel", "meloen"));
            var path2 = new Path(new Path(new Fragment("banaan")), new Fragment( "appel", "meloen" ));

            {
                // Act
                var cmp = Path.SyllableComparer.Compare(path1, path2);

                // Assert
                Assert.IsTrue(cmp == 0);

            }

            {
                // Act
                var cmp = Path.SyllableComparer.Compare(path2, path1);

                // Assert
                Assert.IsTrue(cmp == 0);
            }
        }

        [Test]
        public void OrderSameStartSyllables()
        {
            // Arrange
            var path1 = new Path(new Path(new Fragment("banaan", "appel")), new Fragment( "peer"));
            var path2 = new Path(new Fragment("banaan", "appel"));
            var path3 = new Path(new Path(new Fragment("banaan")), new Fragment("appel", "meloen"));
            var lst = new List<Path> { path1, path2, path3 };

            // Act
            var ar = lst.OrderBy(p => p, Path.SyllableComparer).ToArray();

            // Assert
            Assert.AreEqual(path2, ar[0], "eerste element");
            Assert.AreEqual(path3, ar[1], "tweede element");
            Assert.AreEqual(path1, ar[2], "derde element");
        }
    }
}

