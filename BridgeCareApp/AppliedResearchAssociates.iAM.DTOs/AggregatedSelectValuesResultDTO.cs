using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class AggregatedSelectValuesResultDTO
    {
        public AttributeDTO Attribute { get; set; }
        public List<string> Values { get; set; }
        public string ResultType { get; set; }
        public bool IsNumber { get; set; }
    }
}
