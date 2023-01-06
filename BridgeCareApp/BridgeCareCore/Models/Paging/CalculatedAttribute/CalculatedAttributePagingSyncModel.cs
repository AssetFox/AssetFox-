using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Models
{
    public class CalculatedAttributePagingSyncModel : PagingSyncModel<CalculatedAttributeDTO>
    {
        public CalculatedAttributePagingSyncModel() : base()
        {         
            LibraryId = null;
            AddedPairs = new Dictionary<Guid, List<CalculatedAttributeEquationCriteriaPairDTO>>();
            UpdatedPairs = new Dictionary<Guid, List<CalculatedAttributeEquationCriteriaPairDTO>>();
            DeletedPairs = new Dictionary<Guid, List<Guid>>();
            DefaultEquations = new Dictionary<Guid, CalculatedAttributeEquationCriteriaPairDTO>();
        }
        public Dictionary<Guid, List<CalculatedAttributeEquationCriteriaPairDTO>> AddedPairs { get; set; }
        public Dictionary<Guid, List<CalculatedAttributeEquationCriteriaPairDTO>> UpdatedPairs { get; set; }
        public Dictionary<Guid, List<Guid>> DeletedPairs { get; set; }
        public Dictionary<Guid, CalculatedAttributeEquationCriteriaPairDTO> DefaultEquations { get; set; }
    }
}
