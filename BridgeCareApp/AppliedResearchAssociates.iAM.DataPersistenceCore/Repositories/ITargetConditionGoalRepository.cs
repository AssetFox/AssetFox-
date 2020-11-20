using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ITargetConditionGoalRepository
    {
        void CreateTargetConditionGoalLibrary(string name, string simulationName);
        void CreateTargetConditionGoals(List<TargetConditionGoal> targetConditionGoals, string simulationName);
    }
}
