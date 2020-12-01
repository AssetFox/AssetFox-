using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IYearlyInvestmentRepository
    {
        public Dictionary<string, Budget> GetYearlyBudgetAmount(Guid simulationId, int firstYearOfAnalysisPeriod, int numberOfYears);
    }
}
