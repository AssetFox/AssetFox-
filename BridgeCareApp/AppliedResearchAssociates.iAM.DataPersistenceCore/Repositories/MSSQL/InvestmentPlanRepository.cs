using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class InvestmentPlanRepository : MSSQLRepository, IInvestmentPlanRepository
    {
        public InvestmentPlanRepository(IAMContext context) : base(context) { }

        public void CreateInvestmentPlan(InvestmentPlan domain, string simulationName)
        {
            if (!Context.Simulation.Any(_ => _.Name == simulationName))
            {
                throw new RowNotInTableException($"No simulation found having name {simulationName}");
            }

            var simulation = Context.Simulation.Single(_ => _.Name == simulationName);

            Context.InvestmentPlan.Add(domain.ToEntity(simulation.Id));
            Context.SaveChanges();
        }

        public InvestmentPlan GetSimulationInvestmentPlan(string simulationName)
        {
            if (!Context.Simulation.Any(_ => _.Name == simulationName))
            {
                throw new RowNotInTableException($"No simulation found having name {simulationName}");
            }

            return Context.InvestmentPlan.Include(_ => _.Simulation)
                .ThenInclude(_ => _.Network)
                .Single(_ => _.Simulation.Name == simulationName)
                .ToDomain();
        }
    }
}
