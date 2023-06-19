using System;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// Defines a goal for maximum deficiency levels in a simulation. A deficiency
    /// goal is used when the user desires no more than a certain percentage of
    /// assets in the simulation (weighted by the spatial weighting factor) should
    /// have a particular condition worse than the provided deficiency level.  These
    /// goals may be based on a particular year of the simulation as part of a
    /// capital improvement plan that addresses excess deficiencies.
    /// </summary>
    public class DeficientConditionGoalDTO : BaseDTO
    {
        /// <summary>
        /// Name of the deficient condition goal
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Attribute representing the condition used for the goal
        /// </summary>
        public string Attribute { get; set; }

        /// <summary>
        /// Maximum percent of assets that should be less than the goal
        /// </summary>
        public double AllowedDeficientPercentage { get; set; }

        /// <summary>
        /// Condition value at which an asset becomes deficient
        /// </summary>
        public double DeficientLimit { get; set; }

        /// <summary>
        /// ID of an associated library
        /// </summary>
        public Guid LibraryId { get; set; }

        /// <summary>
        /// Has this setting been modified from the associated library?
        /// </summary>
        public bool IsModified { get; set; }

        /// <summary>
        /// Defines the assets that can use this goal
        /// </summary>
        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
