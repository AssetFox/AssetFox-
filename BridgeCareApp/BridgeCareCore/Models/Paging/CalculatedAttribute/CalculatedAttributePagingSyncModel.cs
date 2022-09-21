using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Models
{
    public class CalculatedAttributePagingSyncModel
    {
        public Guid? LibraryId { get; set; }
        public List<CalculatedAttributeDTO> UpdatedCalculatedAttributes { get; set; }
        public Dictionary<Guid, List<CalculatedAttributeEquationCriteriaPairDTO>> AddedPairs { get; set; }
        public Dictionary<Guid, List<CalculatedAttributeEquationCriteriaPairDTO>> UpdatedPairs { get; set; }
        public Dictionary<Guid, List<Guid>> DeletedPairs { get; set; }
    }
}
