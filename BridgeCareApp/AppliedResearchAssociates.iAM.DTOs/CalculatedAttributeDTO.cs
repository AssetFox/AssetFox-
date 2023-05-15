using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// .
    /// </summary>
    public class CalculatedAttributeDTO : BaseDTO
    {
        /// <summary>
        /// .
        /// </summary>
        public string Attribute { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public int CalculationTiming { get; set; }

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
        public IList<CalculatedAttributeEquationCriteriaPairDTO> Equations { get; set; } = new List<CalculatedAttributeEquationCriteriaPairDTO>();
    }
}
