using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore;

public class LibraryUserAccessModel
{
    public bool LibraryExists { get; set; }

    public Guid UserId { get; set; }

    public LibraryUserDTO Access { get; set; } 
}

public static class LibraryAccessModels
{
    public static LibraryUserAccessModel LibraryDoesNotExist() => new LibraryUserAccessModel
    {
        LibraryExists = false,
    };

    public static LibraryUserAccessModel LibraryExistsWithUsers(Guid userId, LibraryUserDTO user)
    {
        var model = new LibraryUserAccessModel
        {
            Access = user,
            UserId = userId,
            LibraryExists = true,
        };
        return model;
    }
}
