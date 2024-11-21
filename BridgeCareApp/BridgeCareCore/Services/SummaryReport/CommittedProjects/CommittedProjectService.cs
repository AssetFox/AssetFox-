using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.iAM.Common.Logging;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Utils;
using MoreLinq;
using OfficeOpenXml;
using NLog;

namespace BridgeCareCore.Services.SummaryReport.CommittedProjects
{
    public class CommittedProjectService : ICommittedProjectService
    {
        private static IUnitOfWork _unitOfWork;
        private readonly IHubService _hubService;
        public const string UnknownBudgetName = "Unknown";

        // TODO: Determine based on associated network
        private string _networkKeyField;
        private readonly Dictionary<string, List<KeySegmentDatum>> _keyProperties;
        private readonly List<string> _keyFields;        
        private bool newImportFile = false;

        public CommittedProjectService(IUnitOfWork unitOfWork, IHubService hubService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _hubService = hubService;
        }        

        public FileInfoDTO ExportCommittedProjectsFile(Guid simulationId)
        {
            
            var simulation = _unitOfWork.SimulationRepo.GetSimulation(simulationId);
            var keyProperties = _unitOfWork.AssetDataRepository.KeyProperties;
            var keyFields = keyProperties.Keys.Where(_ => _ != "ID").ToList();
            var networkKeyField = _unitOfWork.NetworkRepo.GetNetworkKeyAttribute(simulation.NetworkId);
            var primaryKeyFieldNames = _unitOfWork.AdminSettingsRepo.GetKeyFields();

            var exporter = new CommittedProjectsExporter(
                _unitOfWork,
                simulation,
                networkKeyField,
                keyFields,
                keyProperties,
                primaryKeyFieldNames.ToList());


            var fileInfo = exporter.ExportCommittedProjectsFile(simulationId);
            return fileInfo;
        }

        public FileInfoDTO CreateCommittedProjectTemplate(Guid networkId)
        {            
            _networkKeyField = _unitOfWork.NetworkRepo.GetNetworkKeyAttribute(networkId);
            var generator = new CommittedProjectsTemplateGenerator(_networkKeyField);
            var template = generator.CreateCommittedProjectTemplate();
            return template;        
        }

        /**
         * Gets a Dictionary of Attribute Id per Attribute Name
         */
        private Dictionary<string, Guid> GetAttributeIdsPerAttributeName(List<string> consequenceAttributeNames)
        {
            var attributeDTOs = _unitOfWork.AttributeRepo.GetAttributes();
            var attributeNames = attributeDTOs.Select(_ => _.Name).ToList();

            // Ignore factor columns as bad columns (used for performance factor)
            foreach (var missingAttribute in consequenceAttributeNames)
            {
                if (missingAttribute.Contains("_factor"))
                {
                    attributeNames.Add(missingAttribute);

                    newImportFile = true;
                }
            }

            if (consequenceAttributeNames.Any(name => !attributeNames.Contains(name)))
            {
                var missingAttributes = consequenceAttributeNames.Except(attributeNames).ToList();


                if (missingAttributes.Count == 1)
                {
                    throw new RowNotInTableException($"No attribute found having name {missingAttributes[0]}.");
                }

                throw new RowNotInTableException(
                    $"No attributes found having names: {string.Join(", ", missingAttributes)}.");
            }

            return attributeDTOs.ToDictionary(_ => _.Name, _ => _.Id);
        }

        public void ImportCommittedProjectFiles(
            Guid simulationId,
            ExcelPackage excelPackage,
            string filename,
            string UserId,
            CancellationToken? cancellationToken = null,
            IWorkQueueLog queueLog = null)
        {
            queueLog ??= new DoNothingWorkQueueLog();

            var simulation = _unitOfWork.SimulationRepo.GetSimulation(simulationId);
            var keyProperties = _unitOfWork.AssetDataRepository.KeyProperties;
            var keyFields = keyProperties.Keys.Where(_ => _ != "ID").ToList();
            var networkKeyField = _unitOfWork.NetworkRepo.GetNetworkKeyAttribute(simulation.NetworkId);

            var importer = new CommittedProjectImporter(
                _unitOfWork,
                _hubService,                
                networkKeyField,
                keyFields);

            if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
            {
                return;
            }

            queueLog.UpdateWorkQueueStatus("Creating Committed Projects");

            var committedProjectDTOs = importer.ImportProjectsFromWorksheet(
                excelPackage.Workbook.Worksheets[0],
                simulation,
                UserId);


            if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
            {
                return;
            }

            queueLog.UpdateWorkQueueStatus("Deleting Old Committed Projects");

            _unitOfWork.CommittedProjectRepo.DeleteSimulationCommittedProjects(simulationId);
            if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
            {
                return;
            }

            queueLog.UpdateWorkQueueStatus("Upserting Created Committed Projects");
            _unitOfWork.CommittedProjectRepo.UpsertCommittedProjects(committedProjectDTOs);

            if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
            {
                return;
            }
        }

        public double GetTreatmentCost(string assetKeyData, Guid treatmentId, Guid networkId)
        {
            var asset = _unitOfWork.MaintainableAssetRepo.GetMaintainableAssetByKeyAttribute(networkId, assetKeyData);

            if (asset == null)
                return 0;
            var treatmentCosts = _unitOfWork.TreatmentCostRepo.GetTreatmentCostByScenarioTreatmentId(treatmentId);

            double totalCost = 0;
            if (treatmentCosts == null)
                return totalCost;
            foreach (var cost in treatmentCosts)
            {
                var compiler = new CalculateEvaluateCompiler();

                if (cost.CriterionLibrary.Id != Guid.Empty && !IsCriteriaValid(compiler, cost.CriterionLibrary.MergedCriteriaExpression, asset.Id))
                    continue;

                compiler = new CalculateEvaluateCompiler();
                var attributes = InstantiateCompilerAndGetExpressionAttributes(cost.Equation.Expression, compiler);
                var attributeIds = attributes.Select(a => a.Id).ToList();
                var aggregatedResults = _unitOfWork.AggregatedResultRepo.GetAggregatedResultsForMaintainableAsset(asset.Id, attributeIds);
                var latestAggResults = new List<AggregatedResultDTO>();
                foreach (var attr in attributes)
                {
                    var attrs = aggregatedResults.Where(_ => _.Attribute.Id == attr.Id).ToList();
                    if (attrs.Count == 0)
                        continue;
                    var latestYear = attrs.Max(_ => _.Year);
                    var latestAggResult = attrs.FirstOrDefault(_ => _.Year == latestYear);
                    latestAggResults.Add(latestAggResult);
                }
                var calculator = compiler.GetCalculator(cost.Equation.Expression);
                var scope = new CalculateEvaluateScope();
                if (latestAggResults.Count != attributes.Count)
                    continue;
                InstantiateScope(latestAggResults, scope);
                var currentCost = calculator.Delegate(scope);
                totalCost += currentCost;
            }
            return totalCost;
        }

        public List<CommittedProjectConsequenceDTO> GetValidConsequences(Guid committedProjectId, Guid treatmentId, string assetKeyData, Guid networkId)
        {
            var consequencesToReturn = new List<CommittedProjectConsequenceDTO>();
            var asset = _unitOfWork.MaintainableAssetRepo.GetMaintainableAssetByKeyAttribute(networkId, assetKeyData);
            if (asset == null)
                return consequencesToReturn;
            var treatmentConsequences = _unitOfWork.TreatmentConsequenceRepo.GetScenarioTreatmentConsequencesByTreatmentId(treatmentId);
            if (treatmentConsequences == null)
                return consequencesToReturn;
            foreach (var consequence in treatmentConsequences)
            {
                var compiler = new CalculateEvaluateCompiler();
                if (consequence.CriterionLibrary.Id == Guid.Empty)
                {
                    consequencesToReturn.Add(new CommittedProjectConsequenceDTO() { Id = Guid.NewGuid(), CommittedProjectId = committedProjectId, Attribute = consequence.Attribute, ChangeValue = consequence.ChangeValue });
                    continue;
                }
                if (IsCriteriaValid(compiler, consequence.CriterionLibrary.MergedCriteriaExpression, asset.Id))
                    consequencesToReturn.Add(new CommittedProjectConsequenceDTO() { Id = Guid.NewGuid(), CommittedProjectId = committedProjectId, Attribute = consequence.Attribute, ChangeValue = consequence.ChangeValue });
            }
            return consequencesToReturn;
        }

        private bool IsCriteriaValid(CalculateEvaluateCompiler compiler, string expression, Guid assetId)
        {
            var attributes = InstantiateCompilerAndGetExpressionAttributes(expression, compiler);
            var attributeIds = attributes.Select(a => a.Id).ToList();
            var aggResults = _unitOfWork.AggregatedResultRepo.GetAggregatedResultsForMaintainableAsset(assetId, attributeIds);
            var latestAggResults = new List<AggregatedResultDTO>();
            foreach (var attr in attributes)
            {
                var attrs = aggResults.Where(_ => _.Attribute.Id == attr.Id).ToList();
                if (attrs.Count == 0)
                    continue;
                var latestYear = attrs.Max(_ => _.Year);
                var latestAggResult = attrs.FirstOrDefault(_ => _.Year == latestYear);
                latestAggResults.Add(latestAggResult);
            }
            if (latestAggResults.Count != attributes.Count)
                return false;
            var evaluator = compiler.GetEvaluator(expression);
            var scope = new CalculateEvaluateScope();
            InstantiateScope(latestAggResults, scope);
            return evaluator.Delegate(scope);
        }

        private List<AttributeDTO> InstantiateCompilerAndGetExpressionAttributes(string mergedCriteriaExpression, CalculateEvaluateCompiler compiler)
        {
            var modifiedExpression = mergedCriteriaExpression
                    .Replace("[", "")
                    .Replace("]", "")
                    .Replace("@", "")
                    .Replace("|", "'")
                    .ToUpper();

            var pattern = "\\[[^\\]]*\\]";
            var rg = new Regex(pattern);
            var match = rg.Matches(mergedCriteriaExpression);
            var hashMatch = new List<string>();
            foreach (Match m in match)
            {
                hashMatch.Add(m.Value.Substring(1, m.Value.Length - 2));
            }

            var attributes = _unitOfWork.AttributeRepo.GetAttributesWithNames(hashMatch);

            attributes.ForEach(attribute =>
            {
                compiler.ParameterTypes[attribute.Name] = attribute.Type == "NUMBER"
                    ? CalculateEvaluateParameterType.Number
                    : CalculateEvaluateParameterType.Text;
            });

            return attributes;
        }

        private void InstantiateScope(List<AggregatedResultDTO> results, CalculateEvaluateScope scope)
        {
            results.ForEach(_ =>
            {
                if (_.Attribute.Type == "NUMBER")
                {
                    scope.SetNumber(_.Attribute.Name, _.NumericValue.Value);
                }
                else
                {
                    scope.SetText(_.Attribute.Name, _.TextValue);
                }
            });
        }


    }
}
