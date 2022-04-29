using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Utils;
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
            "BRKEY",
            "BMSID",
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
                        worksheet.Cells[row, column++].Value = project.ScenarioBudget.Name;
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
        private List<CommittedProjectEntity> CreateCommittedProjectEntitiesForImport(Guid simulationId,
            ExcelPackage excelPackage, bool applyNoTreatment)
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
                var brKey = worksheet.GetCellValue<string>(row, 1);
                var bmsId = worksheet.GetCellValue<string>(row, 2);
                var locationIdentifier = $"{brKey}-{bmsId}";

                var projectYear = worksheet.GetCellValue<int>(row, 4);

                if (projectsPerLocationIdentifierAndYearTuple.ContainsKey((locationIdentifier, projectYear)))
                {
                    continue;
                }

                var budgetName = worksheet.GetCellValue<string>(row, 7);

                if (simulationEntity.Budgets.All(_ => _.Name != budgetName))
                {
                    throw new RowNotInTableException(
                        $"Budget {budgetName} does not exist in the applied budget library.");
                }

                var budgetEntity = simulationEntity.Budgets.Single(_ => _.Name == budgetName);

                var project = new CommittedProjectEntity
                {
                    Id = Guid.NewGuid(),
                    SimulationId = simulationEntity.Id,
                    ScenarioBudgetId = budgetEntity.Id,
                    MaintainableAssetId = maintainableAssetIdsPerLocationIdentifier[locationIdentifier],
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
                        };

                        projectsPerLocationIdentifierAndYearTuple.Add((locationIdentifierAndYearTuple.Item1, year),
                            noTreatmentProject);

                        year++;
                    }
                });
            }

            return projectsPerLocationIdentifierAndYearTuple.Values.ToList();
        }

        public void ImportCommittedProjectFiles(Guid simulationId, ExcelPackage excelPackage, bool applyNoTreatment)
        {
            var committedProjectEntities =
                CreateCommittedProjectEntitiesForImport(simulationId, excelPackage, applyNoTreatment);

            _unitOfWork.CommittedProjectRepo.DeleteCommittedProjects(simulationId);

            _unitOfWork.CommittedProjectRepo.CreateCommittedProjects(committedProjectEntities);
        }
    }
}
