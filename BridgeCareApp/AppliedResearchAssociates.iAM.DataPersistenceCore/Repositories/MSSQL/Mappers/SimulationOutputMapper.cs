using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Analysis;
using Newtonsoft.Json;
using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class SimulationOutputMapper
    {
        public static void FillSimulationResults(this SimulationOutputEntity entity, Simulation simulation)
        {
            throw new NotImplementedException();
            //var simulationOutputObject = JsonConvert.DeserializeObject<SimulationOutput>(entity.Output, new JsonSerializerSettings
            //{
            //    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            //});

            //simulation.ClearResults();

            //simulation.Results.InitialConditionOfNetwork = simulationOutputObject.InitialConditionOfNetwork;
            //simulation.Results.InitialAssetSummaries.AddRange(simulationOutputObject.InitialAssetSummaries);
            //simulation.Results.Years.AddRange(simulationOutputObject.Years);
        }
    }
}
