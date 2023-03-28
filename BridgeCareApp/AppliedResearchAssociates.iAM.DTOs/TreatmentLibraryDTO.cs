using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class TreatmentLibraryDTO : BaseLibraryDTO
    {
        public List<TreatmentDTO> Treatments { get; set; }

        public bool IsModified { get; set; }
    }
}
