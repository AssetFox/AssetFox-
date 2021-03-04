using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class DeficientConditionGoalLibraryDTO : BaseLibraryDTO
    {
        public List<DeficientConditionGoalDTO> DeficientConditionGoals { get; set; }
    }
}
