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

        public void CreateSimulationOutput(Guid simulationId, SimulationOutput simulationOutput)
        {
            using (var contextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    if (!Context.Simulation.Any(_ => _.Id == simulationId))
                    {
                        throw new RowNotInTableException($"No simulation found having id {simulationId}");
                    }

                    var simulationEntity = Context.Simulation.Single(_ => _.Id == simulationId);

                    if (simulationOutput == null)
                    {
                        throw new InvalidOperationException($"No results found for simulation {simulationEntity.Name}. Please ensure that the simulation analysis has been run.");
                    }

                    var settings = new Newtonsoft.Json.Converters.StringEnumConverter();
                    var simulationOutputString = JsonConvert.SerializeObject(simulationOutput, settings);

                    Context.SimulationOutput.Add(new SimulationOutputEntity
                    {
                        SimulationId = simulationId, Output = simulationOutputString
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
            if (!Context.Simulation.Any(_ => _.Id == simulation.Id))
            {
                throw new RowNotInTableException($"Found no simulation having id {simulation.Id}");
            }

            if (Context.SimulationOutput.Any(_ => _.SimulationId == simulation.Id))
            {
                Context.SimulationOutput.Single(_ => _.SimulationId == simulation.Id)
                    .FillSimulationResults(simulation);
            }
        }
    }
}
