using System;
using System.Collections.Generic;
using MathNet.Numerics.Random;
using System.Linq;

namespace cerebro.htm
{
    using cerebro.math;

    public class MiniColumn
    {
        public Cortex Region { get; internal set; }
        public int LinearIndex => _LinearIndex;
        public SparseBitArray PotentialConnections => _PotentialConnections;
        public SparseBitArray ActiveConnections => _ActiveConnections;
        public IReadOnlyList<double> SynapticPermanences => _SynapticPermanences;

        /// <summary>
        /// Reinforces active synapses and discourages inactive ones.
        /// </summary>
        /// <param name="activeInputs"></param>
        /// <param name="inc"></param>
        /// <param name="dec"></param>
        public void UpdatePermanences(SparseBitArray activeInputs, double inc, double dec)
        {
            Action<int, int, int> increment = (x, iPotential, jActiveInput) =>
            {
                _SynapticPermanences[iPotential] += inc;
                _SynapticPermanences[iPotential] = Math.Min(1.0, _SynapticPermanences[iPotential]);
            };

            Action<int, int, int, int> decrement = (x, iPotential, y, jActiveInput) =>
            {
                _SynapticPermanences[iPotential] -= dec;
                _SynapticPermanences[iPotential] = Math.Max(0.0, _SynapticPermanences[iPotential]);
            };

            PotentialConnections.SparseZipMap(activeInputs,
                Comparer<int>.Default.Compare,
                increment,
                decrement,
                decrement);
        }

        /// <summary>
        /// Updates the active connections
        /// </summary>
        public void UpdateSynapses()
        {
            _ActiveConnections = PotentialConnections
                .Where((x, i) => SynapticPermanences[i] >= Region.Parameters.ConnectedPermanenceThreshold)
                .ToSparseBitArray(PotentialConnections.Capacity);
        }

        /// <summary>
        /// Initialize a <see cref="MiniColumn">
        /// </summary>
        /// <param name="inputSize"></param>
        /// <param name="activeBits"></param>
        public MiniColumn(
            int index,
            int inputSize,
            double potentialSynapsePercent,
            double connectedPermThreshold)
        {
            _LinearIndex = index;
            var potentialConnections = new List<int>();
            var activeConnections = new List<int>();
            _SynapticPermanences = new List<double>();

            var rng = new Random();

            for (var i = 0; i < inputSize; i++)
            {
                var r = rng.NextDouble();
                if (r < potentialSynapsePercent)
                {
                    potentialConnections.Add(i);
                    _SynapticPermanences.Add(r);

                    if (r >= connectedPermThreshold)
                        activeConnections.Add(i);
                }
            }

            _PotentialConnections = new SparseBitArray(potentialConnections, inputSize);
            _ActiveConnections = new SparseBitArray(activeConnections, inputSize);
        }

        private SparseBitArray _PotentialConnections;
        private SparseBitArray _ActiveConnections;
        private List<double> _SynapticPermanences;
        private int _LinearIndex;
    }
}