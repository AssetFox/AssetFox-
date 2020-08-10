namespace AppliedResearchAssociates.iAM.Analysis
{
    public enum ReasonAgainstCashFlow
    {
        Undefined,
        TreatmentIsPartOfActiveCashFlowProject,
        TreatmentIsDueToCommittedProject,
        NoConditionMetForAnyCashFlowRule,
        ApplicableDistributionRuleIsNotMultiyear,
        LastYearOfCashFlowIsOutsideOfAnalysisPeriod,
        TreatmentEventScheduleIsNotClear,
        FutureFundingIsNotAvailable,
        None,
    }
}
