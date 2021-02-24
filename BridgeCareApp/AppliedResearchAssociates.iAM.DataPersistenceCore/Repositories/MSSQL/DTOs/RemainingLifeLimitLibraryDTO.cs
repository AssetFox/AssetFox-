using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class RemainingLifeLimitLibraryDTO : BaseLibraryDTO
    {
        public List<RemainingLifeLimitDTO> RemainingLifeLimits { get; set; } = new List<RemainingLifeLimitDTO>();
    }
}
