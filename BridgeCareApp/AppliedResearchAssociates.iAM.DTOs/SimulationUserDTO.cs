using System;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class SimulationUserDTO
    {
        public Guid UserId { get; set; }

        public string Username { get; set; }

        public bool CanModify { get; set; }

        public bool IsOwner { get; set; }
    }
}
