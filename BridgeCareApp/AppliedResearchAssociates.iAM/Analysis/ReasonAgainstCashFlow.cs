namespace AppliedResearchAssociates.iAM.Analysis
{
    public enum ReasonAgainstCashFlow
    {
        Undefined,
        TreatmentIsPartOfActiveCashFlowProject,
        TreatmentIsDueToCommittedProject,
        NoConditionMetForAnyCashFlowRule,
        ApplicableDistributionRuleIsForOnlyOneYear,
        LastYearOfCashFlowIsOutsideOfAnalysisPeriod,
        FutureEventScheduleIsBlocked,
        FutureFundingIsNotAvailable,
        None,
    }
}
