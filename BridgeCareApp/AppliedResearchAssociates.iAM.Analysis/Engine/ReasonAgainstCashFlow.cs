namespace AppliedResearchAssociates.iAM.Analysis.Engine
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
