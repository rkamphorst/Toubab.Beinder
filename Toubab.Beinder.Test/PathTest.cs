using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Toubab.Beinder
{
    [TestFixture]
    public class PathTest
    {
        [Test]
        public void CompareSameStartFragments()
        {
            // Arrange
            var path1 = (Path.Path)new[] { "banaan", "appel", "meloen" };
            var path2 = (Path.Path)new[] { "banaan", "appel" };

            {
                // Act
                var cmp = path1.CompareTo(path2);

                // Assert
                Assert.IsTrue(cmp > 0);

            }

            {
                // Act
                var cmp = path2.CompareTo(path1);

                // Assert
                Assert.IsTrue(cmp < 0);
            }
        }

        [Test]
        public void OrderSameStartFragments()
        {
            // Arrange
            var path1 = (Path.Path)new[] { "banaan", "appel", "peer" };
            var path2 = (Path.Path)new[] { "banaan", "appel" };
            var path3 = (Path.Path)new[] { "banaan", "appel", "meloen" };
            var lst = new List<Path.Path> { path1, path2, path3 };

            // Act
            var ar = lst.OrderBy(p => p).ToArray();

            // Assert
            Assert.AreEqual(path2, ar[0], "eerste element");
            Assert.AreEqual(path3, ar[1], "tweede element");
            Assert.AreEqual(path1, ar[2], "derde element");
        }
    }
}

