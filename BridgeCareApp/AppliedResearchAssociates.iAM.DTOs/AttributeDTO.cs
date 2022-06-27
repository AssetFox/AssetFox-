using System;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class AttributeDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string AggregationRuleType { get; set; }

        public string Command { get; set; }

        public string DefaultValue { get; set; }

        public double? Minimum { get; set; }

        public double? Maximum { get; set; }

        public bool IsCalculated { get; set; }

        public bool IsAscending { get; set; }

        public string DataSourceType { get; set; }
    }
}
