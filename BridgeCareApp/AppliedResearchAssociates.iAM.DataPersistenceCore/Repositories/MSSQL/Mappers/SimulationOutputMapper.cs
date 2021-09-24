using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Analysis;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class SimulationOutputMapper
    {
        public static void FillSimulationResults(this SimulationOutputEntity entity, Simulation simulation)
        {
            var simulationOutputObject = JsonConvert.DeserializeObject<SimulationOutput>(entity.Output, new JsonSerializerSettings
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            });

            simulation.ClearResults();

            simulation.Results.InitialConditionOfNetwork = simulationOutputObject.InitialConditionOfNetwork;
            simulation.Results.InitialSectionSummaries.AddRange(simulationOutputObject.InitialSectionSummaries);
            simulation.Results.Years.AddRange(simulationOutputObject.Years);
        }
    }
}
