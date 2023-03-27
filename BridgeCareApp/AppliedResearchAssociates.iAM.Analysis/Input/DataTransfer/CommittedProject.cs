using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer
{
    public sealed class CommittedProject : Treatment
    {
        public string AssetName { get; set; }

        public List<TreatmentConsequence> Consequences { get; set; }

        public double Cost { get; set; }

        public string NameOfUsableBudget { get; set; }

        public int Year { get; set; }
    }
}
