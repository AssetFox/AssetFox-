namespace AppliedResearchAssociates.iAM.DTOs.Enums
{
    /// <summary>
    /// Defines the optimization strategy used by a scenario
    /// </summary>
    public enum OptimizationStrategy
    {
        /// <summary>
        /// Optimize treatment selection based only on benefit
        /// </summary>
        Benefit,

        /// <summary>
        /// Optimize treatment selection based on benefit-cost
        /// ratio
        /// </summary>
        BenefitToCostRatio,

        /// <summary>
        /// Optimize treatment selection based only on the
        /// improvement to remaining life
        /// </summary>
        RemainingLife,

        /// <summary>
        /// Optimize treatment selection based on an improved
        /// remaining life to cost ratio
        /// </summary>
        RemainingLifeToCostRatio,
    }
}
