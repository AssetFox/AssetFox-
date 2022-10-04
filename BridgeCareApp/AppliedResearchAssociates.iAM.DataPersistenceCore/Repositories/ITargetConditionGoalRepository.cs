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

        void UpsertTargetConditionGoalLibrary(TargetConditionGoalLibraryDTO dto);

        void UpsertOrDeleteTargetConditionGoals(List<TargetConditionGoalDTO> targetConditionGoals, Guid libraryId);

        void DeleteTargetConditionGoalLibrary(Guid libraryId);

        List<TargetConditionGoalDTO> GetScenarioTargetConditionGoals(Guid simulationId);
        List<TargetConditionGoalDTO> GetTargetConditionGoalsByLibraryId(Guid libraryId);

        void UpsertOrDeleteScenarioTargetConditionGoals(List<TargetConditionGoalDTO> scenarioTargetConditionGoal, Guid simulationId);
    }
}
