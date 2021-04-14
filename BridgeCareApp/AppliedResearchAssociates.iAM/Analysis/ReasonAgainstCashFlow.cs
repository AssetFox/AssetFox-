namespace AppliedResearchAssociates.iAM.Analysis
{
    public enum ReasonAgainstCashFlow
    {
        Undefined,
        NotNeeded,
        NoApplicableDistributionRule,
        ApplicableDistributionRuleIsForOnlyOneYear,
        LastYearOfCashFlowIsOutsideOfAnalysisPeriod,
        FutureEventScheduleIsBlocked,
        FundingIsNotAvailable,
        None,
    }
}
