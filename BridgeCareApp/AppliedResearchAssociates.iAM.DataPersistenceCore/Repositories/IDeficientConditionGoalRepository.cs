using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IDeficientConditionGoalRepository
    {
        void CreateDeficientConditionGoalLibrary(string name, Guid simulationId);

        void CreateDeficientConditionGoals(List<DeficientConditionGoal> deficientConditionGoals, Guid simulationId);

        Task<List<DeficientConditionGoalLibraryDTO>> DeficientConditionGoalLibrariesWithDeficientConditionGoals();

        void UpsertDeficientConditionGoalLibrary(DeficientConditionGoalLibraryDTO dto, Guid simulationId);

        void UpsertOrDeleteDeficientConditionGoals(List<DeficientConditionGoalDTO> deficientConditionGoals, Guid libraryId);

        void DeleteDeficientConditionGoalLibrary(Guid libraryId);
    }
}
