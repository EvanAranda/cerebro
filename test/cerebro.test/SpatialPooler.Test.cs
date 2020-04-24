using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using cerebro.math;
using cerebro.htm;
using cerebro.encoders;

namespace cerebro.test
{
    [TestClass]
    public class SpatialPoolerTests
    {
        [TestMethod]
        public void ComputeActive()
        {
            var cortex = new Cortex(
                new CortexCreationOptions
                {
                    CortexShape = new int[] { 32, 32 },
                    InputShape = new int[] { 20, 20 },
                    ConnectedPermanenceThreshold = 0.5,
                    PotentialSynapsePercent = 0.75
                }
            );

            var sp = new SpatialPooler(
                new SpatialPoolerCreationOptions
                {
                    ActivationsPerInhibitionRegion = 10,
                    StimulusThreshold = 1.0,
                    SynapticPermanenceInc = 0.1,
                    SynapticPermanenceDec = 0.02,
                    BoostStrength = 100,
                    DutyCycleLength = 1000
                },
                cortex
            );

            var inputs = InputGenerator<double>.SinWave(1, 0, Math.PI, 50);

            foreach (var input in inputs)
            {
                sp.ProcessAndLearn(input);
            }
        }

    }
}