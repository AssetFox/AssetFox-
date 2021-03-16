using System;
using System.IO;
using AppliedResearchAssociates.iAM.Analysis;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem
{
    public class SimulationOutputFileRepository : ISimulationOutputFileRepository
    {
        public SimulationOutput GetSimulationResults(Guid networkId, Guid simulationId)
        {
            var TempfilePath = Path.Combine(Environment.CurrentDirectory, "tempIds.json");
            var tempIds = new TempIds();
            using (FileStream fs = File.Open(TempfilePath, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string rawConnection = sr.ReadToEnd();
                    tempIds = JsonConvert
                                .DeserializeAnonymousType(rawConnection, new { SimulationIds = default(TempIds) })
                                .SimulationIds;
                }
            }
            var folderPathForNewAnalysis = $"DownloadedReports\\{tempIds.DistrictId}_NewAnalysis";
            var outputFile = $"Network 13 - Simulation {tempIds.DistrictId}.json";
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

        private class TempIds
        {
            public string DistrictId { get; set; }
        }
    }
}
