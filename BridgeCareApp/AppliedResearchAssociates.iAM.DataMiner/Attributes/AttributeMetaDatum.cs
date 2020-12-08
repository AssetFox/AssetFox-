using System;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class AttributeMetaDatum
    {
        public string Name { get; set; }

        public Guid Id { get; set; }

        public string DefaultValue { get; set; }

        public double? Minimum { get; set; }

        public double? Maximum { get; set; }

        public string ConnectionString { get; set; }

        public string DataSource { get; set; }

        public string Type { get; set; }

        public string Location { get; set; }

        public string Command { get; set; }

        public string AggregationRule { get; set; }

        public bool IsCalculated { get; set; }

        public bool IsAscending { get; set; }

        public ConnectionType ConnectionType { get; set; }
    }
}
