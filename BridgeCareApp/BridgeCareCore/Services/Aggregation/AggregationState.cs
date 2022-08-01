using System;
using System.Threading.Tasks;

namespace BridgeCareCore.Services.Aggregation
{
    public class AggregationState
    {
        public int Count { get; set; }
        public string Status { get; set; } = string.Empty;
        public double Percentage { get; set; }
        public Guid NetworkId { get; set; } = Guid.Empty;
        public Task CurrentRunningTask { get; set; }
        public string ErrorMessage { get; set; }
    }
}
