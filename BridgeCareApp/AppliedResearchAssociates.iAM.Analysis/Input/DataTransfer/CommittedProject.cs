using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;

public sealed class CommittedProject : Treatment
{
    public Guid AssetID { get; set; }

    public double Cost { get; set; }

    public string NameOfTemplateTreatment { get; set; }

    public string NameOfUsableBudget { get; set; }

    public int Year { get; set; }

    public string ProjectSource { get; set; }
}
