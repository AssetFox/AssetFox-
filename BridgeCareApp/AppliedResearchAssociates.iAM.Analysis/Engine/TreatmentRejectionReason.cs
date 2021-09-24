namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    public enum TreatmentRejectionReason
    {
        Undefined,
        WithinShadowForAnyTreatment,
        WithinShadowForSameTreatment,
        NotFeasible,
        Superseded,
        InvalidCost,
        CostIsBelowMinimumProjectCostLimit,
    }
}
