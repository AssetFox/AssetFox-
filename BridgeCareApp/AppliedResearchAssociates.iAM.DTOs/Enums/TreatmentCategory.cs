namespace AppliedResearchAssociates.iAM.DTOs.Enums
{
    /// <summary>
    /// Category of treatment
    /// </summary>
    public enum TreatmentCategory
    {
        /// <summary>
        /// Treatment is low-cost, low impact designed to preserve the condition of assets
        /// already in good condition
        /// </summary>
        Preservation,

        /// <summary>
        /// Primary reason for treatment is to add capacity, not to improve condition.
        /// However, conditions will improve due to this treatment
        /// </summary>
        CapacityAdding,

        /// <summary>
        /// Treatment is designed to address significant issues with the asset but not
        /// fully replace the asset
        /// </summary>
        Rehabilitation,

        /// <summary>
        /// The treatment will replace the exisiting asset
        /// </summary>
        Replacement,

        /// <summary>
        /// Treatment is low-cost and usually applied according to a timed scehdule as opposed
        /// to reacting to specific asset condiitons.
        /// </summary>
        Maintenance,

        /// <summary>
        /// The treatment is applied for reasons outside of condition or capcity
        /// </summary>
        Other,

        ///<summary>
        /// The treatment that is outside scope of scenario criteria
        ///</summary>
        WorkOutsideScope,

        /// <summary>
        /// Bundled treatments
        /// </summary>
        Bundled
    }
}
