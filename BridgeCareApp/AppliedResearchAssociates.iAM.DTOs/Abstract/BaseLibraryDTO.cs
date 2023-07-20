using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AppliedResearchAssociates.iAM.DTOs.Abstract
{
    /// <summary>
    /// A base library DTO class that all libraries derive from.
    /// </summary>
    public abstract class BaseLibraryDTO : BaseDTO
    {
        /// <summary>
        /// The name of the library.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A description of the library.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A list of the IDs of all scenarios applied to the library.
        /// </summary>
        public List<Guid> AppliedScenarioIds { get; set; } = new List<Guid>();

        /// <summary>
        /// The ID of the user who owns the library.
        /// </summary>
        [Obsolete("This should go away. Instead, we will have a LibraryUserDto with an AccessLevel of Owner.")]
        public Guid Owner { get; set; }

        /// <summary>
        /// Verifies the library can be shared between users.
        /// </summary>
        [Obsolete("This should go away. Instead, we have user-by-user sharing.")]
        public bool IsShared { get; set; } 
    }
}
