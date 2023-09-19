﻿using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ITargetConditionGoalRepository
    {
        void CreateTargetConditionGoals(List<TargetConditionGoal> targetConditionGoals, Guid simulationId);

        List<TargetConditionGoalLibraryDTO> GetTargetConditionGoalLibrariesWithTargetConditionGoals();

        List<TargetConditionGoalLibraryDTO> GetTargetConditionGoalLibrariesNoChildren();

        DateTime GetLibraryModifiedDate(Guid targetLibraryId);

        void UpsertTargetConditionGoalLibrary(TargetConditionGoalLibraryDTO dto);

        void UpsertOrDeleteTargetConditionGoals(List<TargetConditionGoalDTO> targetConditionGoals, Guid libraryId);

        void DeleteTargetConditionGoalLibrary(Guid libraryId);

        List<TargetConditionGoalDTO> GetScenarioTargetConditionGoals(Guid simulationId);
        List<TargetConditionGoalDTO> GetTargetConditionGoalsByLibraryId(Guid libraryId);

        void UpsertOrDeleteScenarioTargetConditionGoals(List<TargetConditionGoalDTO> scenarioTargetConditionGoal, Guid simulationId);

        List<TargetConditionGoalLibraryDTO> GetTargetConditionGoalLibrariesNoChildrenAccessibleToUser(Guid userId);

        LibraryUserAccessModel GetLibraryAccess(Guid libraryId, Guid userId);

        void UpsertOrDeleteUsers(Guid targetConditionGoalLibraryId, IList<LibraryUserDTO> libraryUsers);

        List<LibraryUserDTO> GetLibraryUsers(Guid targetConditionGoalLibraryId);

        void AddLibraryIdToScenarioTargetConditionGoal(List<TargetConditionGoalDTO> targetConditionGoalDTOs, Guid? libraryId);

        void AddModifiedToScenarioTargetConditionGoal(List<TargetConditionGoalDTO> targetConditionGoalDTOs, bool IsModified);
        void UpsertTargetConditionGoalLibraryGoalsAndPossiblyUser(TargetConditionGoalLibraryDTO dto, bool isNewLibrary, Guid ownerIdForNewLibrary);
    }
}
