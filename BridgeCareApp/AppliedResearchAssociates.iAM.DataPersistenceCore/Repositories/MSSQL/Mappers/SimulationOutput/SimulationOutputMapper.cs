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

        public static SimulationOutputEntity ToEntity(
            this SimulationOutput domain,
            Guid simulationId,
            Dictionary<string, Guid> attributeIdLookup)
        {
            var id = Guid.NewGuid();
            var years = new List<SimulationYearDetailEntity>();
            var summaryEntities = new List<AssetSummaryDetailEntity>();
            foreach (var domainSummary in domain.InitialAssetSummaries)
            {
                var summaryEntity = AssetSummaryDetailMapper.ToEntity(domainSummary, id, attributeIdLookup);
                summaryEntities.Add(summaryEntity);
            }
            foreach (var year in domain.Years)
            {
                var mapYear = SimulationYearDetailMapper.ToEntity(year, id, attributeIdLookup);
                years.Add(mapYear);
            }
            var entity = new SimulationOutputEntity
            {
                Id = id,
                InitialConditionOfNetwork = domain.InitialConditionOfNetwork,
                SimulationId = simulationId,
                Years = years,
                InitialAssetSummaries = summaryEntities,
            };
            return entity;
        }
    }
}
