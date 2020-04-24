using Microsoft.VisualStudio.TestTools.UnitTesting;

using cerebro.htm;
using cerebro.math;
using cerebro.encoders;

namespace cerebro.test
{
    [TestClass]
    public class MiniColumnTests
    {
        [TestMethod]
        public void CreateMiniColumn()
        {
            var miniColumn = new MiniColumn(0, 20 * 20, 0.5, 0.2);
            Assert.IsTrue(miniColumn.ActiveConnections.Capacity <= miniColumn.PotentialConnections.Capacity);
        }
    }
}
