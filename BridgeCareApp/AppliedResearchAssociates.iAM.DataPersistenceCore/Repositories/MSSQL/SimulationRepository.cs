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

        public void CreateSimulation(Simulation simulation)
        {
            if (!Context.Network.Any(_ => _.Name == simulation.Network.Name))
            {
                throw new RowNotInTableException($"No network found having name {simulation.Network.Name}");
            }

            var network = Context.Network.Single(_ => _.Name == simulation.Network.Name);

            Context.Simulation.Add(simulation.ToEntity(network.Id));
            Context.SaveChanges();
        }

        public List<Simulation> GetAllInNetwork(string networkName)
        {
            if (!Context.Network.Any(_ => _.Name == networkName))
            {
                throw new RowNotInTableException($"No network found having name {networkName}");
            }

            var simulations = Context.Simulation
                .Include(_ => _.Network)
                .Include(_ => _.AnalysisMethod)
                .Include(_ => _.InvestmentPlanSimulationJoin)
                .ThenInclude(_ => _.InvestmentPlan)
                .Include(_ => _.PerformanceCurveLibrarySimulationJoin)
                .ThenInclude(_ => _.PerformanceCurveLibrary)
                .ThenInclude(_ => _.PerformanceCurves)
                .Where(_ => _.Network.Name == networkName)
                .ToList();

            return simulations.Select(_ => _.ToDomain()).ToList();
        }
    }
}
