using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;

public sealed class CalculatedField : Attribute
{
    public bool IsDecreasingWithDeterioration { get; set; }

    public CalculatedFieldTiming Timing { get; set; }

    public List<CriterionEquationPair> ValueDefinitions { get; set; } = new();
}
