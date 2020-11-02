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
            var folderPathForNewAnalysis = $"DownloadedReports\\1191_NewAnalysis";
            var outputFile = $"Network 13 - Simulation 1191.json";
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
                //dynamic test = JsonConvert.DeserializeObject(rawResult);
                //simulationOutput.InitialConditionOfNetwork = test.InitialConditionOfNetwork;
                //foreach(var item in test.InitialSectionSummaries)
                //{
                //    var exp = new Explorer();
                //    var net = new Network(exp);
                //    var fac = new Facility(net);
                //    var sec = new Section(fac);
                //    fac.Name = item.FacilityName;
                //    sec.Area = item.Area;
                //    sec.Name = item.SectionName;

                //    var initialSection = new SectionSummaryDetail(sec);
                //    var numericAttributes = JsonConvert.DeserializeObject<Dictionary<string, double>>(item.ValuePerNumericAttribute);
                //    foreach (var numAtt in numericAttributes)
                //    {
                //        _ = initialSection.ValuePerNumericAttribute.Add(numAtt.Key, numAtt.Value);
                //    }
                //    var textAttributes = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.ValuePerTextAttribute);
                //    foreach (var textAtt in textAttributes)
                //    {
                //        _ = initialSection.ValuePerTextAttribute.Add(textAtt.Key, textAtt.Value);
                //    }
                //    simulationOutput.InitialSectionSummaries.Add(initialSection);
                //}
                //foreach (var data in test.Years)
                //{
                //    var simulationYearDetail = new SimulationYearDetail(data.Year)
                //    {
                //        ConditionOfNetwork = data.ConditionOfNetwork
                //    };
                //    foreach (var budget in data.Budgets)
                //    {
                //        var bud = new Budget();
                //        bud.Name = budget.BudgetName;
                //        var budgetDetail = new BudgetDetail(bud, budget.AvailableFunding);
                //    }
                //}

                var exp = new Explorer();
                var net = new Network(exp);
                var fac = new Facility(net);
                var sec = new Section(fac);

                var initialSection = new SectionSummaryDetail(sec);
                simulationOutput = JsonConvert.DeserializeObject<SimulationOutput>(rawResult, new JsonSerializerSettings
                {
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                });
            }
            return simulationOutput;
        }
    }
}
