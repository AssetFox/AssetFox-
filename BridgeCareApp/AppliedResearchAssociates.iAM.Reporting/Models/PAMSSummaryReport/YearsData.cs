using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport
{
    public class YearsData
    {
        public int Year { get; set; }

        public string TreatmentName { get; set; }
        public TreatmentCategory TreatmentCategory { get; set; }
        public AssetCategory AssetType { get; set; }

        public double Amount { get; set; }
        public bool isCommitted { get; set; } = false;

        // TODO: Copy/Paste from BAMSSummaryReport; is this needed for PAMS?
        //       public (string bpnName , double cost) costPerBPN { get; set; }
    }
}
