using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
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
