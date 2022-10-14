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
        public List<LibraryUserDTO> Users { get; set; }
    }

    public static class LibraryAccessModels
    {
        public static LibraryAccessModel DoesNotExist() => new LibraryAccessModel
        {
            LibraryExists = false,
        };

        public static LibraryAccessModel ExistsWithUsers(List<LibraryUserDTO> users)
        {
            var model = new LibraryAccessModel
            {
                Users = users,
                LibraryExists = true,
            };
            return model;
        }
    }
}
