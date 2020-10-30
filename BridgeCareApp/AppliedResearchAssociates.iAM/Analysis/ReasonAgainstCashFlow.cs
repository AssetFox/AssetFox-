﻿namespace AppliedResearchAssociates.iAM.Analysis
{
    public enum ReasonAgainstCashFlow
    {
        Undefined,
        NotNeeded,
        ApplicableDistributionRuleIsForOnlyOneYear,
        LastYearOfCashFlowIsOutsideOfAnalysisPeriod,
        FutureEventScheduleIsBlocked,
        FutureFundingIsNotAvailable,
        None,
    }
}