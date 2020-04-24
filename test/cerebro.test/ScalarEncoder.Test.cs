using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

using cerebro.encoders;

namespace cerebro.test
{
    [TestClass]
    public class ScalarEncoderTests
    {
        [TestMethod]
        public void Encode()
        {
            var encoder = new ScalarEncoder(0, 100, 100, 21);
            var value = encoder.Encode(72.0);

            Assert.AreEqual(121, value.Capacity);
            Assert.IsTrue(value.Contains(72));

            var inputs = InputGenerator<double>.SinWave(1, 0, 2 * Math.PI, 50).ToList();

        }
    }
}