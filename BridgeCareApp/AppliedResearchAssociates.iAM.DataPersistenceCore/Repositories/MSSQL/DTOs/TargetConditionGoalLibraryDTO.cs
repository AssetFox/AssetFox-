﻿using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class TargetConditionGoalLibraryDTO : BaseLibraryDTO
    {
        public List<TargetConditionGoalDTO> TargetConditionGoals { get; set; }
    }
}