using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class UserDTO : BaseDTO
    {
        public string Username { get; set; }

        public bool HasInventoryAccess { get; set; }

        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
