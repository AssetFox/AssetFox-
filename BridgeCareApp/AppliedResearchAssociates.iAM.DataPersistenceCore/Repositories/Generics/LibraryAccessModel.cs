using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repsitories
{
    public class LibraryAccessModel
    {
        public bool LibraryExists { get; set; }

        public Guid UserId { get; set; }
        public List<LibraryUserDTO> Users { get; set; }
    }

    public static class LibraryAccessModels
    {
        public static LibraryAccessModel LibraryDoesNotExist() => new LibraryAccessModel
        {
            LibraryExists = false,
        };

        /// <summary>The list should typically have length zero or one. In other words,
        /// either there is access information for the user, or their isn't.</summary>
        public static LibraryAccessModel LibraryExistsWithUsers(Guid userId, List<LibraryUserDTO> users)
        {
            var model = new LibraryAccessModel
            {
                Users = users,
                UserId = userId,
                LibraryExists = true,
            };
            return model;
        }
    }
}
