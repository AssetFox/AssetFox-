using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer
{
    public sealed class Scenario
    {
        public AnalysisMethod AnalysisMethod { get; set; }

        public List<CommittedProject> CommittedProjects { get; set; }

        public InvestmentPlan InvestmentPlan { get; set; }

        public Network Network { get; set; }

        public int NumberOfYearsOfTreatmentOutlook { get; set; }

        public string PassiveTreatmentID { get; set; }

        public List<PerformanceCurve> PerformanceCurves { get; set; }

        public List<SelectableTreatment> SelectableTreatments { get; set; }

        public bool ShouldPreapplyPassiveTreatment { get; set; }
    }
}
