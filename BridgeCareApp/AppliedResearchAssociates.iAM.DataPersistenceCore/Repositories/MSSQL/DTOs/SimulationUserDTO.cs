using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class SimulationUserDTO
    {
        public Guid UserId { get; set; }

        public string Username { get; set; }

        public bool CanModify { get; set; }

        public bool IsOwner { get; set; }
    }
}
