using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Enums;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SimulationOutputRepository : ISimulationOutputRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private const int AssetBatchSize = 400;

        public SimulationOutputRepository(UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateSimulationOutput(Guid simulationId, SimulationOutput simulationOutput)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation found for given scenario.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation.AsNoTracking()
                .Single(_ => _.Id == simulationId);

            if (simulationOutput == null)
            {
                throw new InvalidOperationException($"No results found for simulation {simulationEntity.Name}. Please ensure that the simulation analysis has been run.");
            }
            var allAttributes = _unitOfWork.AttributeRepo.GetAttributes();
            var attributeIdLookup = new Dictionary<string, Guid>();
            foreach (var attribute in allAttributes)
            {
                attributeIdLookup[attribute.Name] = attribute.Id;
            }

            try
            {
                _unitOfWork.Context.DeleteAll<SimulationOutputEntity>(_ =>
                _.SimulationId == simulationId);
                var entity = SimulationOutputMapper.ToEntityWithoutYearDetails(simulationOutput, simulationId, attributeIdLookup);
                _unitOfWork.Context.Add(entity);
                _unitOfWork.Context.SaveChanges();
                foreach (var year in simulationOutput.Years)
                {
                    var yearDetail = SimulationYearDetailMapper.ToEntityWithoutAssets(year, entity.Id, attributeIdLookup);
                    _unitOfWork.Context.Add(yearDetail);
                    var assets = year.Assets;
                    var batchedAssets = assets.ConcreteBatch(AssetBatchSize);
                    var yearSaved = false;
                    foreach (var batch in batchedAssets)
                    {
                        var mappedBatch = AssetDetailMapper.ToEntityList(batch, yearDetail.Id, attributeIdLookup);
                        _unitOfWork.Context.AddRange(mappedBatch);
                        _unitOfWork.Context.SaveChanges();
                        yearSaved = true;
                    }
                    if (!yearSaved)
                    {
                        _unitOfWork.Context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public SimulationOutput GetSimulationOutput(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"Found no simulation having id {simulationId}");
            }

            if (!_unitOfWork.Context.SimulationOutput.Any(_ => _.SimulationId == simulationId))
            {
                throw new RowNotInTableException($"No simulation analysis results were found for simulation having id {simulationId}. Please ensure that the simulation analysis has been run.");
            }

            var simulationOutputObjectCount = _unitOfWork.Context.SimulationOutput.Count(so => so.SimulationId == simulationId);
            if (simulationOutputObjectCount > 1)
            {
                throw new Exception($"Expected to find one output for the simulation. Found {simulationOutputObjectCount}."); ;
            }

            var entitiesWithoutYearContents = _unitOfWork.Context.SimulationOutput
                .Include(so => so.InitialAssetSummaries)
                .ThenInclude(a => a.AssetSummaryDetailValues)
                .Include(so => so.InitialAssetSummaries)
                .ThenInclude(a => a.MaintainableAsset)
                .Include(so => so.Years)
                .Where(_ => _.SimulationId == simulationId)
                .ToList();
            var firstEntity = entitiesWithoutYearContents[0];
            var domain = SimulationOutputMapper.ToDomain(firstEntity);
            foreach (var year in firstEntity.Years)
            {
                var yearId = year.Id;
                var loadedYearWithoutAssets = _unitOfWork.Context.SimulationYearDetail
                .Include(y => y.Assets)
                .ThenInclude(a => a.AssetDetailValues)
                .Include(y => y.Assets)
                .ThenInclude(a => a.TreatmentConsiderationDetails)
                .ThenInclude(tc => tc.CashFlowConsiderationDetails)
                .Include(y => y.Assets)
                .ThenInclude(a => a.TreatmentOptionDetails)
                .Include(y => y.Assets)
                .ThenInclude(a => a.TreatmentRejectionDetails)
                .Include(y => y.Assets)
                .ThenInclude(a => a.TreatmentSchedulingCollisionDetails)
                .Include(y => y.Budgets)
                .Include(y => y.DeficientConditionGoalDetails)
                .Include(y => y.TargetConditionGoalDetails)
                .Where(y => y.Id == yearId)
                .ToList();
                var loadedYearEntity = loadedYearWithoutAssets[0];
                var domainYear = SimulationYearDetailMapper.ToDomain(loadedYearEntity);
                domain.Years.Add(domainYear);
            }
            domain.Years.Sort((y1, y2) => y1.Year.CompareTo(y2.Year));


            return domain;

            //var simulationOutput = new SimulationOutput();
            //foreach (var item in simulationOutputObjects)
            //{
            //    switch (item.OutputType)
            //    {
            //    case SimulationOutputEnum.YearlySection:
            //        var yearlySections = JsonConvert.DeserializeObject<SimulationYearDetail>(item.Output, new JsonSerializerSettings
            //        {
            //            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            //        });
            //        simulationOutput.Years.Add(yearlySections);
            //        break;
            //    case SimulationOutputEnum.InitialConditionNetwork:
            //        simulationOutput.InitialConditionOfNetwork = Convert.ToDouble(item.Output);
            //        break;
            //    case SimulationOutputEnum.InitialSummary:
            //        var initialSummary = JsonConvert.DeserializeObject<List<AssetSummaryDetail>>(item.Output, new JsonSerializerSettings
            //        {
            //            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            //        });
            //        simulationOutput.InitialAssetSummaries.AddRange(initialSummary);
            //        break;
            //    }
            //}
            //simulationOutput.Years.Sort((a, b) => a.Year.CompareTo(b.Year));

            ////var outputData = JsonConvert.DeserializeObject<SimulationOutput>(simulationOutputString, new JsonSerializerSettings
            ////{
            ////    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            ////});

            //return simulationOutput;

            throw new NotImplementedException();
        }
    }
}
