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
    public class SimulationRepository : MSSQLRepository, ISimulationRepository
    {
        public SimulationRepository(IAMContext context) : base(context) { }

        public int CreateSimulations(List<Simulation> simulations, Guid networkId)
        {
            var entities = simulations.Select(_ => _.ToEntity(networkId)).ToList();

            if (!entities.Any())
            {
                return 0;
            }

            Context.Simulation.AddRange(entities);
            Context.SaveChanges();

            return entities.Count();
        }

        public IEnumerable<Simulation> GetAllInNetwork(Guid networkId)
        {
            if (!Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var simulations = Context.Simulation.Include(_ => _.Network)
                .Include(_ => _.AnalysisMethod)
                .Include(_ => _.InvestmentPlan)
                .Include(_ => _.PerformanceCurveLibrarySimulationJoin)
                .ThenInclude(_ => _.PerformanceCurveLibrary)
                .ThenInclude(_ => _.PerformanceCurves)
                .Where(_ => _.NetworkId == networkId)
                .ToList();

            return simulations.Select(_ => _.ToDomain());
        }
    }
}
