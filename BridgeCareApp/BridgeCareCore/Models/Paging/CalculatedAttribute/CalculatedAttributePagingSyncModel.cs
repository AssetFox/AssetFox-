using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Models
{
    public class CalculatedAttributePagingSyncModel
    {
        public CalculatedAttributePagingSyncModel()
        {
            LibraryId = null;
            UpdatedCalculatedAttributes = new List<CalculatedAttributeDTO>();
            AddedPairs = new Dictionary<Guid, List<CalculatedAttributeEquationCriteriaPairDTO>>();
            UpdatedPairs = new Dictionary<Guid, List<CalculatedAttributeEquationCriteriaPairDTO>>();
            DeletedPairs = new Dictionary<Guid, List<Guid>>();
            AddedCalculatedAttributes = new List<CalculatedAttributeDTO>();
        }
        public Guid? LibraryId { get; set; }
        public List<CalculatedAttributeDTO> UpdatedCalculatedAttributes { get; set; }
        public List<CalculatedAttributeDTO> AddedCalculatedAttributes { get; set; }
        public Dictionary<Guid, List<CalculatedAttributeEquationCriteriaPairDTO>> AddedPairs { get; set; }
        public Dictionary<Guid, List<CalculatedAttributeEquationCriteriaPairDTO>> UpdatedPairs { get; set; }
        public Dictionary<Guid, List<Guid>> DeletedPairs { get; set; }
        public Dictionary<Guid, CalculatedAttributeEquationCriteriaPairDTO> DefaultEquations { get; set; }
    }
}
