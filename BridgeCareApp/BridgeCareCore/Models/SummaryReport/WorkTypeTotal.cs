using System;
using System.Collections.Generic;

namespace BridgeCareCore.Models.SummaryReport
{
    public class WorkTypeTotal
    {
        public Dictionary<int, double> MPMSpreservationCostPerYear { get; set; } = new Dictionary<int, double>();
        public Dictionary<int, double> BAMSPreservationCostPerYear { get; set; } = new Dictionary<int, double>();
        public Dictionary<int, double> MPMSEmergencyRepairCostPerYear { get; set; } = new Dictionary<int, double>();
        public Dictionary<int, double> MPMSRehabRepairCostPerYear { get; set; } = new Dictionary<int, double>();
        public Dictionary<int, double> CulvertRehabCostPerYear { get; set; } = new Dictionary<int, double>();
        public Dictionary<int, double> BAMSRehabCostPerYear { get; set; } = new Dictionary<int, double>();
        public Dictionary<int, double> MPMSReplacementCostPerYear { get; set; } = new Dictionary<int, double>();
        public Dictionary<int, double> CulvertReplacementCostPerYear { get; set; } = new Dictionary<int, double>();
        public Dictionary<int, double> BAMSReplacementCostPerYear { get; set; } = new Dictionary<int, double>();

        public Dictionary<int, double> OtherCostPerYear { get; set; } = new Dictionary<int, double>();

        public Dictionary<int, double> TotalCostPerYear { get; set; } = new Dictionary<int, double>();
    }
}
