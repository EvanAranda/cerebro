using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using cerebro.math;

namespace cerebro.test
{
    [TestClass]
    public class SparseBitArrayTests
    {
        [TestMethod]
        public void Indexers()
        {
            var indices = new[] { 0, 20, 25, 28, 35, 44, 47, 49 };

            var array = new SparseBitArray(indices, 50);

            foreach (var index in indices)
            {
                Assert.IsTrue(array.Contains(index));
            }

            for (var i = 0; i < 50; i++)
            {
                if (indices.Contains(i))
                {
                    Assert.AreEqual(1, array[i]);
                }
                else
                {
                    Assert.AreEqual(0, array[i]);
                }
            }
        }

        [TestMethod]
        public void LogicalOperators()
        {
            throw new AssertFailedException();
        }

        [TestMethod]
        public void OverlapCount()
        {
            var indices = new[] { 0, 20, 25, 28, 35, 44, 47, 49 };
            var indices2 = new[] { 0, 20, 25, 27, 35, 43, 47, 49 };

            Assert.AreEqual(6, indices.OverlapCount(indices2));
            Assert.AreEqual(6, indices2.OverlapCount(indices));
        }
    }
}