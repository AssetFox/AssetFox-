using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DTOs.Enums
{
    public enum LibraryAccessLevel
    {
        Read,
        Modify, // can change data but can't delete
        Owner, // required for deletion or re-import. When creating a library, you are automatically an owner, as are admins.
        // As of October, 2022, the user who creates a library becomes an owner, and there
        // is no way to ever create new owners. But admins do have owner access.
    }
}
