using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AppliedResearchAssociates.iAM.DTOs.Abstract
{
    public abstract class BaseLibraryDTO : BaseDTO
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<Guid> AppliedScenarioIds { get; set; } = new List<Guid>();

        [Obsolete("This should go away. Instead, we will have a LibraryUserDto with an AccessLevel of Owner.")]
        public Guid Owner { get; set; }

        [Obsolete("This should go away. Instead, we have user-by-user sharing.")]
        public bool IsShared { get; set; } 
    }
}
