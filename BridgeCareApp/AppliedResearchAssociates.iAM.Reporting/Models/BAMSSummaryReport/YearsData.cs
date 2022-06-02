﻿using AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport
{
    public class YearsData
    {
        public int Year { get; set; }

        public string Treatment { get; set; }

        public TreatmentCategory TreatmentCategory { get; set; }
        public AssetCategory AssetType { get; set; }

        public double Amount { get; set; }

        public bool isCommitted { get; set; } = false;

        public (string bpnName , double cost) costPerBPN { get; set; }
    }
}