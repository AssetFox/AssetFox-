using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics
{
    public static class LibraryUserDtolists
    {
        public static List<LibraryUserDTO> OwnerAccess(Guid userId)
        {

            var owner = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Owner,
                UserId = userId,
            };
            var userList = new List<LibraryUserDTO> { owner };
            return userList;
        }
    }
}
