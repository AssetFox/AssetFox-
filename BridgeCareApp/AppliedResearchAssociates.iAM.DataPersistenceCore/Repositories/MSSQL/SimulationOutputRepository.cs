using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.Common.PerformanceMeasurement;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Enums;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SimulationOutputRepository : ISimulationOutputRepository
    {
        private const bool ShouldHackSaveOutputToFile = false;
        private const bool ShouldHackSaveTimingsToFile = false;
        public const string SimulationOutputLoadKey = "SimulationOutputSqlBatches";
        public const string AssetLoadBatchSizeOverrideKey = "AssetDetailBatchSizeOverrideForValueLoad";
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        public const int AssetLoadBatchSize = 2000;
        public const int AssetDetailSaveBatchSize = 100000;
        public const string AssetDetailSaveOverrideBatchSizeKey = "AssetDetailBatchSizeOverrideForValueSave";

        public SimulationOutputRepository(UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        
        public void CreateSimulationOutputViaRelational(Guid simulationId, SimulationOutput simulationOutput, ILog loggerForUserInfo = null, ILog loggerForTechnicalInfo = null)
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
            var saveMemos = EventMemoModelLists.GetFreshInstance("Save");
            var simulationMemos = EventMemoModelLists.GetInstance("Simulation");
            simulationMemos.Mark("Starting save");
            var startMemo = saveMemos.MarkInformation("Starting save", loggerForTechnicalInfo);
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
                var configuredBatchSize = GetConfiguredBatchSize(_unitOfWork.Config, AssetDetailSaveOverrideBatchSizeKey);
                var batchSize = configuredBatchSize ?? AssetDetailSaveBatchSize;
                var assetSummaries = simulationOutput.InitialAssetSummaries;
                saveMemos.Mark("assetSummaries");
                var family = AssetSummaryDetailMapper.ToEntityLists(assetSummaries, entity.Id, attributeIdLookup);
                _unitOfWork.Context.AddAll(family.AssetSummaryDetails, batchSize: batchSize);
                simulationMemos.Mark("assetSummaryDetails");
                _unitOfWork.Context.AddAll(family.AssetSummaryDetailValues, batchSize: batchSize);
                saveMemos.Mark("assetSummaryDetailValues");
                _unitOfWork.Commit();
                 foreach (var year in simulationOutput.Years)
                {
                    loggerForUserInfo.Information($"Saving year {year.Year}");
                    _unitOfWork.BeginTransaction();
                    var yearMemo = saveMemos.MarkInformation($"Y{year.Year}", loggerForTechnicalInfo);
                    var yearDetail = SimulationYearDetailMapper.ToEntityWithoutAssets(year, entity.Id, attributeIdLookup);
                    _unitOfWork.Context.Add(yearDetail);
                    var assets = year.Assets;
                    var assetFamily = AssetDetailMapper.ToEntityFamily(assets, yearDetail.Id, attributeIdLookup);
                    _unitOfWork.Context.AddAll(assetFamily.AssetDetails, batchSize: batchSize);
                    saveMemos.Mark($" {assetFamily.AssetDetails.Count} assetDetails");
                    _unitOfWork.Context.AddAll(assetFamily.AssetDetailValues, batchSize: batchSize);
                    saveMemos.Mark($" {assetFamily.AssetDetailValues.Count} assetDetailValues batchSize: {batchSize}");
                    _unitOfWork.Context.AddAll(assetFamily.TreatmentOptionDetails, batchSize: batchSize);
                    saveMemos.Mark($" {assetFamily.TreatmentOptionDetails.Count} treatmentOptionDetails");
                    _unitOfWork.Context.AddAll(assetFamily.TreatmentRejectionDetails, batchSize: batchSize);
                    saveMemos.Mark($" {assetFamily.TreatmentRejectionDetails.Count} treatmentRejectionDetails");
                    _unitOfWork.Context.AddAll(assetFamily.TreatmentSchedulingCollisionDetails, batchSize: batchSize);
                    saveMemos.Mark($" {assetFamily.TreatmentSchedulingCollisionDetails.Count} treatmentSchedulingCollisionDetails");
                    _unitOfWork.Context.AddAll(assetFamily.TreatmentConsiderationDetails, batchSize: batchSize);
                    saveMemos.Mark($" {assetFamily.TreatmentSchedulingCollisionDetails.Count} treatmentConsiderationDetails");
                    _unitOfWork.Context.AddAll(assetFamily.BudgetUsageDetails, batchSize: batchSize);
                    saveMemos.Mark($" {assetFamily.BudgetUsageDetails.Count} budgetUsageDetails");
                    _unitOfWork.Context.AddAll(assetFamily.CashFlowConsiderationDetails, batchSize: batchSize);
                    saveMemos.Mark($" {assetFamily.CashFlowConsiderationDetails.Count} cashFlowConsiderationDetails");
                    _unitOfWork.Commit();
                    saveMemos.Mark(" Committed");
                    _unitOfWork.Context.ChangeTracker.Clear();
                    saveMemos.Mark(" Cleared ChangeTracker");
                }
                saveMemos.MarkInformation("Save complete", loggerForTechnicalInfo);
                simulationMemos.Mark("Save complete");
                if (ShouldHackSaveTimingsToFile)
                {
                    var timingsOutputFilename = "SaveTimings.txt";
                    var simulationOutputFilename = "SimulationTimings.txt";
                    WriteTimingsToFile(saveMemos, timingsOutputFilename);
                    WriteTimingsToFile(simulationMemos, simulationOutputFilename);
                }
                loggerForUserInfo.Information(SimulationUserMessages.SimulationOutputSavedToDatabase);
            }
            catch (Exception ex)
            {
                var error = saveMemos.Mark($"Save failed with exception {ex.Message}");
                loggerForTechnicalInfo.Error(error);
                _unitOfWork.Rollback();
                throw;
            }
        }

        public void CreateSimulationOutputViaJson(Guid simulationId, SimulationOutput simulationOutput)
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

            var settings = new StringEnumConverter();

            try
            {
                _unitOfWork.Context.DeleteAll<SimulationOutputJsonEntity>(_ =>
                _.SimulationId == simulationId);

                var outputInitialConditionNetwork = JsonConvert.SerializeObject(simulationOutput.InitialConditionOfNetwork, settings);

                _unitOfWork.Context.Add(
                        new SimulationOutputJsonEntity
                        {
                            Id = Guid.NewGuid(),
                            SimulationId = simulationId,
                            Output = outputInitialConditionNetwork,
                            OutputType = SimulationOutputEnum.InitialConditionNetwork
                        });

                var outputInitialSummary = JsonConvert.SerializeObject(simulationOutput.InitialAssetSummaries, settings);

                _unitOfWork.Context.Add(
                        new SimulationOutputJsonEntity
                        {
                            Id = Guid.NewGuid(),
                            SimulationId = simulationId,
                            Output = outputInitialSummary,
                            OutputType = SimulationOutputEnum.InitialSummary
                        });

                foreach (var item in simulationOutput.Years)
                {
                    var  targetGoalsToRemove = item.TargetConditionGoals.Where(_ => double.IsNaN(_.TargetValue) || double.IsNaN(_.ActualValue)).ToList();
                    targetGoalsToRemove.ForEach(_ => item.TargetConditionGoals.Remove(_));
                    var conditionGoalsToRemove = item.DeficientConditionGoals.Where(_ => double.IsNaN(_.DeficientLimit) || double.IsNaN(_.AllowedDeficientPercentage) || double.IsNaN(_.ActualDeficientPercentage)).ToList();
                    conditionGoalsToRemove.ForEach(_ => item.DeficientConditionGoals.Remove(_));
                    var simulationOutputYearData = JsonConvert.SerializeObject(item, settings);

                    _unitOfWork.Context.Add(
                        new SimulationOutputJsonEntity
                        {
                            Id = Guid.NewGuid(),
                            SimulationId = simulationId,
                            Output = simulationOutputYearData,
                            OutputType = SimulationOutputEnum.YearlySection
                        });
                }
                _unitOfWork.Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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

        /// <summary>
        /// Returns batch size from the configuration. 
        /// </summary>
        /// <param name="config">The configuration to look in</param>
        /// <param name="key">The key to use inside the configuration</param>
        /// <returns>If no batch size is found in the
        /// configuration, or if zero or less is found, returns null. Otherwise, returns the integer found.</returns>
        private static int? GetConfiguredBatchSize(IConfiguration config, string configurationKey)
        {
            var section = config.GetSection(SimulationOutputLoadKey);
            var overrideSection = section.GetSection(configurationKey);
            var overrideValue = overrideSection.Value;
            if (overrideValue!=null)
            {
                if (int.TryParse(overrideValue, out int overrideInt))
                {
                    if (overrideInt > 0)
                    {
                        return overrideInt;
                    }
                }
            }
            return null;
        }

        public SimulationOutput GetSimulationOutputViaRelation(Guid simulationId, ILog loggerForUserInfo = null, ILog loggerForTechinalInfo = null)
        {
            loggerForUserInfo ??= new DoNotLog();
            loggerForTechinalInfo ??= new DoNotLog();
            _unitOfWork.Context.Database.SetCommandTimeout(TimeSpan.FromSeconds(3600));
            var memos = EventMemoModelLists.GetFreshInstance("Load");
            var assetLoadBatchSize = GetConfiguredBatchSize(_unitOfWork.Config, AssetLoadBatchSizeOverrideKey) ?? AssetLoadBatchSize; ;
            var startMemo = memos.MarkInformation($"Starting load batchSize {assetLoadBatchSize}", loggerForTechinalInfo);
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
            var attributeNameLookup = _unitOfWork.AttributeRepo.GetAttributeNameLookupDictionary();
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
                   .Include(a => a.AssetDetailValuesIntId)
                   .AsSplitQuery()
                   .Skip(assetLoadBatchSize * batchIndex)
                   .Take(assetLoadBatchSize)
                   .ToList();
                    memos.Mark("  assetEntities");
                    if (assetEntities.Any())
                    {
                        AssetDetailMapper.AppendToDomainDictionaryWithValues(assets, assetEntities, year, attributeNameLookup, assetNameLookup);
                        _unitOfWork.Context.ChangeTracker.Clear();
                    }
                    memos.Mark($" batch {batchIndex} done");
                    batchIndex++;
                    shouldContinueLoadingAssets = assetEntities.Count() == assetLoadBatchSize;
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

        /// <summary>
        /// Method that GetSimulationsOutput query uses in order to retrieve simulation output object via simulation id.
        /// Was created due GetSimulationsOutput query giving a simulation error when simulation isn't received beforehand.
        /// </summary>
        /// <param name="simulationId">The simulation id passed in.</param>
        /// <returns>Simulation object based on simulation id.</returns>
        public SimulationOutput GetSimulationOutputViaJsonGraphQL(Guid simulationId)
        {
            var simulationDTO = _unitOfWork.SimulationRepo.GetSimulation(simulationId);
            var simulationOutput = _unitOfWork.SimulationOutputRepo.GetSimulationOutputViaJson(simulationId);
            return simulationOutput;
        }
        public SimulationOutput GetSimulationOutputViaJson(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"Found no simulation having id {simulationId}");
            }

            if (!_unitOfWork.Context.SimulationOutputJson.Any(_ => _.SimulationId == simulationId))
            {
                throw new RowNotInTableException($"No simulation analysis results were found for simulation having id {simulationId}. Please ensure that the simulation analysis has been run.");
            }

            var simulationOutputObjects = _unitOfWork.Context.SimulationOutputJson.Where(_ => _.SimulationId == simulationId);

            var simulationOutput = new SimulationOutput();
            foreach (var item in simulationOutputObjects)
            {
                switch (item.OutputType)
                {
                case SimulationOutputEnum.YearlySection:
                    var yearlySections = JsonConvert.DeserializeObject<SimulationYearDetail>(item.Output, new JsonSerializerSettings
                    {
                        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                    });
                    simulationOutput.Years.Add(yearlySections);
                    break;
                case SimulationOutputEnum.InitialConditionNetwork:
                    simulationOutput.InitialConditionOfNetwork = Convert.ToDouble(item.Output);
                    break;
                case SimulationOutputEnum.InitialSummary:
                    var initialSummary = JsonConvert.DeserializeObject<List<AssetSummaryDetail>>(item.Output, new JsonSerializerSettings
                    {
                        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                    });
                    simulationOutput.InitialAssetSummaries.AddRange(initialSummary);
                    break;
                }
            }
            simulationOutput.Years.Sort((a, b) => a.Year.CompareTo(b.Year));
            simulationOutput.LastModifiedDate = simulationOutputObjects.FirstOrDefault().Simulation.LastModifiedDate;

            return simulationOutput;
        }

        public void ConvertSimulationOutpuFromJsonTorelational(Guid simulationId)
        {
            var output = GetSimulationOutputViaJson(simulationId);            
            CreateSimulationOutputViaRelational(simulationId, output);
            try
            {
                _unitOfWork.BeginTransaction();
                var outputId = _unitOfWork.Context.SimulationOutput.First(_ => _.SimulationId == simulationId).Id;
                var outputJsons = _unitOfWork.Context.SimulationOutputJson.Where(_ => _.SimulationId == simulationId).ToList();
                if (outputJsons.Count != 0)
                {
                    outputJsons.ForEach(_ => _.SimulationOutputId = outputId);
                }
                
                _unitOfWork.Context.UpdateAll(outputJsons);
                _unitOfWork.Commit();
            }
            catch(Exception ex)
            {
                _unitOfWork.Rollback();
                throw;
            }
            
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
