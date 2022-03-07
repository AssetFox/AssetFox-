﻿namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    public enum ReasonAgainstCashFlow
    {
        /// <summary>
        ///     Indicates the existence of incomplete logic in the analysis engine.
        /// </summary>
        Undefined,

        /// <summary>
        ///     Indicates a cash flow rule that was after a selected cash flow rule in the
        ///     scenario's cash flow rule order.
        /// </summary>
        NotNeeded,

        /// <summary>
        ///     Indicates a cash flow rule that lacked a distribution rule applicable to the
        ///     treatment cost.
        /// </summary>
        NoApplicableDistributionRule,

        /// <summary>
        ///     Indicates a cash flow rule whose applicable distribution rule is for only one year
        ///     and thus is unnecessary.
        /// </summary>
        ApplicableDistributionRuleIsForOnlyOneYear,

        /// <summary>
        ///     Indicates a cash flow rule whose applicable distribution rule would have extended
        ///     the cash flow beyond the end of the analysis period.
        /// </summary>
        LastYearOfCashFlowIsOutsideOfAnalysisPeriod,

        /// <summary>
        ///     Indicates a cash flow rule whose applicable distribution rule would extend the cash
        ///     flow into a year blocked by other work, e.g. a previously scheduled treatment.
        /// </summary>
        FutureEventScheduleIsBlocked,

        /// <summary>
        ///     Indicates a cash flow rule whose applicable distribution rule could not be applied
        ///     due to lack of funding in the distribution period.
        /// </summary>
        FundingIsNotAvailable,

        /// <summary>
        ///     Indicates a selected cash flow rule.
        /// </summary>
        None,
    }
}
