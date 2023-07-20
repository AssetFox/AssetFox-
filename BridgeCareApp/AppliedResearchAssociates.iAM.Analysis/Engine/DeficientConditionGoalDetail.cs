namespace AppliedResearchAssociates.iAM.Analysis.Engine;

/// <summary>
///     The maximum deficiency allowed for a specific attribute. Typically associated with a
///     specific time (i.e., year) in the scenario.
/// </summary>
public sealed class DeficientConditionGoalDetail : ConditionGoalDetail
{
    /// <summary>
    ///     The acutal percentage (weighted by the spatial weighting factor) of the assets that do
    ///     not meet the <see cref="AllowedDeficientPercentage"/> at the END of the time period.
    /// </summary>
    public double ActualDeficientPercentage { get; set; }

    /// <summary>
    ///     The allowed percentage (weighted by the spatial weighting factor) of the assets that can
    ///     be deficient.
    /// </summary>
    public double AllowedDeficientPercentage { get; set; }

    /// <summary>
    ///     The value that determines if an asset is deficient.
    /// </summary>
    public double DeficientLimit { get; set; }
}
