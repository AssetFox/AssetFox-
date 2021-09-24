using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IDeficientConditionGoalRepository
    {
        void CreateDeficientConditionGoals(List<DeficientConditionGoal> deficientConditionGoals, Guid simulationId);

        List<DeficientConditionGoalLibraryDTO> GetDeficientConditionGoalLibrariesWithDeficientConditionGoals();

        void UpsertDeficientConditionGoalLibrary(DeficientConditionGoalLibraryDTO dto);

        void UpsertOrDeleteDeficientConditionGoals(List<DeficientConditionGoalDTO> deficientConditionGoals, Guid libraryId);

        void DeleteDeficientConditionGoalLibrary(Guid libraryId);

        List<DeficientConditionGoalDTO> GetScenarioDeficientConditionGoals(Guid simulationId);

        void UpsertOrDeleteScenarioDeficientConditionGoals(List<DeficientConditionGoalDTO> scenarioDeficientConditionGoal, Guid simulationId);
    }
}
