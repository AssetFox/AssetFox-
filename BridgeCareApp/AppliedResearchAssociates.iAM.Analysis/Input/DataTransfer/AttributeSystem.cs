using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;

public sealed class AttributeSystem
{
    public string AgeAttributeName { get; set; }

    public List<CalculatedField> CalculatedFields { get; init; } = new();

    public List<NumberAttribute> NumberAttributes { get; init; } = new();

    public List<TextAttribute> TextAttributes { get; init; } = new();
}
