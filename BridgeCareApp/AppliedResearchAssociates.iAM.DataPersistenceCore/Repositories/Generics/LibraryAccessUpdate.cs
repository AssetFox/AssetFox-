using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repsitories;
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

        public static void AdjustAccessForUpdate(LibraryAccessModel currentAccessModel, List<LibraryUserDTO> proposedUserList)
        {
            foreach (var user in currentAccessModel.Users)
            {
                if (user.AccessLevel == LibraryAccessLevel.Owner)
                {
                    GrantOwnerAccess(user.UserId, proposedUserList);
                }
            }
            foreach (var proposedUser in proposedUserList)
            {
                if (proposedUser.AccessLevel == LibraryAccessLevel.Owner)
                {
                    if (!currentAccessModel.HasAccess(proposedUser.UserId, LibraryAccessLevel.Owner)) {
                        proposedUser.AccessLevel = LibraryAccessLevel.Modify;
                    };
                }
            }
        }
    }
}
