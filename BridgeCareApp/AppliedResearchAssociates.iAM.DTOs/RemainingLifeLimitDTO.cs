using System;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// Parameter to define remaining life based on a single attribute. A
    /// simulation may have more than one remaining life limit parameter - for
    /// a specific asset, the lowest remaining life is always used for
    /// the overall remaining life of that asset
    /// </summary>
    public class RemainingLifeLimitDTO : BaseDTO
    {
        /// <summary>
        /// Name of the attribute representing the condition used in the
        /// remaining life calculation
        /// </summary>
        public string Attribute { get; set; }

        /// <summary>
        /// Value of the condition where remaining life is zero
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// ID of an associated library
        /// </summary>
        public Guid LibraryId { get; set; }

        /// <summary>
        /// Has this setting been modified from the associated library?
        /// </summary>
        public bool IsModified { get; set; }

        /// <summary>
        /// Defines the assets that can use this remaining life
        /// </summary>
        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
