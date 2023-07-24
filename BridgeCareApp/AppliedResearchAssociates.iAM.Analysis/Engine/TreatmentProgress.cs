using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

internal sealed class TreatmentProgress
{
    public TreatmentProgress(Treatment treatment, TreatmentConsiderationDetail treatmentConsideration)
    {
        Treatment = treatment ?? throw new ArgumentNullException(nameof(treatment));
        TreatmentConsideration = treatmentConsideration ?? throw new ArgumentNullException(nameof(treatmentConsideration));
    }

    public bool IsComplete { get; set; }

    public Treatment Treatment { get; }

    public TreatmentConsiderationDetail TreatmentConsideration { get; }
}
