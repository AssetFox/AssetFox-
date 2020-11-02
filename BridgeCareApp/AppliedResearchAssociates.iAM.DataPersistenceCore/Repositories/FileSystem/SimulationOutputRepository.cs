using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AppliedResearchAssociates.iAM.Analysis;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem
{
    public class SimulationOutputRepository : ISimulationOutputRepository
    {
        public SimulationOutput GetSimulationResults(Guid networkId, Guid simulationId)
        {
            var folderPathForNewAnalysis = $"DownloadedReports\\{simulationId}_NewAnalysis";
            var outputFile = $"Network {networkId} - Simulation {simulationId}.json";
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderPathForNewAnalysis, outputFile);
            // check that the file exists
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"{filePath} does not exist");
            }
            var simulationOutput = new SimulationOutput();
            using (StreamReader reader = new StreamReader(filePath))
            {
                var rawResult = reader.ReadToEnd();
                simulationOutput = JsonConvert.DeserializeObject<SimulationOutput>(rawResult);
            }
            return simulationOutput;
        }
    }
}
