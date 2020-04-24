using System.Collections.Generic;

namespace cerebro.htm
{
    using cerebro.math;

    public struct CortexCreationOptions
    {
        public int[] CortexShape { get; set; }
        public int[] InputShape { get; set; }
        public double PotentialSynapsePercent { get; set; }
        public double ConnectedPermanenceThreshold { get; set; }
    }

    public class Cortex
    {
        public CortexCreationOptions Parameters { get; }
        public IReadOnlyList<MiniColumn> MiniColumns => _MiniColumns;

        public Cortex(CortexCreationOptions options)
        {
            Parameters = options;
            _MiniColumns = new List<MiniColumn>();

            int numColumns = options.CortexShape.Product();
            int numInputs = options.InputShape.Product();

            for (int iColumn = 0; iColumn < numColumns; iColumn++)
            {
                var column = new MiniColumn(iColumn, numInputs,
                    options.PotentialSynapsePercent,
                    options.ConnectedPermanenceThreshold);
                _MiniColumns.Add(column);
                column.Region = this;
            }
        }

        private List<MiniColumn> _MiniColumns;
    }
}