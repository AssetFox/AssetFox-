using System;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// Defines a goals for average condition level in a simulation. An average
    /// condition goal is used when the user desires a specific average condition
    /// (weighted by the spatial weighting factor) to be achieved by the
    /// simulation. These goals may be based on a particular year of the 
    /// simulation as part of a capital improvement plan that aims to calculate 
    /// teh funding needed to meet a certain goal.
    /// </summary>
    public class TargetConditionGoalDTO : BaseDTO
    {
        /// <summary>
        /// Name of the average condition goal
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Attribute representing the condition used for the goal
        /// </summary>
        public string Attribute { get; set; }

        /// <summary>
        /// The target average condition
        /// </summary>
        public double Target { get; set; }

        /// <summary>
        /// The year the condition goal should be met
        /// </summary>
        public int? Year { get; set; }

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
