using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Utils;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using NuGet.ContentModel;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace BridgeCareCore.Services
{
    public class CommittedProjectService : ICommittedProjectService
    {
        private static IUnitOfWork _unitOfWork;
        public const string UnknownBudgetName = "Unknown";

        // TODO: Determine based on associated network
        private readonly string _networkKeyField = "BRKEY_";
        private Dictionary<string, List<KeySegmentDatum>> _keyProperties;
        private List<string> _keyFields;

        private static readonly List<string> InitialHeaders = new()
        {
            "TREATMENT",
            "YEAR",
            "YEARANY",
            "YEARSAME",
            "BUDGET",
            "COST",
            "AREA",
            "CATEGORY"
        };

        private static readonly string NoTreatment = "No Treatment";

        public CommittedProjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /**
         * Adds excel worksheet header row cell values for Committed Project Export
         */
        private void AddHeaderCells(ExcelWorksheet worksheet, List<string> attributeNames)
        {            
            var column = 1;
            if (_keyFields.Contains(_networkKeyField))
            {
                worksheet.Cells[1, column++].Value = _networkKeyField;
            }

            for (var keyColumnIndex = 0; keyColumnIndex < _keyFields.Count; keyColumnIndex++)
            {
                if (_keyFields[keyColumnIndex] != _networkKeyField)
                {
                    worksheet.Cells[1, column++].Value = _keyFields[keyColumnIndex];
                }
            }

            for (var headerCount = 0; headerCount < InitialHeaders.Count; headerCount++)
            {
                worksheet.Cells[1, column++].Value = InitialHeaders[headerCount];
            }

            attributeNames.ForEach(attributeName => worksheet.Cells[1, column++].Value = attributeName);
        }

        /**
         * Adds excel worksheet cell values for Committed Project Export
         */
        private void AddDataCells(ExcelWorksheet worksheet, List<BaseCommittedProjectDTO> committedProjectDTOs, List<string> orderedAttributeNames)
        {
            var row = 2;
            committedProjectDTOs.OrderBy(_ => _.LocationKeys[_networkKeyField])
                .ThenByDescending(_ => _.Year).ForEach(
                    project =>
                    {
                        var column = 1;
                        var locationValue = String.Empty;
                        if (project.LocationKeys.ContainsKey(_networkKeyField))
                        {
                            locationValue = project.LocationKeys[_networkKeyField];
                            worksheet.Cells[row, column++].Value = locationValue;
                        }
                                            
                        // Add other data from key fields based on ID
                        var otherData = _keyFields.Where(_ => _ != _networkKeyField);
                        if (!String.IsNullOrEmpty(locationValue) && otherData.Count() > 0)
                        {
                            var assetId = _keyProperties[_networkKeyField].FirstOrDefault(_ => _.KeyValue.Value == locationValue);
                            if (assetId != null)
                            {
                                foreach (var field in otherData)
                                {
                                    worksheet.Cells[row, column++].Value = _keyProperties[field].FirstOrDefault(_ => _.AssetId == assetId.AssetId).KeyValue.Value;
                                }
                            }
                            else
                            {
                                column += otherData.Count();
                            }
                        }
                        else
                        {
                            column += otherData.Count();
                        }

                        worksheet.Cells[row, column++].Value = project.Treatment;
                        worksheet.Cells[row, column++].Value = project.Year;
                        worksheet.Cells[row, column++].Value = project.ShadowForAnyTreatment;
                        worksheet.Cells[row, column++].Value = project.ShadowForSameTreatment;
                        var linkedBudget = _unitOfWork.BudgetRepo.GetScenarioBudgets(project.SimulationId).FirstOrDefault(_ => _.Id == project.ScenarioBudgetId);
                        var budgetName = linkedBudget?.Name ?? UnknownBudgetName;
                        if (budgetName == UnknownBudgetName)
                        {
                            budgetName = "";
                        }
                        worksheet.Cells[row, column++].Value = budgetName;
                        worksheet.Cells[row, column++].Value = project.Cost;
                        worksheet.Cells[row, column++].Value = string.Empty; // AREA
                        worksheet.Cells[row, column++].Value = project.Category.ToString();
                        // Cycling through the existing attributes will ensure the change values are matched to the correct attribute
                        orderedAttributeNames.ForEach(attribute =>
                        {
                            var specificChangeValue = project.Consequences.FirstOrDefault(_ => _.Attribute == attribute)?.ChangeValue ?? "";
                            worksheet.Cells[row, column++].Value = specificChangeValue;
                        });
                        row++;
                    });
        }

        public FileInfoDTO ExportCommittedProjectsFile(Guid simulationId)
        {
            var simulationName = _unitOfWork.SimulationRepo.GetSimulationName(simulationId);
            if (simulationName == null)
            {
                throw new ArgumentException($"Unable to find simulation with ID of {simulationId}");
            }

            var committedProjectDTOs = _unitOfWork.CommittedProjectRepo.GetCommittedProjectsForExport(simulationId);
                        
            var fileName = $"CommittedProjects_{simulationName.Trim().Replace(" ", "_")}.xlsx";

            using var excelPackage = new ExcelPackage(new FileInfo(fileName));

            var worksheet = excelPackage.Workbook.Worksheets.Add("Committed Projects");
            _keyProperties = _unitOfWork.AssetDataRepository.KeyProperties;
            _keyFields = _keyProperties.Keys.Where(_ => _ != "ID").ToList();
            if (committedProjectDTOs.Any())
            {
                var attributeNames = committedProjectDTOs
                    .SelectMany(_ =>
                        _.Consequences.Select(__ => __.Attribute).Distinct().OrderBy(__ => __))
                    .Distinct()
                    .ToList();
                AddHeaderCells(worksheet, attributeNames);
                AddDataCells(worksheet, committedProjectDTOs, attributeNames);
            }
            else
            {
                // Return a template
                AddHeaderCells(worksheet, new List<string>());
            }
            

            return new FileInfoDTO
            {
                FileName = fileName,
                FileData = Convert.ToBase64String(excelPackage.GetAsByteArray()),
                MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
        }

        public FileInfoDTO CreateCommittedProjectTemplate()
        {
            var fileName = $"CommittedProjectsTemplate.xlsx";

            using var excelPackage = new ExcelPackage(new FileInfo(fileName));

            var worksheet = excelPackage.Workbook.Worksheets.Add("Committed Projects");
            _keyProperties = _unitOfWork.AssetDataRepository.KeyProperties;
            _keyFields = _keyProperties.Keys.Where(_ => _ != "ID").ToList();
            AddHeaderCells(worksheet, new List<string> { "Add Consequences Here and in columns to the right" });

            return new FileInfoDTO
            {
                FileName = fileName,
                FileData = Convert.ToBase64String(excelPackage.GetAsByteArray()),
                MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
        }

        /**
         * Gets SimulationEntity data for a Committed Project Import
         */
        private SimulationEntity GetSimulationEntityForCommittedProjectImport(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new ArgumentException($"No simulation was found for the given scenario.");
            }

            return _unitOfWork.Context.Simulation
                .Include(_ => _.InvestmentPlan)
                .Include(_ => _.Budgets)
                .ThenInclude(_ => _.ScenarioBudgetAmounts)
                .Single(_ => _.Id == simulationId);
        }

        /**
         * Gets a Dictionary of AttributeEntity Id per AttributeEntity Name
         */
        private Dictionary<string, Guid> GetAttributeIdsPerAttributeName(List<string> consequenceAttributeNames)
        {
            var attributeDTOs = _unitOfWork.AttributeRepo.GetAttributes();
            var attributeNames = attributeDTOs.Select(_ => _.Name).ToList();

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

        /**
         * Gets a Dictionary of MaintainableAssetEntity Id per MaintainableAssetLocationEntity LocationIdentifier
         */
        private Dictionary<string, Guid> GetMaintainableAssetsPerLocationIdentifier(Guid networkId)
        {
            var assets = _unitOfWork.MaintainableAssetRepo.GetAllInNetworkWithLocations(networkId);
            if (!assets.Any())
            {
                throw new RowNotInTableException("There are no maintainable assets in the database.");
            }

            // Not sure if there is something else going on here
            //var oldReturn = _unitOfWork.Context.MaintainableAssetLocation
            //    .Where(_ => maintainableAssetsInNetwork.Any(__ => __.Id == _.MaintainableAssetId))
            //    .Select(maintainableAssetLocation => new MaintainableAssetEntity
            //    {
            //        Id = maintainableAssetLocation.MaintainableAssetId,
            //        MaintainableAssetLocation = new MaintainableAssetLocationEntity
            //        {
            //            LocationIdentifier = maintainableAssetLocation.LocationIdentifier
            //        }
            //    }).ToDictionary(_ => _.MaintainableAssetLocation.LocationIdentifier, _ => _.Id);

            return assets.ToDictionary(_ => _.Location.LocationIdentifier, _ => _.Id);
        }

        /**
         * Creates CommittedProjectEntity data for Committed Project Import
         */
        private List<SectionCommittedProjectDTO> CreateSectionCommittedProjectsForImport(Guid simulationId,
            ExcelPackage excelPackage, string filename, bool applyNoTreatment)
        {
            // First, get the simulation
            var simulationEntity = GetSimulationEntityForCommittedProjectImport(simulationId);

            if (simulationEntity.InvestmentPlan == null)
            {
                throw new RowNotInTableException("Simulation has no investment plan.");
            }

            // Get the Excel file workshhet containing the data to import
            var worksheet = excelPackage.Workbook.Worksheets[0];

            // Extract the names of the consequence headers and link them to their associated attribute IDs
            var headers = worksheet.Cells.GroupBy(cell => cell.Start.Row).First().Select(_ => _.GetValue<string>())
                .ToList();
            var consequenceAttributeNames = headers.Skip(_keyFields.Count + InitialHeaders.Count).ToList();
            var attributeIdsPerAttributeName = GetAttributeIdsPerAttributeName(consequenceAttributeNames.Distinct().ToList());
            var end = worksheet.Dimension.End;  // Contains both column and row ends

            // Get the location => asset ID lookup for all maintainable assets in the network
            var maintainableAssetIdsPerLocationIdentifier = GetMaintainableAssetsPerLocationIdentifier(simulationEntity.NetworkId);

            // Get the column ID for the network's key field
            if (!headers.Contains(_networkKeyField))
            {
                throw new RowNotInTableException($"Unable to find a column in the committed project sheet named {_networkKeyField}.  This is a required column for the network associated with the specified scenario");
            }
            var locationColumnNames = new Dictionary<int, string>();
            var keyColumn = 0;
            for (var column = 1; column <= _keyFields.Count; column++)
            {
                var columnName = worksheet.GetCellValue<string>(1, column);
                if (!_keyFields.Contains(columnName))
                {
                    var keyFieldList = new StringBuilder();
                    foreach (var field in _keyFields)
                    {
                        keyFieldList.Append(field);
                    }
                    throw new RowNotInTableException($"{columnName} is not a key field of the provided network.  Possible key fields are {keyFieldList}");
                }
                locationColumnNames.Add(column, columnName);
                if (columnName == _networkKeyField)
                {
                    keyColumn = column;
                }
            }
            if (keyColumn == 0)
            {
                // This should never be reached since we checked for existence unless there is a coding error
                throw new RowNotInTableException($"The key location column for this network was found in the locations, but its specific column number was not identified");
            }

            // Create the output lookup
            var projectsPerLocationIdentifierAndYearTuple = new Dictionary<(string, int), SectionCommittedProjectDTO>();

            // Read in the data by row
            for (var row = 2; row <= end.Row; row++)
            {
                // Get the project year for this work
                var projectYear = worksheet.GetCellValue<int>(row, _keyFields.Count + 2);  // Assumes that InitialHeaders stays constant

                // Get the location information of the project.  This must include the maintainable asset ID using the "ID" key
                var locationInformation = new Dictionary<string, string>();
                var locationIdentifier = worksheet.GetCellValue<string>(row, keyColumn);
                if (maintainableAssetIdsPerLocationIdentifier.Keys.ToList().Contains(locationIdentifier))
                {
                    // The location matches an asset in the network
                    locationInformation["ID"] = maintainableAssetIdsPerLocationIdentifier[locationIdentifier].ToString();
                }
                else
                    throw new RowNotInTableException($"An asset with the location identifier '{locationIdentifier}' does not exist");

                for (var column = 1; column <= _keyFields.Count; column++)
                {
                    locationInformation.Add(locationColumnNames[column], worksheet.GetCellValue<string>(row, column));
                }

                // Determine the appropriate budget entity to assign if any
                var budgetName = worksheet.GetCellValue<string>(row, _keyFields.Count + 5); // Assumes that InitialHeaders stays constant
                var budgetNameIsEmpty = string.IsNullOrWhiteSpace(budgetName);
                Guid? budgetId = null;

                if (!budgetNameIsEmpty)
                {
                    if (simulationEntity.Budgets.All(_ => _.Name != budgetName))
                    {
                        throw new RowNotInTableException(
                            $"Budget {budgetName} does not exist in the applied budget library.");
                    }
                    budgetId = simulationEntity.Budgets.Single(_ => _.Name == budgetName).Id;
                }

                // This to convert the incoming string to a TreatmentCategory
                var convertedCategory = new TreatmentCategory();
                try
                {
                    convertedCategory = EnumDeserializer.Deserialize<TreatmentCategory>(worksheet.GetCellValue<string>(row, _keyFields.Count + 8));// Assumes that InitialHeaders stays constant
                }
                catch
                {
                    convertedCategory = TreatmentCategory.Other;
                }

                // Build the committed project object
                var project = new SectionCommittedProjectDTO
                {
                    Id = Guid.NewGuid(),
                    SimulationId = simulationEntity.Id,
                    ScenarioBudgetId = budgetId,
                    LocationKeys = locationInformation,
                    Treatment = worksheet.GetCellValue<string>(row, _keyFields.Count + 1), // Assumes that InitialHeaders stays constant
                    Year = projectYear,
                    ShadowForAnyTreatment = worksheet.GetCellValue<int>(row, _keyFields.Count + 3), // Assumes that InitialHeaders stays constant
                    ShadowForSameTreatment = worksheet.GetCellValue<int>(row, _keyFields.Count + 4), // Assumes that InitialHeaders stays constant
                    Cost = worksheet.GetCellValue<double>(row, _keyFields.Count + 6), // Assumes that InitialHeaders stays constant
                    Category = convertedCategory,
                    Consequences = new List<CommittedProjectConsequenceDTO>()
                };

                if (end.Column > _keyFields.Count + InitialHeaders.Count)
                {
                    // There are consequences in the committed project file - add them to the DTO
                    for (var column = _keyFields.Count + InitialHeaders.Count + 1; column <= end.Column; column++)
                    {
                        project.Consequences.Add(new CommittedProjectConsequenceDTO
                        {
                            Id = Guid.NewGuid(),
                            CommittedProjectId = project.Id,
                            Attribute = worksheet.GetCellValue<string>(1, column),
                            ChangeValue = worksheet.GetCellValue<string>(row, column)
                        });
                    }
                }

                // Add to the list of projects
                projectsPerLocationIdentifierAndYearTuple.Add((locationIdentifier, projectYear), project);
            }

            // Apply required no treatment entries if required by the user
            if (applyNoTreatment && projectsPerLocationIdentifierAndYearTuple.Keys.Any(_ =>
                _.Item2 > simulationEntity.InvestmentPlan.FirstYearOfAnalysisPeriod))
            {
                // Loop through committed projects that do not start in the initial year
                // (Projects in the initial year do not require No Treatment variables)
                var locationIdentifierAndYearTuples = projectsPerLocationIdentifierAndYearTuple.Keys
                    .Where(_ => _.Item2 > simulationEntity.InvestmentPlan.FirstYearOfAnalysisPeriod).ToList();
                locationIdentifierAndYearTuples.ForEach(locationIdentifierAndYearTuple =>
                {
                    var project = projectsPerLocationIdentifierAndYearTuple[locationIdentifierAndYearTuple];

                    // Add no treatment projects for each year from the first year of the analysis to the year
                    // prior to the committed project
                    var year = simulationEntity.InvestmentPlan.FirstYearOfAnalysisPeriod;
                    while (year < project.Year &&
                           !projectsPerLocationIdentifierAndYearTuple.ContainsKey((locationIdentifierAndYearTuple.Item1,
                               year)))
                    {
                        var noTreatmentProjectId = Guid.NewGuid();
                        var noTreatmentProject = new SectionCommittedProjectDTO
                        {
                            Id = noTreatmentProjectId,
                            SimulationId = project.SimulationId,
                            ScenarioBudgetId = null,
                            LocationKeys = project.LocationKeys,
                            Treatment = NoTreatment,
                            Year = year,
                            ShadowForAnyTreatment = 0,
                            ShadowForSameTreatment = 0,
                            Cost = 0,
                            Consequences = project.Consequences.Select(_ =>
                                new CommittedProjectConsequenceDTO
                                {
                                    Id = Guid.NewGuid(),
                                    CommittedProjectId = noTreatmentProjectId,
                                    Attribute = _.Attribute,
                                    ChangeValue = "+0"
                                }).ToList()
                        };

                        projectsPerLocationIdentifierAndYearTuple.Add((locationIdentifierAndYearTuple.Item1, year),
                            noTreatmentProject);

                        year++;
                    }
                });
            }

            return projectsPerLocationIdentifierAndYearTuple.Values.ToList();
        }

        public void ImportCommittedProjectFiles(Guid simulationId, ExcelPackage excelPackage, string filename, bool applyNoTreatment)
        {
            _keyProperties = _unitOfWork.AssetDataRepository.KeyProperties;
            _keyFields = _keyProperties.Keys.Where(_ => _ != "ID").ToList();
            var committedProjectDTOs =
                CreateSectionCommittedProjectsForImport(simulationId, excelPackage, filename, applyNoTreatment);

            _unitOfWork.CommittedProjectRepo.DeleteSimulationCommittedProjects(simulationId);

            _unitOfWork.CommittedProjectRepo.UpsertCommittedProjects(committedProjectDTOs);
        }

        public double GetTreatmentCost(Guid treatmentLibraryId, string assetKeyData, string treatment, Guid networkId)
        {
            var asset = _unitOfWork.MaintainableAssetRepo.GetMaintainableAssetByKeyAttribute(networkId, assetKeyData);
            
            if (asset == null)
                return 0;
            var treatmentCosts = _unitOfWork.Context.SelectableTreatment.AsNoTracking()
                .Include(_ => _.TreatmentCosts)
                .ThenInclude(_ => _.TreatmentCostEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.TreatmentCosts)
                .ThenInclude(_ => _.CriterionLibraryTreatmentCostJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .FirstOrDefault(_ => _.Name == treatment && _.TreatmentLibraryId == treatmentLibraryId)?.TreatmentCosts
                .Where(_ => _.TreatmentCostEquationJoin != null);

            double totalCost = 0;
            if (treatmentCosts == null)
                return totalCost;
            foreach(var cost in treatmentCosts)
            {
                var compiler = new CalculateEvaluateCompiler();

                if (cost.CriterionLibraryTreatmentCostJoin != null && !IsCriteriaValid(compiler, cost.CriterionLibraryTreatmentCostJoin.CriterionLibrary.MergedCriteriaExpression, asset.Id))               
                    continue;
                
                compiler = new CalculateEvaluateCompiler();
                var attributes = InstantiateCompilerAndGetExpressionAttributes(cost.TreatmentCostEquationJoin.Equation.Expression, compiler);

                var aggResults = _unitOfWork.Context.AggregatedResult.AsNoTracking().Include(_ => _.Attribute)
                    .Where(_ => _.MaintainableAssetId == asset.Id).ToList().Where(_ => attributes.Any(a => a.Id == _.AttributeId)).ToList();
                var latestAggResults = new List<AggregatedResultEntity>();
                foreach(var attr in attributes)
                {
                    var attrs = aggResults.Where(_ => _.AttributeId == attr.Id).ToList();
                    if (attrs.Count == 0)
                        continue;
                    var latestYear = attrs.Max(_ => _.Year);
                    var latestAggResult = attrs.FirstOrDefault(_ => _.Year == latestYear);
                    latestAggResults.Add(latestAggResult);
                }                             
                var calculator = compiler.GetCalculator(cost.TreatmentCostEquationJoin.Equation.Expression);
                var scope = new CalculateEvaluateScope();
                if (latestAggResults.Count != attributes.Count)
                    continue;
                InstantiateScope(latestAggResults, scope);
                var currentCost = calculator.Delegate(scope);
                totalCost += currentCost;
            }
            return totalCost;
        }

        public List<CommittedProjectConsequenceDTO> GetValidConsequences(Guid committedProjectId, Guid treatmentLIbraryId, string assetKeyData, string treatment, Guid networkId)
        {
            var consequencesToReturn = new List<CommittedProjectConsequenceDTO>();
            var asset = _unitOfWork.MaintainableAssetRepo.GetMaintainableAssetByKeyAttribute(networkId, assetKeyData);
            if (asset == null)
                return consequencesToReturn;
            var treatmentConsequences = _unitOfWork.Context.SelectableTreatment.AsNoTracking()
                .Include(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.CriterionLibraryConditionalTreatmentConsequenceJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.Attribute)
                .FirstOrDefault(_ => _.Name == treatment && _.TreatmentLibraryId == treatmentLIbraryId)?.TreatmentConsequences.ToList();
            if (treatmentConsequences == null)
                return consequencesToReturn;
            foreach (var consequence in treatmentConsequences)
            {
                var compiler = new CalculateEvaluateCompiler();
                if(consequence.CriterionLibraryConditionalTreatmentConsequenceJoin == null)
                {
                    consequencesToReturn.Add(new CommittedProjectConsequenceDTO() { Id = Guid.NewGuid(), CommittedProjectId = committedProjectId, Attribute = consequence.Attribute.Name, ChangeValue = consequence.ChangeValue });
                    continue;
                }
                if (IsCriteriaValid(compiler, consequence.CriterionLibraryConditionalTreatmentConsequenceJoin.CriterionLibrary.MergedCriteriaExpression, asset.Id))
                    consequencesToReturn.Add(new CommittedProjectConsequenceDTO() { Id = Guid.NewGuid(), CommittedProjectId = committedProjectId, Attribute = consequence.Attribute.Name, ChangeValue = consequence.ChangeValue});
            }
            return consequencesToReturn;
        }

        public PagingPageModel<SectionCommittedProjectDTO> GetCommittedProjectPage(List<SectionCommittedProjectDTO> committedProjects, PagingRequestModel<SectionCommittedProjectDTO> request)
        {
            var skip = 0;
            var take = 0;
            var items = new List<SectionCommittedProjectDTO>();
            var budgetdict = new Dictionary<Guid, string>();
            var totalItems = 0; 

            request.search = request.search == null ? "" : request.search;
            request.sortColumn = request.sortColumn == null ? "" : request.sortColumn;

            

            committedProjects = SyncedDataset(committedProjects, request.PagingSync);
            committedProjects = committedProjects.OrderBy(_ => _.Id).ToList();
            totalItems = committedProjects.Count;

            if (request.search.Trim() != "" || request.sortColumn.Trim() != "")
            {
                var budgetIds = committedProjects.Select(_ => _.ScenarioBudgetId).Distinct();
                budgetdict = _unitOfWork.Context.ScenarioBudget.AsNoTracking().Where(_ => budgetIds.Contains(_.Id)).ToDictionary(_ => _.Id, _ => _.Name);
            }

            if (request.search.Trim() != "")
                committedProjects = SearchCurves(committedProjects, request.search, budgetdict);
            if (request.sortColumn.Trim() != "")
                committedProjects = OrderByColumn(committedProjects, request.sortColumn, request.isDescending, budgetdict);            

            if (request.RowsPerPage > 0)
            {
                take = request.RowsPerPage;
                skip = request.RowsPerPage * (request.Page - 1);
                items = committedProjects.Skip(skip).Take(take).ToList();
            }
            else
            {
                items = committedProjects;
                return new PagingPageModel<SectionCommittedProjectDTO>()
                {
                    Items = items,
                    TotalItems = items.Count
                };
            }

            return new PagingPageModel<SectionCommittedProjectDTO>()
            {
                Items = items,
                TotalItems = totalItems
            };
        }

        public List<SectionCommittedProjectDTO> GetSyncedDataset(Guid simulationId, PagingSyncModel<SectionCommittedProjectDTO> request)
        {
            var committedProjects = _unitOfWork.CommittedProjectRepo.GetSectionCommittedProjectDTOs(simulationId);
            return SyncedDataset(committedProjects, request);
        }


        private List<SectionCommittedProjectDTO> OrderByColumn(List<SectionCommittedProjectDTO> committedProjects,
            string sortColumn,
            bool isDescending,
            Dictionary<Guid, string> budgetDict)
        {
            sortColumn = sortColumn?.ToLower();
            switch (sortColumn)
            {
            case "brkey":
                if (isDescending)
                    return committedProjects.OrderByDescending(_ => _.LocationKeys[_networkKeyField], new AlphanumericComparator()).ToList();
                else
                    return committedProjects.OrderBy(_ => _.LocationKeys[_networkKeyField], new AlphanumericComparator()).ToList();
            case "year":
                if (isDescending)
                    return committedProjects.OrderByDescending(_ => _.Year).ToList();
                else
                    return committedProjects.OrderBy(_ => _.Year).ToList();
            case "treatment":
                if(isDescending)
                    return  committedProjects.OrderByDescending(_ => _.Treatment.ToLower()).ToList();
                else
                    return committedProjects.OrderBy(_ => _.Treatment.ToLower()).ToList();
            case "category":
                if (isDescending)
                    return committedProjects.OrderByDescending(_ => _.Category.ToString().ToLower()).ToList();
                else
                    return committedProjects.OrderBy(_ => _.Category.ToString().ToLower()).ToList();
            case "budget":
                if (isDescending)
                    return committedProjects.OrderByDescending(_ => _.ScenarioBudgetId == null ? "" : budgetDict[_.ScenarioBudgetId.Value].ToLower()).ToList();
                else
                    return committedProjects.OrderBy(_ => _.Category.ToString().ToLower()).ToList();
            case "cost":
                if (isDescending)
                    return committedProjects.OrderByDescending(_ => _.Cost).ToList();
                else
                    return committedProjects.OrderBy(_ => _.Cost).ToList();
            }
            return committedProjects;
        }

        private bool IsCriteriaValid(CalculateEvaluateCompiler compiler, string expression, Guid assetId)
        {
            var attributes = InstantiateCompilerAndGetExpressionAttributes(expression, compiler);

            var aggResults = _unitOfWork.Context.AggregatedResult.AsNoTracking().Include(_ => _.Attribute)
                    .Where(_ => _.MaintainableAssetId == assetId).ToList().Where(_ => attributes.Any(a => a.Id == _.AttributeId)).ToList();
            var latestAggResults = new List<AggregatedResultEntity>();
            foreach (var attr in attributes)
            {
                var attrs = aggResults.Where(_ => _.AttributeId == attr.Id).ToList();
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

        private List<SectionCommittedProjectDTO> SearchCurves(List<SectionCommittedProjectDTO> committedProjects,
            string search,
            Dictionary<Guid, string> budgetDict)
        {
            search = search.ToLower();
            return committedProjects
                .Where(_ => _.LocationKeys[_networkKeyField].ToLower().Contains(search) ||
                    _.Year.ToString().Contains(search) ||
                    _.Treatment.ToLower().Contains(search) ||
                    _.Category.ToString().ToLower().Contains(search) ||
                    (_.ScenarioBudgetId == null ? "" : budgetDict[_.ScenarioBudgetId.Value]).Contains(search) ||
                    _.Cost.ToString().Contains(search)).ToList();
        }

        private List<SectionCommittedProjectDTO> SyncedDataset(List<SectionCommittedProjectDTO> committedProjects, PagingSyncModel<SectionCommittedProjectDTO> request)
        {
            committedProjects = committedProjects.Concat(request.AddedRows).Where(_ => !request.RowsForDeletion.Contains(_.Id)).ToList();

            for (var i = 0; i < committedProjects.Count; i++)
            {
                var item = request.UpdateRows.FirstOrDefault(row => row.Id == committedProjects[i].Id);
                if (item != null)
                    committedProjects[i] = item;
            }

            return committedProjects;
        }

        private List<AttributeEntity> InstantiateCompilerAndGetExpressionAttributes(string mergedCriteriaExpression, CalculateEvaluateCompiler compiler)
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
            var hashMatch = new HashSet<string>();
            foreach (Match m in match)
            {
                hashMatch.Add(m.Value.Substring(1, m.Value.Length - 2));
            }

            var attributes = _unitOfWork.Context.Attribute.AsNoTracking()
                .Where(_ => hashMatch.Contains(_.Name))
                .Select(attribute => new AttributeEntity
                {
                    Id = attribute.Id,
                    Name = attribute.Name,
                    DataType = attribute.DataType
                }).AsNoTracking().ToList();

            attributes.ForEach(attribute =>
            {
                compiler.ParameterTypes[attribute.Name] = attribute.DataType == "NUMBER"
                    ? CalculateEvaluateParameterType.Number
                    : CalculateEvaluateParameterType.Text;
            });

            return attributes;
        }

        private void InstantiateScope(List<AggregatedResultEntity> results, CalculateEvaluateScope scope)
        {
            results.ForEach(_ =>
            {
                if (_.Attribute.DataType == "NUMBER")
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
