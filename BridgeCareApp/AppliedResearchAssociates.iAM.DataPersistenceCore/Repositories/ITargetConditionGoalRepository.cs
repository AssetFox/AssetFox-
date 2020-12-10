using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ITargetConditionGoalRepository
    {
        void CreateTargetConditionGoalLibrary(string name, Guid simulationId);
        void CreateTargetConditionGoals(List<TargetConditionGoal> targetConditionGoals, Guid simulationId);
    }
}
