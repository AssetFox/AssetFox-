using System;
using System.Data;
using System.IO;
using AppliedResearchAssociates.iAM.Analysis;
using System.Linq;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SimulationOutputRepository : MSSQLRepository, ISimulationOutputRepository
    {
        public SimulationOutputRepository(IAMContext context) : base(context) { }

        public void CreateSimulationOutput(string fileName, string simulationName)
        {
            using (var contextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    if (!Context.Simulation.Any(_ => _.Name == simulationName))
                    {
                        throw new RowNotInTableException($"No simulation found having name {simulationName}");
                    }

                    var simulationOutputFileData = System.IO.File.ReadAllBytes(fileName);

                    if (!simulationOutputFileData.Any())
                    {
                        throw new FileNotFoundException();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
            }
        }

        

        public SimulationOutput GetSimulationResults(Guid networkId, Guid simulationId)
        {
            var folderPathForNewAnalysis = $"DownloadedReports\\1189_NewAnalysis";
            var outputFile = $"Network 13 - Simulation 1189.json";
            var filePath = Path.Combine(Environment.CurrentDirectory, folderPathForNewAnalysis, outputFile);
            // check that the file exists
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"{filePath} does not exist");
            }
            var simulationOutput = new SimulationOutput();
            using (StreamReader reader = new StreamReader(filePath))
            {
                var rawResult = reader.ReadToEnd();
                simulationOutput = JsonConvert.DeserializeObject<SimulationOutput>(rawResult, new JsonSerializerSettings
                {
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                });
            }
            return simulationOutput;
        }
    }
}
