using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport
{
    public class YearsData
    {
        public int Year { get; set; }

        public string Treatment { get; set; }

        public TreatmentCategory TreatmentCategory { get; set; }
        public AssetCategories AssetType { get; set; }

        public string ProjectSource { get; set; }

        public double Amount { get; set; }

        public bool isCommitted { get; set; } = false;

        public (string bpnName , double cost) costPerBPN { get; set; }
    }
}
