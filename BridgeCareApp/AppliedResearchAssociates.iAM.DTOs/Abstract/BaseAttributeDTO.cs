using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DTOs.Abstract
{
    public class BaseAttributeDTO
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
    }
}
