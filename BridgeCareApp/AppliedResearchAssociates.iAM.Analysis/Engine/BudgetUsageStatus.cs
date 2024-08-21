namespace AppliedResearchAssociates.iAM.Analysis.Engine;

/// <summary>
///     Represents the manner in which a budget was or was not used.
/// </summary>
public enum BudgetUsageStatus
{
    /// <summary>
    ///     Indicates the existence of incomplete logic in the analysis engine.
    /// </summary>
    Undefined,

    /// <summary>
    ///     Indicates a budget that paid at least some of the cost of the treatment.
    /// </summary>
    CostCovered,

    /// <summary>
    ///     Indicates a budget that could not pay for the treatment.
    /// </summary>
    CostNotCovered,

    /// <summary>
    ///     Indicates a budget with one or more user-defined conditions, none of which were met.
    /// </summary>
    ConditionNotMet,

    /// <summary>
    ///     Indicates a budget that was usable, but other budgets before this one in the scenario's
    ///     budget order were sufficient.
    /// </summary>
    NotNeeded,

    /// <summary>
    ///     Indicates a budget excluded by the treatment's settings.
    /// </summary>
    NotUsable,
}
