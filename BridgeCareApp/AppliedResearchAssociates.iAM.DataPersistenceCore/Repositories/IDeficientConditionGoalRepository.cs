using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IDeficientConditionGoalRepository
    {
        void CreateDeficientConditionGoalLibrary(string name, Guid simulationId);

        void CreateDeficientConditionGoals(List<DeficientConditionGoal> deficientConditionGoals, Guid simulationId);

        List<DeficientConditionGoalLibraryDTO> DeficientConditionGoalLibrariesWithDeficientConditionGoals();

        void UpsertPermitted(Guid simulationId, DeficientConditionGoalLibraryDTO dto);

        void UpsertDeficientConditionGoalLibrary(DeficientConditionGoalLibraryDTO dto, Guid simulationId);

        void UpsertOrDeleteDeficientConditionGoals(List<DeficientConditionGoalDTO> deficientConditionGoals, Guid libraryId);

        void DeleteDeficientConditionGoalLibrary(Guid libraryId);
    }
}
