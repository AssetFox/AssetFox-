namespace AppliedResearchAssociates.iAM.Analysis.Engine;

public enum TreatmentRejectionReason
{
    /// <summary>
    ///     Indicates the existence of incomplete logic in the analysis engine.
    /// </summary>
    Undefined,

    /// <summary>
    ///     Indicates a treatment rejected due to a previously selected treatment's "shadow"
    ///     preventing the selection of <em>any</em> treatment.
    /// </summary>
    WithinShadowForAnyTreatment,

    /// <summary>
    ///     Indicates a treatment rejected due to a previously selected treatment's "shadow"
    ///     preventing the selection of <em>the same</em> treatment.
    /// </summary>
    WithinShadowForSameTreatment,

    /// <summary>
    ///     Indicates a treatment whose user-defined feasibility criteria were not satisfied.
    /// </summary>
    NotFeasible,

    /// <summary>
    ///     Indicates a treatment rejected due to a user-defined treatment "supersession".
    /// </summary>
    Superseded,

    /// <summary>
    ///     Indicates a treatment whose cost was negative, zero, or an astronomically large
    ///     positive amount (larger than <see cref="decimal.MaxValue"/>).
    /// </summary>
    InvalidCost,

    /// <summary>
    ///     Indicates a treatment whose cost was less than the scenario's minimum project cost
    ///     limit. See <see cref="InvestmentPlan.MinimumProjectCostLimit"/>.
    /// </summary>
    CostIsBelowMinimumProjectCostLimit,
}
