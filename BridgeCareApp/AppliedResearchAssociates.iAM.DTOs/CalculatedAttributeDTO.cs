using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// Describes whose value is calculated (as opposed to an attribute
    /// whose value is provided by the user)
    /// </summary>
    public class CalculatedAttributeDTO : BaseDTO
    {
        /// <summary>
        /// Name of the attribute
        /// </summary>
        public string Attribute { get; set; }

        /// <summary>
        /// When the attribute should be calculated
        /// </summary>
        public int CalculationTiming { get; set; }

        /// <summary>
        /// ID of an associated library
        /// </summary>
        public Guid LibraryId { get; set; }

        /// <summary>
        /// Has this setting been modified from the associated library?
        /// </summary>
        public bool IsModified { get; set; }

        /// <summary>
        /// List of the equations used to calculate this attribute
        /// </summary>
        public IList<CalculatedAttributeEquationCriteriaPairDTO> Equations { get; set; } = new List<CalculatedAttributeEquationCriteriaPairDTO>();
    }
}
