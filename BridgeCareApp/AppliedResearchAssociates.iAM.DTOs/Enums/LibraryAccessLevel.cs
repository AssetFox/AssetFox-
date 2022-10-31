using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DTOs.Enums
{
    /// <summary>Enum is arranged in order of increasing access. The code depends on this.</summary>
    public enum LibraryAccessLevel
    {
        Read,
        Modify, // can change data but can't delete
        Owner, // required for deletion or re-import. When creating a library, you are automatically an owner, as are admins.
        // As of October, 2022, the user who creates a library becomes an owner, and there
        // is no way to ever create new owners. But admins do have owner access.
    }
}
