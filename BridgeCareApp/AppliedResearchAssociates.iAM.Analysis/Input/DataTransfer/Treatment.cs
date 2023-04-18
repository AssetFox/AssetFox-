using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;

public class Treatment
{
    public string Name { get; set; }

    public int ShadowForAnyTreatment { get; set; }

    public int ShadowForSameTreatment { get; set; }

    public TreatmentCategory Category { get; set; }
}
