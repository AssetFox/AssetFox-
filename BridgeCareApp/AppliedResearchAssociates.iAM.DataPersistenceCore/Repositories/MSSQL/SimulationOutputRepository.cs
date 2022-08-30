using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Enums;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Debugging;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SimulationOutputRepository : ISimulationOutputRepository
    {
        private const bool ShouldHackSaveOutputToFile = false;
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        public const int AssetSaveBatchSize = 300;
        public const int AssetSummarySaveBatchSize = 2000;
        public const int AssetLoadBatchSize = 400;
        public const int AssetSummaryLoadBatchSize = 400;

        public SimulationOutputRepository(UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateSimulationOutput(Guid simulationId, SimulationOutput simulationOutput)
        {
            if (ShouldHackSaveOutputToFile)
            {
#pragma warning disable CS0162 // Unreachable code detected
                HackSaveOutputToFile(simulationOutput);
#pragma warning restore CS0162 // Unreachable code detected
            }
            var memos = EventMemoModelLists.GetFreshInstance("Save");
            memos.Mark("Starting save");
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
                var entity = SimulationOutputMapper.ToEntityWithoutAssetsOrYearDetails(simulationOutput, simulationId, attributeIdLookup);
                _unitOfWork.Context.Add(entity);
                _unitOfWork.Context.SaveChanges();
                var assetSummaries = simulationOutput.InitialAssetSummaries;
                var family = AssetSummaryDetailMapper.ToEntityLists(assetSummaries, entity.Id, attributeIdLookup);
                _unitOfWork.Context.AddAll(family.AssetSummaryDetails);
                _unitOfWork.Context.AddAll(family.AssetSummaryDetailValues);
                foreach (var year in simulationOutput.Years)
                {
                    memos.Mark($"Y{ year.Year}");
                    var yearDetail = SimulationYearDetailMapper.ToEntityWithoutAssets(year, entity.Id, attributeIdLookup);
                    _unitOfWork.Context.Add(yearDetail);
                    var assets = year.Assets;
                    var batchedAssets = assets.ConcreteBatch(AssetSaveBatchSize);
                    var yearSaved = false;
                    int batchIndex = 0;
                    foreach (var batch in batchedAssets)
                    {
                        var mappedBatch = AssetDetailMapper.ToEntityList(batch, yearDetail.Id, attributeIdLookup);
                        _unitOfWork.Context.AddRange(mappedBatch);
                        _unitOfWork.Context.SaveChanges();
                        yearSaved = true;
                        memos.Mark($" b{batchIndex}");
                        batchIndex++;
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
            finally
            {
                var timings = memos.ToMultilineString(true);
                System.Diagnostics.Debug.WriteLine(timings);
                var directory = Directory.GetCurrentDirectory();
                var outputTimingsPath = Path.Combine(directory, "SaveTimings.txt");
                File.WriteAllText(outputTimingsPath, timings);
            }
        }

        private static void HackSaveOutputToFile(SimulationOutput simulationOutput)
        {
            var directory = Directory.GetCurrentDirectory();
            var path = Path.Combine(directory, "SimulationOutput.json");
            var serializedOutput = JsonConvert.SerializeObject(simulationOutput);
            File.Delete(path);
            File.WriteAllText(path, serializedOutput);
        }

        public SimulationOutput GetSimulationOutput(Guid simulationId)
        {
            var memos = EventMemoModelLists.GetFreshInstance("Load");
            memos.Mark("Starting load");
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
            var allAttributes = _unitOfWork.AttributeRepo.GetAttributes();
            var attributeNameLookup = new Dictionary<Guid, string>();
            foreach (var attribute in allAttributes)
            {
                attributeNameLookup[attribute.Id] = attribute.Name;
            }
            var entitiesWithoutAssetSummariesOrYearContents = _unitOfWork.Context.SimulationOutput
                .Include(so => so.Years)
                .Where(_ => _.SimulationId == simulationId)
                .ToList();
            var firstEntity = entitiesWithoutAssetSummariesOrYearContents[0];
            var simulationOutputId = firstEntity.Id;
            var cacheYears = firstEntity.Years.OrderBy(y => y.Year).ToList();
            firstEntity.Years.Clear();
            var domain = SimulationOutputMapper.ToDomain(firstEntity, attributeNameLookup);
            var shouldContinueLoadingAssetSummaries = true;
            int summaryBatchIndex = 0;
            while (shouldContinueLoadingAssetSummaries)
            {
                var assetSummaries = _unitOfWork.Context.AssetSummaryDetail
                .Include(a => a.AssetSummaryDetailValues)
                .Include(a => a.MaintainableAsset)
                .OrderBy(a => a.Id)
                .Skip(summaryBatchIndex * AssetSummaryLoadBatchSize)
                .Take(AssetSummaryLoadBatchSize)
                .Where(a => a.SimulationOutputId == simulationOutputId)
                .ToList();
                var assetSummaryDomainList = AssetSummaryDetailMapper.ToDomainListNullSafe(assetSummaries);
                domain.InitialAssetSummaries.AddRange(assetSummaryDomainList);
                shouldContinueLoadingAssetSummaries = assetSummaries.Count() == AssetSummaryLoadBatchSize;
                summaryBatchIndex++;
            }
            foreach (var cacheYear in cacheYears)
            {
                memos.Mark($"Y{cacheYear.Year}");
                var yearId = cacheYear.Id;
                var year = cacheYear.Year;
                var loadedYearWithoutAssets = _unitOfWork.Context.SimulationYearDetail
                .Include(y => y.Budgets)
                .Include(y => y.DeficientConditionGoalDetails)
                .Include(y => y.TargetConditionGoalDetails)
                .Where(y => y.Id == yearId)
                .ToList();
                var loadedYearEntity = loadedYearWithoutAssets[0];
                var domainYear = SimulationYearDetailMapper.ToDomainWithoutAssets(loadedYearEntity, attributeNameLookup);
                domain.Years.Add(domainYear);
                bool shouldContinueLoadingAssets = true;
                var batchIndex = 0;
                while (shouldContinueLoadingAssets)
                {
                    var assetEntities = _unitOfWork.Context.AssetDetail
                           .Where(a => a.SimulationYearDetailId == yearId)
                           .OrderBy(a => a.Id)
                           .AsNoTracking()
                   .Include(a => a.MaintainableAsset)
                   .Include(a => a.AssetDetailValues)
                   .Include(a => a.TreatmentConsiderationDetails)
                   .ThenInclude(tc => tc.CashFlowConsiderationDetails)
                   .Include(a => a.TreatmentConsiderationDetails)
                   .ThenInclude(tc => tc.BudgetUsageDetails)
                   .Include(a => a.TreatmentOptionDetails)
                   .Include(a => a.TreatmentRejectionDetails)
                   .Include(a => a.TreatmentSchedulingCollisionDetails)
                   .Skip(AssetLoadBatchSize * batchIndex)
                   .Take(AssetLoadBatchSize)
                   .ToList();
                    var assets = AssetDetailMapper.ToDomainList(assetEntities, year, attributeNameLookup);
                    domainYear.Assets.AddRange(assets);
                    shouldContinueLoadingAssets = assets.Count == AssetLoadBatchSize;
                    _unitOfWork.Context.ChangeTracker.Clear();
                    memos.Mark($"b{batchIndex}");
                    batchIndex++;
                }
            }
            domain.Years.Sort((y1, y2) => y1.Year.CompareTo(y2.Year));
            var timings = memos.ToMultilineString(true);
            System.Diagnostics.Debug.WriteLine(timings);
            var directory = Directory.GetCurrentDirectory();
            var path = Path.Combine(directory, "LoadTimings.txt");
            File.Delete(path);
            File.WriteAllText(path, timings);
            return domain;
        }
    }
}
