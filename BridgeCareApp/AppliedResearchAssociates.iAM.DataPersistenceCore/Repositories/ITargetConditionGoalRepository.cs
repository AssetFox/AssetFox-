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

        List<TargetConditionGoalLibraryDTO> TargetConditionGoalLibrariesWithTargetConditionGoals();

        void UpsertPermitted(Guid simulationId, TargetConditionGoalLibraryDTO dto);

        void UpsertTargetConditionGoalLibrary(TargetConditionGoalLibraryDTO dto, Guid simulationId);

        void UpsertOrDeleteTargetConditionGoals(List<TargetConditionGoalDTO> targetConditionGoals, Guid libraryId);

        void DeleteTargetConditionGoalLibrary(Guid libraryId);
    }
}
