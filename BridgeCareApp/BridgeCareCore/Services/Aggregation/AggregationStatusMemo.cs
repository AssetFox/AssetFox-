using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services.Aggregation
{
    public class AggregationStatusMemo
    {
        public NetworkRollupDetailDTO rollupDetailDto { get; set; }
        public string ErrorMessage { get; set; }
        public double Percentage { get; set; }
    }
}
