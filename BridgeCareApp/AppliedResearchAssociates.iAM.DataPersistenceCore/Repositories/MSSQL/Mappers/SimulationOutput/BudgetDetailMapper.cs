using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class BudgetDetailMapper
    {
        public static BudgetDetailEntity ToEntity(BudgetDetail budget, Guid simulationYearDetailId)
        {
            var entity = new BudgetDetailEntity
            {
                Id = Guid.NewGuid(),
                SimulationYearDetailId = simulationYearDetailId,
                BudgetName = budget.BudgetName,
                AvailableFunding = budget.AvailableFunding,
            };
            return entity;
        }

        public static List<BudgetDetailEntity> ToEntityList(List<BudgetDetail> domainList, Guid id)
        {
            var entities = new List<BudgetDetailEntity>();
            foreach (var budget in domainList)
            {
                var mapBudget = ToEntity(budget, id);
                entities.Add(mapBudget);
            }
            return entities;
        }
    }
}
