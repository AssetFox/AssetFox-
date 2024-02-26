using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport
{
    public class WorkTypeTotal
    {
        public Dictionary<int, double> PreservationCostPerYear { get; set; } = new Dictionary<int, double>();
        public Dictionary<int, double> CapacityAddingCostPerYear { get; set; } = new Dictionary<int, double>();
        public Dictionary<int, double> RehabilitationCostPerYear { get; set; } = new Dictionary<int, double>();
        public Dictionary<int, double> ReplacementCostPerYear { get; set; } = new Dictionary<int, double>();
        public Dictionary<int, double> MaintenanceCostPerYear { get; set; } = new Dictionary<int, double>();
        public Dictionary<int, double> OtherCostPerYear { get; set; } = new Dictionary<int, double>();
        public Dictionary<int, double> WorkOutsideScopeCostPerYear { get; set; } = new Dictionary<int, double>();
        public Dictionary<int, double> Bundled { get; set; } = new Dictionary<int, double>();
        public Dictionary<int, double> TotalCostPerYear { get; set; } = new Dictionary<int, double>();
    }
}
