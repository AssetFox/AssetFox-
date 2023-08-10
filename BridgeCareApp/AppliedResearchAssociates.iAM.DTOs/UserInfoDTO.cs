using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class UserInfoDTO
    {
        public string Sub { get; set; }

        public string Roles { get; set; }        

        public string Email { get; set; }

        public bool HasAdminAccess { get; set; }

        public bool HasSimulationAccess { get; set; }
    }
}
