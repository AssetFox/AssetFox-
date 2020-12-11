using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IDeficientConditionGoalRepository
    {
        void CreateDeficientConditionGoalLibrary(string name, Guid simulationId);
        void CreateDeficientConditionGoals(List<DeficientConditionGoal> deficientConditionGoals, Guid simulationId);
    }
}
