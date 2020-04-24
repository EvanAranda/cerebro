using System.ComponentModel.Design;
using System.Linq;
using System;
using System.Collections.Generic;
namespace cerebro.htm
{
    using cerebro.math;

    /// <summary>
    /// Wraps a mini column and stores information related to it
    /// </summary>
    public class SpatialPoolerColumn
    {
        public MiniColumn MiniColumn { get; set; }
        public bool IsActive { get; set; }
        public double OverlapScore { get; set; }
        public double BoostFactor { get; set; }
        public double ActiveDutyCycle { get; set; }
        public double OverlapDutyCycle { get; set; }

        public double UpdateBoostFactor(double b, double a, double ma)
        {
            BoostFactor = Math.Exp(-b * (a - ma));
            return BoostFactor;
        }

        public double UpdateActiveDutyCycle(int active, int T)
        {
            ActiveDutyCycle = (ActiveDutyCycle * (T - 1) + active) / T;
            return ActiveDutyCycle;
        }

        public double UpdateOverlapDutyCycle(int overlap, int T)
        {
            OverlapDutyCycle = (OverlapDutyCycle * (T - 1) + overlap) / T;
            return OverlapDutyCycle;
        }

        public double ComputeOverlap(SparseBitArray input)
        {
            var overlapCount = MiniColumn.ActiveConnections.OverlapCount(input);
            return overlapCount * BoostFactor;
        }

        public SpatialPoolerColumn(MiniColumn miniColumn)
        {
            MiniColumn = miniColumn;

            var rng = new Random();

            double minBoost = 0.95;
            double maxBoost = 1.05;
            BoostFactor = rng.NextDouble(minBoost, maxBoost);
        }
    }

    public struct SpatialPoolerCreationOptions
    {
        public int ActivationsPerInhibitionRegion { get; set; }
        public double StimulusThreshold { get; set; }
        public double SynapticPermanenceInc { get; set; }
        public double SynapticPermanenceDec { get; set; }
        public int DutyCycleLength { get; set; }
        public double BoostStrength { get; set; }
    }

    public class SpatialPooler
    {
        public Cortex CorticalRegion { get; }
        public SpatialPoolerCreationOptions Parameters { get; }
        public IReadOnlyList<SpatialPoolerColumn> PoolerColumns => _PoolerColumns;
        public IReadOnlyList<SpatialPoolerColumn> ActiveColumns => _ActiveColumns;
        public double MeanActiveDutyCycle => _MeanActiveDutyCycle;

        public List<SpatialPoolerColumn> ComputeActive(SparseBitArray input)
        {
            var overlapScores = new List<double>(CorticalRegion.MiniColumns.Count);

            foreach (var column in PoolerColumns)
            {
                column.IsActive = false;
                var overlapScore = column.ComputeOverlap(input);
                column.OverlapScore = overlapScore;
                overlapScores.Add(overlapScore);
            }

            var topScores = overlapScores.KMax(Parameters.ActivationsPerInhibitionRegion);
            var minimumScore = topScores.Last();

            var activeColumns = new List<SpatialPoolerColumn>();

            foreach (var column in PoolerColumns)
            {
                if (overlapScores[column.MiniColumn.LinearIndex] > Parameters.StimulusThreshold &&
                    overlapScores[column.MiniColumn.LinearIndex] >= minimumScore)
                {
                    column.IsActive = true;
                    activeColumns.Add(column);
                }
            }

            _ActiveColumns = activeColumns;

            return activeColumns;
        }

        public void Learn(IEnumerable<SpatialPoolerColumn> activeColumns, SparseBitArray activeInputs)
        {
            foreach (var column in activeColumns)
            {
                column.IsActive = true;
                column.MiniColumn.UpdatePermanences(activeInputs, Parameters.SynapticPermanenceInc, Parameters.SynapticPermanenceDec);
            }

            foreach (var column in PoolerColumns)
            {
                int active = column.IsActive ? 1 : 0;
                column.UpdateActiveDutyCycle(active, Parameters.DutyCycleLength);
                column.IsActive = false;

                int overlapped = column.OverlapScore > Parameters.StimulusThreshold ? 1 : 0;
                column.UpdateOverlapDutyCycle(overlapped, Parameters.DutyCycleLength);
            }

            _MeanActiveDutyCycle = PoolerColumns.Average(column => column.ActiveDutyCycle);

            foreach (var column in PoolerColumns)
            {
                column.UpdateBoostFactor(Parameters.BoostStrength, column.ActiveDutyCycle, _MeanActiveDutyCycle);
                column.MiniColumn.UpdateSynapses();
            }
        }

        public void ProcessAndLearn(SparseBitArray activeInputs)
        {
            var activeColumns = ComputeActive(activeInputs);
            Learn(activeColumns, activeInputs);
        }

        public SpatialPooler(SpatialPoolerCreationOptions options, Cortex corticalRegion)
        {
            Parameters = options;
            CorticalRegion = corticalRegion;
            _PoolerColumns = corticalRegion.MiniColumns.Select(mc => new SpatialPoolerColumn(mc)).ToList();
        }

        private List<SpatialPoolerColumn> _PoolerColumns;
        private List<SpatialPoolerColumn> _ActiveColumns;
        private double _MeanActiveDutyCycle;
    }
}