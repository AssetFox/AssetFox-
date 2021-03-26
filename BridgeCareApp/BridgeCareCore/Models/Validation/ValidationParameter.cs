using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;

namespace BridgeCareCore.Models
{
    public class ValidationParameter
    {
        public string Expression { get; set; }

        public UserCriteriaDTO CurrentUserCriteriaFilter { get; set; }
    }
}
