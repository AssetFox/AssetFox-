using System;
using System.IO;
using System.Threading.Tasks;
using BridgeCareCore.Interfaces.DefaultData;
using BridgeCareCore.Models.DefaultData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BridgeCareCore.Services.DefaultData
{
    public class AnalysisDefaultDataService : IAnalysisDefaultDataService
    {
        public async Task<AnalysisDefaultData> GetAnalysisDefaultData()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty,
               "MetaData//DefaultData", "defaultDataAttributes.json");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"{filePath} does not exist");
            }

            var content = File.ReadAllText(filePath);
            var defaultDataJson = JObject.Parse(content).SelectToken("DefaultData");
            var analysisDefaultDataJson = defaultDataJson?.SelectToken("Analysis").ToString();
            var analysisDefaultData = JsonConvert.DeserializeObject<AnalysisDefaultData>(analysisDefaultDataJson);
            return analysisDefaultData;
        }
    }
}
