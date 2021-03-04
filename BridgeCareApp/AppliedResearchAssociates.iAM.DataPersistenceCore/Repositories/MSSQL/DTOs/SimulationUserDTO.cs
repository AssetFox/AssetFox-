namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class SimulationUserDTO : CrudDTO
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public bool CanModify { get; set; }
    }
}
