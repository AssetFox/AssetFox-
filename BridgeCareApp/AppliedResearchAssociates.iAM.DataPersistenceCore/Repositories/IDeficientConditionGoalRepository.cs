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
        List<DeficientConditionGoalLibraryDTO> GetDeficientConditionGoalLibrariesNoChildren();

        void UpsertDeficientConditionGoalLibrary(DeficientConditionGoalLibraryDTO dto);

        void UpsertOrDeleteDeficientConditionGoals(List<DeficientConditionGoalDTO> deficientConditionGoals, Guid libraryId);

        void DeleteDeficientConditionGoalLibrary(Guid libraryId);

        List<DeficientConditionGoalDTO> GetScenarioDeficientConditionGoals(Guid simulationId);

        List<DeficientConditionGoalDTO> GetDeficientConditionGoalsByLibraryId(Guid libraryId);

        void UpsertOrDeleteScenarioDeficientConditionGoals(List<DeficientConditionGoalDTO> scenarioDeficientConditionGoal, Guid simulationId);

        List<DeficientConditionGoalLibraryDTO> GetDeficientConditionGoalLibrariesNoChildrenAccessibleToUser(Guid userId);

        LibraryUserAccessModel GetLibraryAccess(Guid libraryId, Guid userId);

        void UpsertOrDeleteUsers(Guid deficientConditionGoalLibraryId, IList<LibraryUserDTO> libraryUsers);

        List<LibraryUserDTO> GetLibraryUsers(Guid deficientConditionGoalLibraryId);
    }
}
