namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;

public sealed class DeficientConditionGoal : ConditionGoal
{
    public double AllowedDeficientPercentage { get; set; }

    public double DeficientLimit { get; set; }
}
