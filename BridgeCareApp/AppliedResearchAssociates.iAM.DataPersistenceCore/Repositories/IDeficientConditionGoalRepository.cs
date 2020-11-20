using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IDeficientConditionGoalRepository
    {
        void CreateDeficientConditionGoalLibrary(string name, string simulationName);
        void CreateDeficientConditionGoals(List<DeficientConditionGoal> deficientConditionGoals, string simulationName);
    }
}
