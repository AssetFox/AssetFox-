using System;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// Describes a specific performance model which must be related to a
    /// specific condition attribute.  This determines the future value of a
    /// condition without any intervening maintenance.
    /// </summary>
    public class PerformanceCurveDTO : BaseDTO
    {
        /// <summary>
        /// Name of the attribute to be modeled
        /// </summary>
        public string Attribute { get; set; }

        /// <summary>
        /// Name of the performance model
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ID of an associated library
        /// </summary>
        public Guid LibraryId { get; set; }

        /// <summary>
        /// Has this setting been modified from the associated library?
        /// </summary>
        public bool IsModified { get; set; }

        /// <summary>
        /// Should the model be shifted to accommodate existing data?
        /// </summary>
        public bool Shift { get; set; }

        /// <summary>
        /// Defines the assets that can use this model
        /// </summary>
        public CriterionLibraryDTO CriterionLibrary { get; set; }

        /// <summary>
        /// The equation used to calculate future performance for this model
        /// </summary>
        public EquationDTO Equation { get; set; }
    }
}
