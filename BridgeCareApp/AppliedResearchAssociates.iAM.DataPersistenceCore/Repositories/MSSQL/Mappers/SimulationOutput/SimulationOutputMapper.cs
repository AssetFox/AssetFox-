using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Analysis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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

        public static SimulationOutputEntity ToEntity(this SimulationOutput domain, Guid simulationId)
        {
            var id = Guid.NewGuid();
            var attributeLookup = new Dictionary<string, Guid>();
            var years = new List<SimulationYearDetailEntity>();
            foreach (var year in domain.Years)
            {
                var mapYear = SimulationOutputYearMapper.ToEntity(year, id, attributeLookup);
                years.Add(mapYear);
            }
            var entity = new SimulationOutputEntity
            {
                Id = id,
                InitialConditionOfNetwork = domain.InitialConditionOfNetwork,
                SimulationId = simulationId,
                
            };
            return entity;
        }
    }
}
