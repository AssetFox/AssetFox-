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

        void UpsertPermitted(UserInfoDTO userInfo, Guid simulationId, TargetConditionGoalLibraryDTO dto);

        void UpsertTargetConditionGoalLibrary(TargetConditionGoalLibraryDTO dto, Guid simulationId, UserInfoDTO userInfo);

        void UpsertOrDeleteTargetConditionGoals(List<TargetConditionGoalDTO> targetConditionGoals, Guid libraryId, UserInfoDTO userInfo);

        void DeleteTargetConditionGoalLibrary(Guid libraryId);
    }
}
