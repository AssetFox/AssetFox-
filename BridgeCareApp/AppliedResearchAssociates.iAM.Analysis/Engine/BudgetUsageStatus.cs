namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    public enum BudgetUsageStatus
    {
        /// <summary>
        ///     Indicates a budget that paid the entire remaining cost of the treatment.
        /// </summary>
        CostCoveredInFull,

        /// <summary>
        ///     Indicates a budget that paid some of the remaining cost of the treatment. Only
        ///     possible when <see cref="AnalysisMethod.ShouldUseExtraFundsAcrossBudgets"/> is true.
        ///     Mutually exclusive with <see cref="CostNotCovered"/>.
        /// </summary>
        CostCoveredInPart,

        /// <summary>
        ///     Indicates a budget that was insufficient to pay the remaining cost of the treatment.
        ///     Only possible when <see cref="AnalysisMethod.ShouldUseExtraFundsAcrossBudgets"/> is
        ///     false. Mutually exclusive with <see cref="CostCoveredInPart"/>.
        /// </summary>
        CostNotCovered,

        /// <summary>
        ///     Indicates a budget with one or more user-defined conditions, none of which were met.
        /// </summary>
        ConditionNotMet,

        /// <summary>
        ///     Indicates a budget that was usable, but other budgets before this one in the
        ///     scenario's budget order were sufficient to pay for the treatment.
        /// </summary>
        NotNeeded,

        /// <summary>
        ///     Indicates a budget excluded by the treatment's settings.
        /// </summary>
        NotUsable,
    }
}
