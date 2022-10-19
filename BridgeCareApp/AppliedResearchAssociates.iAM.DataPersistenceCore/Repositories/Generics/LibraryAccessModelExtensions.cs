using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public static class LibraryAccessModelExtensions
    {
        /// <summary>This method will not notice if the user is an admin. It checks only whether or not
        /// the accessModel knows that the user has the access.</summary>
        public static bool HasAccess(this LibraryAccessModel accessModel, Guid userId, LibraryAccessLevel minimumAccessLevel)
        {
            if (!accessModel.LibraryExists)
            {
                throw new InvalidOperationException("Can't check access of nonexistent library.");
            }
            var user = accessModel.Users.FirstOrDefault(u => u.UserId == userId);
            var authorized = user != null && user.AccessLevel >= minimumAccessLevel;
            return authorized;
        }

        /// <summary>This method will not notice if the user is an admin. It checks only whether or not
        /// the accessModel knows that the user is authorized.</summary>
        public static bool Unauthorized(this LibraryAccessModel accessModel, Guid userId, LibraryAccessLevel minimumAccessLevel)
        {
            var hasAccess = accessModel.HasAccess(userId, minimumAccessLevel);
            return !hasAccess;
        }
    }
}
