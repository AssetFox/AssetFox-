using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;


namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics
{
    public static class LibraryUserDtolistUpdater
    {
        public static void GrantOwnerAccess(Guid userId, List<LibraryUserDTO> proposedUserList)
        {
            var userAccess = proposedUserList.SingleOrDefault(u => u.UserId == userId);
            if (userAccess == null)
            {
                var owner = new LibraryUserDTO
                {
                    AccessLevel = LibraryAccessLevel.Owner,
                    UserId = userId,
                };
                proposedUserList.Add(owner);
            } else
            {
                userAccess.AccessLevel = LibraryAccessLevel.Owner;
            }
        }
    }
}
