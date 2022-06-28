using System;

namespace BridgeCareCore.Services
{
    public class AggregationState
    {
        public int Count { get; set; }
        public string Status { get; set; } = string.Empty;
        public double Percentage { get; set; }
        public Guid NetworkId { get; set; } = Guid.Empty;
    }
}
