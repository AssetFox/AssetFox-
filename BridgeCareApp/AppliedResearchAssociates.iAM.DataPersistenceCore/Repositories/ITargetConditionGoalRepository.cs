using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ITargetConditionGoalRepository
    {
        void CreateTargetConditionGoalLibrary(string name, Guid simulationId);
        void CreateTargetConditionGoals(List<TargetConditionGoal> targetConditionGoals, Guid simulationId);
        Task<List<TargetConditionGoalLibraryDTO>> TargetConditionGoalLibrariesWithTargetConditionGoals();
        void AddOrUpdateTargetConditionGoalLibrary(TargetConditionGoalLibraryDTO dto, Guid simulationId);
        void AddOrUpdateOrDeleteTargetConditionGoals(List<TargetConditionGoalDTO> remainingLifeLimits, Guid libraryId);
        void DeleteTargetConditionGoalLibrary(Guid libraryId);
    }
}
