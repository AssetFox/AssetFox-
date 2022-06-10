using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Services.CommittedProjects;
using BridgeCareCore.Utils;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using OfficeOpenXml;

namespace BridgeCareCore.Services
{
    public class CommittedProjectService : ICommittedProjectService
    {
        private static UnitOfDataPersistenceWork _unitOfWork;

        public const string UnknownBudgetName = "Unknown";

        // TODO: Determine based on associated network
        private readonly string _networkKeyField = "BRKEY";

        private readonly List<string> _keyFields = _unitOfWork.AssetDataRepository.KeyProperties.Keys.ToList();

        private static readonly List<string> InitialHeaders = new List<string>
        {
            "TREATMENT",
            "YEAR",
            "YEARANY",
            "YEARSAME",
            "BUDGET",
            "COST",
            "AREA"
        };

        private static readonly string NoTreatment = "No Treatment";

        public CommittedProjectService(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        /**
         * Adds excel worksheet header row cell values for Committed Project Export
         */
        private void AddHeaderCells(ExcelWorksheet worksheet, List<string> attributeNames)
        {
            for (var keyColumnIndex = 0; keyColumnIndex < _keyFields.Count; keyColumnIndex++)
            {
                worksheet.Cells[1, keyColumnIndex + 1].Value = _keyFields[keyColumnIndex];
            }

            for (var column = 0; column < InitialHeaders.Count; column++)
            {
                worksheet.Cells[1, column + _keyFields.Count + 1].Value = InitialHeaders[column];
            }

            var attributeColumn = InitialHeaders.Count + _keyFields.Count;
            attributeNames.ForEach(attributeName => worksheet.Cells[1, ++attributeColumn].Value = attributeName);
        }

        /**
         * Adds excel worksheet cell values for Committed Project Export
         */
        private void AddDataCells(ExcelWorksheet worksheet, List<BaseCommittedProjectDTO> committedProjectDTOs)
        {
            var row = 2;
            committedProjectDTOs.OrderBy(_ => _.LocationKeys[_networkKeyField])
                .ThenByDescending(_ => _.Year).ForEach(
                    project =>
                    {
                        var column = 1;

                        foreach (var pair in project.LocationKeys)
                        {
                            if (_keyFields.Contains(pair.Key))
                            {
                                worksheet.Cells[row, column++].Value = pair.Value;
                            }
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
                        project.Consequences.OrderBy(_ => _.Attribute).ForEach(consequence =>
                        {
                            worksheet.Cells[row, column++].Value = consequence.ChangeValue;
                        });
                        row++;
                    });
        }

        public FileInfoDTO ExportCommittedProjectsFile(Guid simulationId)
        {
            var committedProjectDTOs = _unitOfWork.CommittedProjectRepo.GetCommittedProjectsForExport(simulationId);
            //var sectionCommittedProjects = committedProjectDTOs.Where(_ => _ is SectionCommittedProjectDTO).Select(_ => (SectionCommittedProjectDTO)_).ToList();

            var simulationEntity = _unitOfWork.Context.Simulation.Where(_ => _.Id == simulationId)
                .Select(simulation => new SimulationEntity { Name = simulation.Name }).Single();
            var fileName = $"CommittedProjects_{simulationEntity.Name.Trim().Replace(" ", "_")}.xlsx";

            using var excelPackage = new ExcelPackage(new FileInfo(fileName));

            var worksheet = excelPackage.Workbook.Worksheets.Add("Committed Projects");

            if (committedProjectDTOs.Any())
            {
                var attributeNames = committedProjectDTOs
                    .SelectMany(_ =>
                        _.Consequences.Select(__ => __.Attribute).Distinct().OrderBy(__ => __))
                    .Distinct()
                    .ToList();
                AddHeaderCells(worksheet, attributeNames);
                AddDataCells(worksheet, committedProjectDTOs);
            }

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
                throw new RowNotInTableException($"No simulation was found for the given scenario.");
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
            var attributeEntities = _unitOfWork.Context.Attribute.ToList();
            var attributeNames = attributeEntities.Select(_ => _.Name).ToList();

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

            return attributeEntities.ToDictionary(_ => _.Name, _ => _.Id);
        }

        /**
         * Gets a Dictionary of MaintainableAssetEntity Id per MaintainableAssetLocationEntity LocationIdentifier
         */
        private Dictionary<string, Guid> GetMaintainableAssetsPerLocationIdentifier(Guid networkId)
        {
            if (!_unitOfWork.Context.MaintainableAsset.Any())
            {
                throw new RowNotInTableException("There are no maintainable assets in the database.");
            }

            var maintainableAssetsInNetwork = _unitOfWork.Context.MaintainableAsset
                .Where(_ => _.NetworkId == networkId);

            return _unitOfWork.Context.MaintainableAssetLocation
                .Where(_ => maintainableAssetsInNetwork.Any(__ => __.Id == _.MaintainableAssetId))
                .Select(maintainableAssetLocation => new MaintainableAssetEntity
                {
                    Id = maintainableAssetLocation.MaintainableAssetId,
                    MaintainableAssetLocation = new MaintainableAssetLocationEntity
                    {
                        LocationIdentifier = maintainableAssetLocation.LocationIdentifier
                    }
                }).ToDictionary(_ => _.MaintainableAssetLocation.LocationIdentifier, _ => _.Id);
        }

        /**
         * Creates CommittedProjectEntity data for Committed Project Import
         */
        private List<SectionCommittedProjectDTO> CreateSectionCommittedProjectsForImport(Guid simulationId,
            ExcelPackage excelPackage, string filename, bool applyNoTreatment)
        {
            var simulationEntity = GetSimulationEntityForCommittedProjectImport(simulationId);

            if (simulationEntity.InvestmentPlan == null)
            {
                throw new RowNotInTableException("Simulation has no investment plan.");
            }

            if (!simulationEntity.Budgets.Any())
            {
                throw new RowNotInTableException("Simulation has no budgets.");
            }

            var networkId = simulationEntity.NetworkId;
            var worksheet = excelPackage.Workbook.Worksheets[0];
            var end = worksheet.Dimension.End;

            var headers = worksheet.Cells.GroupBy(cell => cell.Start.Row).First().Select(_ => _.GetValue<string>())
                .ToList();
            var consequenceAttributeNames = headers.Skip(9).ToList();

            var attributeIdsPerAttributeName =
                GetAttributeIdsPerAttributeName(consequenceAttributeNames.Distinct().ToList());

            var maintainableAssetIdsPerLocationIdentifier = GetMaintainableAssetsPerLocationIdentifier(networkId);

            var projectsPerLocationIdentifierAndYearTuple = new Dictionary<(string, int), CommittedProjectEntity>();

            for (var row = 2; row <= end.Row; row++)
            {
                //var brKey = worksheet.GetCellValue<string>(row, 1);
                //var bmsId = worksheet.GetCellValue<string>(row, 2);
                var projectYear = worksheet.GetCellValue<int>(row, 4);
                var searchList = maintainableAssetIdsPerLocationIdentifier.Keys.ToList();
                var locationSearchResult = LocationMatchFinder.FindUniqueMatch(searchList, brKey, bmsId);
                var fileString = string.IsNullOrEmpty(filename) ? "" : @$" from file ""{filename}""";
                if (locationSearchResult.Message != null)
                {
                    var exceptionMessage = $"Error importing committed projects{fileString}. Row {row}: {locationSearchResult.Message}";
                    throw new Exception(exceptionMessage);
                }
                var locationIdentifier = locationSearchResult.LocationIdentifier;

                if (projectsPerLocationIdentifierAndYearTuple.ContainsKey((locationIdentifier, projectYear)))
                {
                    continue;
                }

                var budgetName = worksheet.GetCellValue<string>(row, 7);
                var budgetNameIsEmpty = string.IsNullOrWhiteSpace(budgetName);
                ScenarioBudgetEntity budgetEntity = null;

                if (budgetNameIsEmpty)
                {
                    budgetEntity = _unitOfWork.BudgetRepo.EnsureExistenceOfUnknownBudgetForSimulation(simulationId);
                }
                else 
                {
                    if (simulationEntity.Budgets.All(_ => _.Name != budgetName))
                    {
                        throw new RowNotInTableException(
                            $"Budget {budgetName} does not exist in the applied budget library.");
                    }
                    budgetEntity = simulationEntity.Budgets.Single(_ => _.Name == budgetName);
                } 

                var project = new CommittedProjectEntity
                {
                    Id = Guid.NewGuid(),
                    SimulationId = simulationEntity.Id,
                    ScenarioBudgetId = budgetEntity.Id,
                    CommittedProjectLocation = new CommittedProjectLocationEntity(Guid.NewGuid(), DataPersistenceConstants.SectionLocation, locationIdentifier),
                    Name = worksheet.GetCellValue<string>(row, 3),
                    Year = projectYear,
                    ShadowForAnyTreatment = worksheet.GetCellValue<int>(row, 5),
                    ShadowForSameTreatment = worksheet.GetCellValue<int>(row, 6),
                    Cost = worksheet.GetCellValue<double>(row, 8),
                    CommittedProjectConsequences = new List<CommittedProjectConsequenceEntity>()
                };

                if (end.Column >= 10)
                {
                    for (var column = 10; column <= end.Column; column++)
                    {
                        project.CommittedProjectConsequences.Add(new CommittedProjectConsequenceEntity
                        {
                            Id = Guid.NewGuid(),
                            CommittedProjectId = project.Id,
                            AttributeId = attributeIdsPerAttributeName[worksheet.GetCellValue<string>(1, column)],
                            ChangeValue = worksheet.GetCellValue<string>(row, column)
                        });
                    }
                }

                projectsPerLocationIdentifierAndYearTuple.Add((locationIdentifier, projectYear), project);
            }

            if (applyNoTreatment && projectsPerLocationIdentifierAndYearTuple.Keys.Any(_ =>
                _.Item2 > simulationEntity.InvestmentPlan.FirstYearOfAnalysisPeriod))
            {
                var locationIdentifierAndYearTuples = projectsPerLocationIdentifierAndYearTuple.Keys
                    .Where(_ => _.Item2 > simulationEntity.InvestmentPlan.FirstYearOfAnalysisPeriod).ToList();
                locationIdentifierAndYearTuples.ForEach(locationIdentifierAndYearTuple =>
                {
                    var project = projectsPerLocationIdentifierAndYearTuple[locationIdentifierAndYearTuple];
                    var year = simulationEntity.InvestmentPlan.FirstYearOfAnalysisPeriod;
                    while (year < project.Year &&
                           !projectsPerLocationIdentifierAndYearTuple.ContainsKey((locationIdentifierAndYearTuple.Item1,
                               year)))
                    {
                        var noTreatmentProjectId = Guid.NewGuid();
                        var noTreatmentProject = new CommittedProjectEntity
                        {
                            Id = noTreatmentProjectId,
                            SimulationId = project.SimulationId,
                            ScenarioBudgetId = project.ScenarioBudgetId,
                            CommittedProjectLocation = new CommittedProjectLocationEntity(Guid.NewGuid(), DataPersistenceConstants.SectionLocation, locationIdentifierAndYearTuple.Item1),
                            Name = NoTreatment,
                            Year = year,
                            ShadowForAnyTreatment = project.ShadowForAnyTreatment,
                            ShadowForSameTreatment = project.ShadowForSameTreatment,
                            Cost = 0,
                            CommittedProjectConsequences = project.CommittedProjectConsequences.Select(_ =>
                                new CommittedProjectConsequenceEntity
                                {
                                    Id = Guid.NewGuid(),
                                    CommittedProjectId = noTreatmentProjectId,
                                    AttributeId = _.AttributeId,
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
            var committedProjectEntities =
                CreateSectionCommittedProjectsForImport(simulationId, excelPackage, filename, applyNoTreatment);

            _unitOfWork.CommittedProjectRepo.DeleteCommittedProjects(simulationId);

            _unitOfWork.CommittedProjectRepo.CreateCommittedProjects(committedProjectEntities.Select(_ => (BaseCommittedProjectDTO)_).ToList());
        }
    }
}
