using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport
{
    public class YearsData
    {
        public int Year { get; set; }

        public string TreatmentName { get; set; }
        public TreatmentCategory TreatmentCategory { get; set; }
        public AssetCategories AssetType { get; set; }
        public int SurfaceId { get; set; }

        public double Amount { get; set; }
        public bool isCommitted { get; set; } = false;
    }
}
