﻿namespace AppliedResearchAssociates.iAM.DTOs
{
    public class UserInfoDTO
    {
        public string Sub { get; set; }

        public string Roles { get; set; }        

        public string Email { get; set; }

        public bool HasAdminClaim { get; set; }

        public string InternalRole { get; set; }
    }
}
