using System;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// .
    /// </summary>
    public class PerformanceCurveDTO : BaseDTO
    {
        /// <summary>
        /// .
        /// </summary>
        public string Attribute { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public string Name { get; set; }

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
        public bool Shift { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public CriterionLibraryDTO CriterionLibrary { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public EquationDTO Equation { get; set; }
    }
}
