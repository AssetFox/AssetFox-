using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport
{
    public class ParametersModel
    {
        public NHSModel nHSModel { get; set; } = new NHSModel();

        public List<string> BPNValues { get; } = new List<string>();

        public List<string> Status { get; } = new List<string>();

        public int P3 { get; set; }

        public string LengthBetween8and20 { get; set; } = "N";

        public string LengthGreaterThan20 { get; set; } = "N";

        public List<string> OwnerCode { get; } = new List<string>();

        public List<string> FunctionalClass { get; } = new List<string>();
    }
}
