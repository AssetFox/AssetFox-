using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using OfficeOpenXml;

namespace BridgeCareCore.Services
{
    public class CommittedProjectService : ICommittedProjectService
    {
        private static UnitOfDataPersistenceWork _unitOfWork;

        private static readonly List<string> InitialHeaders = new List<string>
        {
            "BRKEY","BMSID","TREATMENT","YEAR","YEARANY","YEARSAME","BUDGET","COST","AREA"
        };

        private static readonly string NoTreatment = "No Treatment";

        public CommittedProjectService(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        /**
         * Adds excel worksheet header row cell values for Committed Project Export
         */
        private void AddHeaderCells(ExcelWorksheet worksheet, List<string> attributeNames)
        {
            for (var column = 0; column < InitialHeaders.Count; column++)
            {
                worksheet.Cells[1, column + 1].Value = InitialHeaders[column];
            }

            var attributeColumn = InitialHeaders.Count;
            attributeNames.ForEach(attributeName => worksheet.Cells[1, ++attributeColumn].Value = attributeName);
        }

        /**
         * Adds excel worksheet cell values for Committed Project Export
         */
        private void AddDataCells(ExcelWorksheet worksheet, List<CommittedProjectEntity> committedProjectEntities)
        {
            var row = 2;
            committedProjectEntities.OrderBy(_ => _.MaintainableAsset.MaintainableAssetLocation.LocationIdentifier)
                .ThenByDescending(_ => _.Year).ForEach(
                    project =>
                    {
                        var column = 1;
                        var brKeyBmsIdSplit =
                            project.MaintainableAsset.MaintainableAssetLocation.LocationIdentifier.Split('-');
                        worksheet.Cells[row, column++].Value = brKeyBmsIdSplit[0];
                        worksheet.Cells[row, column++].Value = brKeyBmsIdSplit[1];
                        worksheet.Cells[row, column++].Value = project.Name;
                        worksheet.Cells[row, column++].Value = project.Year;
                        worksheet.Cells[row, column++].Value = project.ShadowForAnyTreatment;
                        worksheet.Cells[row, column++].Value = project.ShadowForSameTreatment;
                        worksheet.Cells[row, column++].Value = project.Budget.Name;
                        worksheet.Cells[row, column++].Value = project.Cost;
                        worksheet.Cells[row, column++].Value = string.Empty; // AREA
                        project.CommittedProjectConsequences.OrderBy(_ => _.Attribute.Name).ForEach(consequence =>
                        {
                            worksheet.Cells[row, column++].Value = consequence.ChangeValue;
                        });
                        row++;
                    });
        }

        public FileInfoDTO ExportCommittedProjectsFile(Guid simulationId)
        {
            var committedProjectEntities = _unitOfWork.CommittedProjectRepo.GetCommittedProjectsForExport(simulationId);

            var simulationEntity = _unitOfWork.Context.Simulation.Where(_ => _.Id == simulationId)
                .Select(simulation => new SimulationEntity {Name = simulation.Name}).Single();
            var fileName = $"CommittedProjects_{simulationEntity.Name.Trim().Replace(" ", "_")}.xlsx";

            using var excelPackage = new ExcelPackage(new FileInfo(fileName));

            var worksheet = excelPackage.Workbook.Worksheets.Add("Committed Projects");

            if (committedProjectEntities.Any())
            {
                var attributeNames = committedProjectEntities
                    .SelectMany(_ =>
                        _.CommittedProjectConsequences.Select(__ => __.Attribute.Name).Distinct().OrderBy(__ => __))
                    .Distinct()
                    .ToList();
                AddHeaderCells(worksheet, attributeNames);
                AddDataCells(worksheet, committedProjectEntities);
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
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            return _unitOfWork.Context.Simulation
                .Include(_ => _.InvestmentPlan)
                .Include(_ => _.BudgetLibrarySimulationJoin)
                .ThenInclude(_ => _.BudgetLibrary)
                .ThenInclude(_ => _.Budgets)
                .ThenInclude(_ => _.BudgetAmounts)
                .Single(_ => _.Id == simulationId);
        }

        /**
         * Validates a SimulationEntity's data for a Committed Project Import
         */
        private void ValidateSimulationEntityForCommittedProjectImport(SimulationEntity simulationEntity)
        {
            if (simulationEntity.InvestmentPlan == null)
            {
                throw new RowNotInTableException("Simulation has no investment plan.");
            }

            if (simulationEntity.BudgetLibrarySimulationJoin == null)
            {
                throw new RowNotInTableException("Simulation has no applied budget library.");
            }

            if (!simulationEntity.BudgetLibrarySimulationJoin.BudgetLibrary.Budgets.Any())
            {
                throw new RowNotInTableException("Simulation applied budget library has no budgets.");
            }
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
        private Dictionary<string, Guid> GetMaintainableAssetsPerLocationIdentifier(List<string> locationIdentifiers)
        {
            if (!_unitOfWork.Context.MaintainableAsset.Any())
            {
                throw new RowNotInTableException("There are no maintainable assets in the database.");
            }

            return _unitOfWork.Context.MaintainableAssetLocation
                .Where(_ => locationIdentifiers.Contains(_.LocationIdentifier))
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
         * Gets the value of an excel worksheet cell
         */
        private string GetCellValue(ExcelWorksheet worksheet, int row, int column) =>
            worksheet.Cells[row, column].GetValue<string>().Trim();

        /**
         * Creates CommittedProjectEntity data for Committed Project Import
         */
        private List<CommittedProjectEntity> CreateCommittedProjectEntitiesForImport(Guid simulationId,
            List<ExcelPackage> excelPackages, bool applyNoTreatment)
        {
            var simulationEntity = GetSimulationEntityForCommittedProjectImport(simulationId);
            ValidateSimulationEntityForCommittedProjectImport(simulationEntity);

            var locationIdentifiers = new List<string>();
            var consequenceAttributeNames = new List<string>();

            excelPackages.ForEach(excelPackage =>
            {
                var worksheet = excelPackage.Workbook.Worksheets[0];

                var headers = worksheet.Cells.GroupBy(cell => cell.Start.Row).First().Select(_ => _.GetValue<string>()).ToList();
                consequenceAttributeNames.AddRange(headers.Skip(9));

                var end = worksheet.Dimension.End;
                for (var row = 2; row <= end.Row; row++)
                {
                    var brKey = GetCellValue(worksheet, row, 1);
                    var bmsId = GetCellValue(worksheet, row, 2);
                    var locationIdentifier = $"{brKey}-{bmsId}";
                    if (!locationIdentifiers.Contains(locationIdentifier))
                    {
                        locationIdentifiers.Add(locationIdentifier);
                    }
                }
            });

            var attributeIdsPerAttributeName =
                GetAttributeIdsPerAttributeName(consequenceAttributeNames.Distinct().ToList());

            var maintainableAssetIdsPerLocationIdentifier =
                GetMaintainableAssetsPerLocationIdentifier(locationIdentifiers);

            return excelPackages.SelectMany(excelPackage =>
            {
                var worksheet = excelPackage.Workbook.Worksheets[0];
                var end = worksheet.Dimension.End;

                var projects = new List<CommittedProjectEntity>();
                var projectUniqueIdentifierTuples = new List<(string, string, int, string)>();

                for (var row = 2; row <= end.Row; row++)
                {
                    var brKey = GetCellValue(worksheet, row, 1);
                    var bmsId = GetCellValue(worksheet, row, 2);
                    var locationIdentifier = $"{brKey}-{bmsId}";

                    var projectName = GetCellValue(worksheet, row, 3);

                    var projectYear = int.Parse(GetCellValue(worksheet, row, 4));

                    var budgetName = GetCellValue(worksheet, row, 7);

                    var projectUniqueIdentifierTuple = (locationIdentifier, projectName, projectYear, budgetName);

                    if (projectUniqueIdentifierTuples.Contains(projectUniqueIdentifierTuple))
                    {
                        continue;
                    }

                    if (simulationEntity.BudgetLibrarySimulationJoin.BudgetLibrary.Budgets.All(
                        _ => _.Name != budgetName))
                    {
                        throw new RowNotInTableException(
                            $"Budget {budgetName} does not exist in the applied budget library.");
                    }

                    var budgetEntity = simulationEntity.BudgetLibrarySimulationJoin.BudgetLibrary.Budgets.Single(
                        _ => _.Name == budgetName);

                    var maintainableAssetId = maintainableAssetIdsPerLocationIdentifier[locationIdentifier];

                    var project = new CommittedProjectEntity
                    {
                        Id = Guid.NewGuid(),
                        SimulationId = simulationEntity.Id,
                        BudgetId = budgetEntity.Id,
                        MaintainableAssetId = maintainableAssetId,
                        Name = projectName,
                        Year = projectYear,
                        ShadowForAnyTreatment = int.Parse(GetCellValue(worksheet, row, 5)),
                        ShadowForSameTreatment = int.Parse(GetCellValue(worksheet, row, 6)),
                        Cost = double.Parse(GetCellValue(worksheet, row, 8)),
                        CommittedProjectConsequences = new List<CommittedProjectConsequenceEntity>()
                    };
                    projects.Add(project);
                    projectUniqueIdentifierTuples.Add(projectUniqueIdentifierTuple);

                    if (end.Column >= 10)
                    {
                        for (var column = 10; column <= end.Column; column++)
                        {
                            project.CommittedProjectConsequences.Add(new CommittedProjectConsequenceEntity
                            {
                                Id = Guid.NewGuid(),
                                CommittedProjectId = project.Id,
                                AttributeId = attributeIdsPerAttributeName[GetCellValue(worksheet, 1, column)],
                                ChangeValue = GetCellValue(worksheet, row, column)
                            });
                        }

                        if (applyNoTreatment && simulationEntity.InvestmentPlan.FirstYearOfAnalysisPeriod < project.Year)
                        {
                            var year = simulationEntity.InvestmentPlan.FirstYearOfAnalysisPeriod;
                            while (year < project.Year)
                            {
                                var noTreatmentProjectId = Guid.NewGuid();
                                projects.Add(new CommittedProjectEntity
                                {
                                    Id = noTreatmentProjectId,
                                    SimulationId = project.SimulationId,
                                    BudgetId = project.BudgetId,
                                    MaintainableAssetId = project.MaintainableAssetId,
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
                                });

                                var noTreatmentProjectUniqueIdentifierTuple = (locationIdentifier, NoTreatment, year, budgetName);
                                projectUniqueIdentifierTuples.Add(noTreatmentProjectUniqueIdentifierTuple);

                                year++;
                            }
                        }
                    }
                }
                return projects;
            }).ToList();
        }

        public void ImportCommittedProjectFiles(Guid simulationId, List<ExcelPackage> excelPackages, bool applyNoTreatment)
        {
            var committedProjectEntities =
                CreateCommittedProjectEntitiesForImport(simulationId, excelPackages, applyNoTreatment);

            _unitOfWork.CommittedProjectRepo.DeleteCommittedProjects(simulationId);

            _unitOfWork.CommittedProjectRepo.CreateCommittedProjects(committedProjectEntities);
        }
    }
}
