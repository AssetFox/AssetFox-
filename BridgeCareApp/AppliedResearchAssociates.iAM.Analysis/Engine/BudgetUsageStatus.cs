using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    public enum BudgetUsageStatus
    {
        /// <summary>
        ///     If present in final output, this indicates missing or incorrect logic in the
        ///     analysis engine.
        /// </summary>
        Undefined,

        /// <summary>
        ///     Indicates a budget that paid at least some of the cost of the treatment.
        /// </summary>
        CostCovered,

        /// <summary>
        ///     Indicates a budget that did not pay any cost of the treatment.
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
