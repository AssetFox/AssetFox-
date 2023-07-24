using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;

public sealed class CommittedProject : Treatment
{
    public Guid AssetID { get; set; }

    public List<TreatmentConsequence> Consequences { get; init; } = new();

    public double Cost { get; set; }

    public string NameOfUsableBudget { get; set; }

    public int Year { get; set; }
}
