using System;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// Serialization-friendly aggregate of values of a user, related to a simulation.
    /// </summary>
    public class SimulationUserDTO
    {
        /// <summary>
        /// The ID of the user.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The name of the user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Verifies the user can change a library.
        /// </summary>
        public bool CanModify { get; set; }

        /// <summary>
        /// Verifies the user owns a library.
        /// </summary>
        public bool IsOwner { get; set; }
    }
}
