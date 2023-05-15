namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    /// <summary>
    /// .
    /// </summary>
    public enum TreatmentStatus
    {
        /// <summary>
        ///     Indicates the existence of incomplete logic in the analysis engine.
        /// </summary>
        Undefined,

        /// <summary>
        ///     Indicates a treatment that has been fully applied.
        /// </summary>
        Applied,

        /// <summary>
        ///     Indicates a treatment that has been partially applied (during the leading years of a
        ///     multi-year treatment period).
        /// </summary>
        Progressed,
    }
}
