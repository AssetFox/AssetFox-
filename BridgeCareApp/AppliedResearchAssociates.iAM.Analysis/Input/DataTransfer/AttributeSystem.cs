using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;

public sealed class AttributeSystem
{
    public string AgeAttributeName { get; set; }

    public List<CalculatedField> CalculatedFields { get; set; } = new();

    public List<NumberAttribute> NumberAttributes { get; set; } = new();

    public List<TextAttribute> TextAttributes { get; set; } = new();
}
