using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class TreatmentLibraryUserDTO
    {
        public Guid UserId { get; set; }

        public bool CanModify { get; set; }

        public bool IsOwner { get; set; }
    }
}
