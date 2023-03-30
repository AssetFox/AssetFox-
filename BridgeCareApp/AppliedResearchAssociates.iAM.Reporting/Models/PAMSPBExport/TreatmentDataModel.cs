using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Reporting.Models.PAMSPBExport
{
    public class TreatmentDataModel
    {
        public Guid SimulationId { get; set; }

        public Guid SimulationOutputId { get; set; }

        public Guid NetworkId { get; set; }

        public Guid MaintainableAssetId { get; set; }

        public string District { get; set; }

        public string Cnty { get; set; }

        public string SR { get; set; }

        public string AssetName { get; set; }

        public string FromSection { get; set; }

        public string ToSection { get; set; }

        public int Year { get; set; }

        public string Appliedtreatment { get; set; }

        public double Cost { get; set; }

        public double Benefit { get; set; }

        public double RiskScore { get; set; }

        public double? RemainingLife { get; set; }

        public int PriorityLevel { get; set; }

        public int TreatmentFundingIgnoresSpendingLimit { get; set; } // TODO from bool

        public int TreatmentStatus { get; set; } // From enum TreatmentStatus

        public int TreatmentCause { get; set; }

        public string Budget { get; set; } // TODO: source??

        public string Category { get; set; } // TODO source?? If TreatmentCategory, what to consider

        public List<string> Consequences { get; set; } // Treatment attributes i.e. ones getting affected from treatment
    }
}
