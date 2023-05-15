using System;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// .
    /// </summary>
    public class DeficientConditionGoalDTO : BaseDTO
    {
        /// <summary>
        /// .
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public string Attribute { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public double AllowedDeficientPercentage { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public double DeficientLimit { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public Guid LibraryId { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public bool IsModified { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
