using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore;

public class LibraryAccessModel
{
    public bool LibraryExists { get; set; }

    public Guid UserId { get; set; }
    /// <summary> always has length 0 or 1, depending on whether or not the user has access. </summary>
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
