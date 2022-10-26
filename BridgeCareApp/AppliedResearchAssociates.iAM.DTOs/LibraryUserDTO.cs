using System;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class LibraryUserDTO
    {
        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public LibraryAccessLevel AccessLevel { get; set; }
    }
}
