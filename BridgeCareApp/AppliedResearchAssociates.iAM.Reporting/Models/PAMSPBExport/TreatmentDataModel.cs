﻿using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Reporting.Models.PAMSPBExport
{
    public class TreatmentDataModel
    {
        public Guid SimulationId { get; set; }

        public Guid NetworkId { get; set; }

        public Guid MaintainableAssetId { get; set; }

        public string District { get; set; }

        public string Cnty { get; set; }

        public string Route { get; set; }

        public string AssetName { get; set; }

        public string Direction { get; set; }

        public string FromSection { get; set; } 

        public string ToSection { get; set; } 

        public int Year { get; set; }

        public int MinYear { get; set; }

        public int MaxYear { get; set; }

        public string Interstate { get; set; }

        public string Appliedtreatment { get; set; }

        public double Cost { get; set; }

        public double Benefit { get; set; }

        public double RiskScore { get; set; }

        public double? RemainingLife { get; set; }

        public int? PriorityLevel { get; set; }

        public int TreatmentFundingIgnoresSpendingLimit { get; set; }

        public string TreatmentStatus { get; set; }

        public string TreatmentCause { get; set; }

        public string Budget { get; set; } 

        public string Category { get; set; } 

        public List<double> TreatmentAttributeValues { get; set; } = new List<double>();
    }
}
