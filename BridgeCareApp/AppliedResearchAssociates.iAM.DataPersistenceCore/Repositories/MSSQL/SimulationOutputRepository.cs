using System;
using System.Data;
using System.IO;
using AppliedResearchAssociates.iAM.Analysis;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SimulationOutputRepository : MSSQLRepository, ISimulationOutputRepository
    {
        public SimulationOutputRepository(IAMContext context) : base(context) { }

        public void CreateSimulationOutput(string simulationName, SimulationOutput simulationOutput)
        {
            using (var contextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    if (!Context.Simulation.Any(_ => _.Name == simulationName))
                    {
                        throw new RowNotInTableException($"No simulation found having name {simulationName}");
                    }

                    var simulationEntity = Context.Simulation.Single(_ => _.Name == simulationName);

                    if (simulationOutput == null)
                    {
                        throw new InvalidOperationException($"No results found for simulation {simulationName}. Please ensure that the simulation analysis has been run.");
                    }

                    var settings = new Newtonsoft.Json.Converters.StringEnumConverter();
                    var simulationOutputString = JsonConvert.SerializeObject(simulationOutput, settings);

                    Context.SimulationOutput.Add(new SimulationOutputEntity
                    {
                        Id = Guid.NewGuid(), SimulationId = simulationEntity.Id, Output = simulationOutputString
                    });

                    Context.SaveChanges();

                    contextTransaction.Commit();
                }
                catch (Exception e)
                {
                    contextTransaction.Rollback();
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public void GetSimulationOutput(Simulation simulation)
        {
            if (!Context.Simulation.Any(_ => _.Name == simulation.Name))
            {
                throw new RowNotInTableException($"Found no simulation having name {simulation.Name}");
            }

            Context.SimulationOutput.Single(_ => _.Simulation.Name == simulation.Name)
                .FillSimulationResults(simulation);
        }
    }
}
