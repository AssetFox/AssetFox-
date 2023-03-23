using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class CalculatedAttributeDTO : BaseDTO
    {
        public string Attribute { get; set; }

        public int CalculationTiming { get; set; }

        public Guid LibraryId { get; set; }

        public bool IsModified { get; set; }

        public ICollection<CalculatedAttributeEquationCriteriaPairDTO> Equations { get; set; } = new List<CalculatedAttributeEquationCriteriaPairDTO>();
    }
}
