using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class CalculatedAttributeLibraryDTO : BaseLibraryDTO
    {
        public bool IsDefault { get; set; } = false;

        public ICollection<CalculatedAttributeDTO> CalculatedAttributes { get; set; } = new List<CalculatedAttributeDTO>();
    }
}
