using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.Common.PerformanceMeasurement;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SimulationOutputRepository : ISimulationOutputRepository
    {
        private const bool ShouldHackSaveOutputToFile = false;
        private const bool ShouldHackSaveTimingsToFile = false;
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        public const int AssetLoadBatchSize = 2000;

        public SimulationOutputRepository(UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateSimulationOutput(Guid simulationId, SimulationOutput simulationOutput, ILog loggerForUserInfo = null, ILog loggerForTechnicalInfo = null)
        {
            loggerForTechnicalInfo ??= new DoNotLog();
            loggerForUserInfo ??= new DoNotLog();
            loggerForUserInfo.Information("Preparing to save to database");
            if (ShouldHackSaveOutputToFile)
            {
#pragma warning disable CS0162 // Unreachable code detected
                HackSaveOutputToFile(simulationOutput);
#pragma warning restore CS0162 // Unreachable code detected
            }
            var memos = EventMemoModelLists.GetFreshInstance("Save");
            var startMemo = memos.MarkInformation("Starting save", loggerForTechnicalInfo);
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation found for given scenario.");
            }

            _unitOfWork.BeginTransaction();

            // Once we are inside the transaction, there is not much value in adding logs to the
            // user logger. The problem is that these are often communicated to the user via the database.
            // But the database won't update until the transaction is completed. Therefore,
            // adding user logs about the transaction's progress will likely just cause confusion
            // about why these logs are sometimes not appearing in the UI.
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
                memos.Mark("assetSummaries");
                var family = AssetSummaryDetailMapper.ToEntityLists(assetSummaries, entity.Id, attributeIdLookup);
                _unitOfWork.Context.AddAll(family.AssetSummaryDetails);
                memos.Mark("assetSummaryDetails");
                _unitOfWork.Context.AddAll(family.AssetSummaryDetailValues);
                memos.Mark("assetSummaryDetailValues");
                _unitOfWork.Commit();
                foreach (var year in simulationOutput.Years)
                {
                    loggerForUserInfo.Information($"Saving year {year.Year}");
                    _unitOfWork.BeginTransaction();
                    var yearMemo = memos.MarkInformation($"Y{year.Year}", loggerForTechnicalInfo);
                    var yearDetail = SimulationYearDetailMapper.ToEntityWithoutAssets(year, entity.Id, attributeIdLookup);
                    _unitOfWork.Context.Add(yearDetail);
                    var assets = year.Assets;
                    var assetFamily = AssetDetailMapper.ToEntityFamily(assets, yearDetail.Id, attributeIdLookup);
                    _unitOfWork.Context.AddAll(assetFamily.AssetDetails);
                    memos.Mark($" {assetFamily.AssetDetails.Count} assetDetails");
                    _unitOfWork.Context.AddAll(assetFamily.AssetDetailValues);
                    memos.Mark($" {assetFamily.AssetDetailValues.Count} assetDetailValues");
                    _unitOfWork.Context.AddAll(assetFamily.TreatmentOptionDetails);
                    memos.Mark($" {assetFamily.TreatmentOptionDetails.Count} treatmentOptionDetails");
                    _unitOfWork.Context.AddAll(assetFamily.TreatmentRejectionDetails);
                    memos.Mark($" {assetFamily.TreatmentRejectionDetails.Count} treatmentRejectionDetails");
                    _unitOfWork.Context.AddAll(assetFamily.TreatmentSchedulingCollisionDetails);
                    memos.Mark($" {assetFamily.TreatmentSchedulingCollisionDetails.Count} treatmentSchedulingCollisionDetails");
                    _unitOfWork.Context.AddAll(assetFamily.TreatmentConsiderationDetails);
                    memos.Mark($" {assetFamily.TreatmentSchedulingCollisionDetails.Count} treatmentConsiderationDetails");
                    _unitOfWork.Context.AddAll(assetFamily.BudgetUsageDetails);
                    memos.Mark($" {assetFamily.BudgetUsageDetails.Count} budgetUsageDetails");
                    _unitOfWork.Context.AddAll(assetFamily.CashFlowConsiderationDetails);
                    memos.Mark($" {assetFamily.CashFlowConsiderationDetails.Count} cashFlowConsiderationDetails");
                    _unitOfWork.Commit();
                    memos.Mark(" Committed");
                    _unitOfWork.Context.ChangeTracker.Clear();
                    memos.Mark(" Cleared ChangeTracker");
                }
                memos.MarkInformation("Save complete", loggerForTechnicalInfo);
                if (ShouldHackSaveTimingsToFile)
                {
                    var outputFilename = "SaveTimings.txt";
                    WriteTimingsToFile(memos, outputFilename);
                }
                loggerForUserInfo.Information("Simulation output saved to database");
            }
            catch (Exception ex)
            {
                var error = memos.Mark($"Save failed with exception {ex.Message}");
                loggerForTechnicalInfo.Error(error);
                _unitOfWork.Rollback();
                throw;
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

        public SimulationOutput GetSimulationOutput(Guid simulationId, ILog loggerForUserInfo = null, ILog loggerForTechinalInfo = null)
        {
            loggerForUserInfo ??= new DoNotLog();
            loggerForTechinalInfo ??= new DoNotLog();
            var memos = EventMemoModelLists.GetFreshInstance("Load");
            var startMemo = memos.MarkInformation("Starting load", loggerForTechinalInfo);
            loggerForUserInfo.Information("Loading SimulationOutput");
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
                .Include(so => so.Simulation)
                .Where(_ => _.SimulationId == simulationId)
                .AsNoTracking()
                .ToList();
            var firstEntity = entitiesWithoutAssetSummariesOrYearContents[0];
            var simulationOutputId = firstEntity.Id;
            var cacheYears = firstEntity.Years.OrderBy(y => y.Year).ToList();
            firstEntity.Years.Clear();
            var domain = SimulationOutputMapper.ToDomainWithoutAssets(firstEntity, attributeNameLookup);
            var assetNameLookup = new Dictionary<Guid, string>();
            var usedAttributeIds = BuildUsedAttributeIdList(simulationOutputId);
            var assetSummaryDetails = _unitOfWork.Context.AssetSummaryDetail
                .Include(a => a.MaintainableAsset)
                .OrderBy(a => a.Id)
                .Where(a => a.SimulationOutputId == simulationOutputId)
                .AsNoTracking()
                .ToList();
            memos.Mark("assetSummaryDetails");
            foreach (var assetSummary in assetSummaryDetails)
            {
                assetNameLookup[assetSummary.MaintainableAssetId] = assetSummary.MaintainableAsset.AssetName;
            }
            var assetSummaryDomainDictionary = AssetSummaryDetailMapper.ToDomainDictionaryNullSafe(assetSummaryDetails, attributeNameLookup);
            domain.InitialAssetSummaries.AddRange(assetSummaryDomainDictionary.Values);
            var assetSummaryDetailValueConfig = new BulkConfig
            {
                UpdateByProperties = new List<string> { nameof(AssetSummaryDetailValueEntityIntId.AssetSummaryDetailId), nameof(AssetSummaryDetailValueEntityIntId.AttributeId) }
            };
            var assetSummaryDetailValueEntities = new List<AssetSummaryDetailValueEntityIntId>();
            foreach (var assetSummaryDetail in assetSummaryDetails)
            {
                foreach (var usedAttributeId in usedAttributeIds)
                {
                    assetSummaryDetailValueEntities.Add(new AssetSummaryDetailValueEntityIntId
                    {
                        AttributeId = usedAttributeId,
                        AssetSummaryDetailId = assetSummaryDetail.Id,
                    });
                }
            }
            var configMemo = memos.MarkInformation("assetSummary config", loggerForTechinalInfo);
            _unitOfWork.Context.BulkRead(assetSummaryDetailValueEntities, assetSummaryDetailValueConfig);
            foreach (var assetSummaryDetailValueEntity in assetSummaryDetailValueEntities)
            {
                var summary = assetSummaryDomainDictionary[assetSummaryDetailValueEntity.AssetSummaryDetailId];
                AssetSummaryDetailValueMapper.AddToDictionary(assetSummaryDetailValueEntity, summary.ValuePerNumericAttribute, summary.ValuePerTextAttribute, attributeNameLookup);
            }
            foreach (var summaryValue in assetSummaryDomainDictionary.Values)
            {
                AssetSummaryDetailValueMapper.FillAreaAttributeValue(summaryValue.ValuePerNumericAttribute);
            }
            var summariesDoneMemo = memos.MarkInformation("assetSummaries done", loggerForTechinalInfo);
            foreach (var cacheYear in cacheYears)
            {
                var yearMemo = memos.MarkInformation($"Y{cacheYear.Year}", loggerForTechinalInfo);
                loggerForUserInfo.Information($"Loading {cacheYear.Year}");
                var yearId = cacheYear.Id;
                var year = cacheYear.Year;
                var loadedYearWithoutAssets = _unitOfWork.Context.SimulationYearDetail
                .Include(y => y.Budgets)
                .Include(y => y.DeficientConditionGoalDetails)
                .Include(y => y.TargetConditionGoalDetails)
                .Where(y => y.Id == yearId)
                .AsNoTracking()
                .ToList();
                var loadedYearEntity = loadedYearWithoutAssets[0];
                var domainYear = SimulationYearDetailMapper.ToDomainWithoutAssets(loadedYearEntity, attributeNameLookup);
                domain.Years.Add(domainYear);
                var shouldContinueLoadingAssets = true;
                var batchIndex = 0;
                var assets = new Dictionary<Guid, AssetDetail>();
                while (shouldContinueLoadingAssets)
                {
                    var assetEntities = _unitOfWork.Context.AssetDetail
                           .Where(a => a.SimulationYearDetailId == yearId)
                           .OrderBy(a => a.Id)
                   .AsNoTracking()
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
                    memos.Mark("  assetEntities");
                    AssetDetailMapper.AppendToDomainDictionary(assets, assetEntities, year, attributeNameLookup, assetNameLookup);
                    memos.Mark("  toDomainDictionary");
                    var assetDetailValues = new List<AssetDetailValueEntityIntId>();
                    foreach (var assetEntity in assetEntities)
                    {
                        foreach (var attributeId in usedAttributeIds)
                        {
                            var assetDetail = new AssetDetailValueEntityIntId
                            {
                                AssetDetailId = assetEntity.Id,
                                AttributeId = attributeId,
                            };
                            assetDetailValues.Add(assetDetail);
                        }
                    }
                    memos.Mark("  empty assetDetails");
                    var assetDetailValueConfig = new BulkConfig
                    {
                        UpdateByProperties = new List<string>
                        {
                            nameof(AssetDetailValueEntityIntId.AttributeId),
                            nameof(AssetDetailValueEntityIntId.AssetDetailId)
                        }
                    };
                    _unitOfWork.Context.BulkRead(assetDetailValues, assetDetailValueConfig);
                    memos.Mark("  BulkRead done");
                    foreach (var assetDetailValue in assetDetailValues)
                    {
                        var assetDetail = assets[assetDetailValue.AssetDetailId];
                        AssetDetailValueMapper.AddToDictionary(assetDetailValue, assetDetail.ValuePerTextAttribute, assetDetail.ValuePerNumericAttribute, attributeNameLookup);
                    }
                    memos.Mark($" batch {batchIndex} done");
                    batchIndex++;
                    shouldContinueLoadingAssets = assetEntities.Any();
                }
                domainYear.Assets.AddRange(assets.Values);
                foreach (var asset in domainYear.Assets)
                {
                    AssetDetailValueMapper.FillArea(asset.ValuePerNumericAttribute);
                }
            }
            domain.Years.Sort((y1, y2) => y1.Year.CompareTo(y2.Year));
            memos.MarkInformation("Load done", loggerForTechinalInfo);
            loggerForUserInfo.Information($"Simulation output load completed");
            if (ShouldHackSaveTimingsToFile)
            {
                var outputFilename = "LoadTimings.txt";
                WriteTimingsToFile(memos, outputFilename);
            }
            return domain;
        }

        private static void WriteTimingsToFile(List<EventMemoModel> memos, string filename)
        {
            var timings = memos.ToMultilineString(true);
            System.Diagnostics.Debug.WriteLine(timings);
            var directory = Directory.GetCurrentDirectory();
            var path = Path.Combine(directory, filename);
            File.Delete(path);
            File.WriteAllText(path, timings);
        }

        private List<Guid> BuildUsedAttributeIdList(Guid simulationOutputId)
        {
            var usedAttributeIds = new List<Guid>();
            var randomAssetSummary = _unitOfWork.Context.AssetSummaryDetail
                .Include(a => a.AssetSummaryDetailValuesIntId)
                .Include(a => a.MaintainableAsset)
                .Where(a => a.SimulationOutputId == simulationOutputId)
                .AsNoTracking()
                .FirstOrDefault();
            foreach (var assetSummaryDetailValue in randomAssetSummary.AssetSummaryDetailValuesIntId)
            {
                usedAttributeIds.Add(assetSummaryDetailValue.AttributeId);
            }
            return usedAttributeIds;
        }
    }
}
