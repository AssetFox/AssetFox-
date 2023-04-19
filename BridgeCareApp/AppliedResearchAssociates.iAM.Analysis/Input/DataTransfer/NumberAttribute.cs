namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;

public sealed class NumberAttribute : Attribute<double>
{
    public bool IsDecreasingWithDeterioration { get; set; }

    public double? MaximumValue { get; set; }

    public double? MinimumValue { get; set; }
}
