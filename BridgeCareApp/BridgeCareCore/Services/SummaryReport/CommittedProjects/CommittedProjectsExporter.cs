using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.CommittedProjects
{
    public class CommittedProjectsExporter
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _networkKeyField;
        private readonly Dictionary<string, List<KeySegmentDatum>> _keyProperties;
        private readonly List<string> _keyFields;
        private readonly SimulationDTO _simulation;
        private readonly IList<string> _primaryKeyFieldNames;


        private const string UnknownBudgetName = "Unknown";

        public CommittedProjectsExporter(
            IUnitOfWork unitOfWork,
            SimulationDTO simulation,
            string networkKeyField,
            List<string> keyFields,
            Dictionary<string, List<KeySegmentDatum>> keyProperties,
            List<string> primaryKeyFieldNames)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _simulation = simulation;
            _networkKeyField = networkKeyField;
            _keyFields = keyFields;
            _keyProperties = keyProperties;
            _primaryKeyFieldNames = primaryKeyFieldNames;

            // Filter key properties to include only primary key fields and the network key field
            foreach (var kvp in _keyProperties.ToList())
            {
                var key = kvp.Key;
                if (!primaryKeyFieldNames.Contains(key) && key != _networkKeyField)
                {
                    _keyProperties.Remove(key);
                }
            }

            _keyFields = _keyProperties.Keys.Where(_ => _ != "ID").ToList();
        }

        // ColumnDefinition class to hold header and data retrieval logic
        private class ColumnDefinition
        {
            public string Header { get; set; }
            public Func<BaseCommittedProjectDTO, object> GetValue { get; set; }
        }

        // Method to create the list of column definitions
        private List<ColumnDefinition> GetColumnDefinitions(Guid simulationId)
        {
            var columns = new List<ColumnDefinition>();

            // Key fields
            foreach (var keyField in _keyFields)
            {
                var field = keyField; // Capture loop variable
                columns.Add(new ColumnDefinition
                {
                    Header = field,
                    GetValue = (project) =>
                    {
                        if (project.LocationKeys.ContainsKey(field))
                        {
                            return project.LocationKeys[field];
                        }
                        else
                        {
                            // Retrieve other data from key properties based on asset ID
                            var assetId = _keyProperties[_networkKeyField]
                                .FirstOrDefault(_ => _.KeyValue.Value == project.LocationKeys[_networkKeyField])?.AssetId;

                            if (assetId != null)
                            {
                                return _keyProperties[field]
                                    .FirstOrDefault(_ => _.AssetId == assetId)?.KeyValue.Value ?? "";
                            }
                            return "";
                        }
                    }
                });
            }

            // Other columns
            columns.Add(new ColumnDefinition { Header = "TREATMENT", GetValue = (project) => project.Treatment });
            columns.Add(new ColumnDefinition { Header = "YEAR", GetValue = (project) => project.Year });
            columns.Add(new ColumnDefinition { Header = "BUDGET", GetValue = (project) => GetBudgetName(project, simulationId) });
            columns.Add(new ColumnDefinition { Header = "COST", GetValue = (project) => project.Cost });
            columns.Add(new ColumnDefinition { Header = "PROJECTSOURCE", GetValue = (project) => project.ProjectSource });
            columns.Add(new ColumnDefinition { Header = "PROJECTSOURCEID", GetValue = (project) => project.ProjectId });
            columns.Add(new ColumnDefinition { Header = "CATEGORY", GetValue = (project) => project.Category.ToString() });

            return columns;
        }

        // Helper method to get the budget name
        private string GetBudgetName(BaseCommittedProjectDTO project, Guid simulationId)
        {
            var linkedBudget = _unitOfWork.BudgetRepo.GetScenarioBudgets(simulationId)
                .FirstOrDefault(_ => _.Id == project.ScenarioBudgetId);
            var budgetName = linkedBudget?.Name ?? UnknownBudgetName;
            return budgetName == UnknownBudgetName ? "" : budgetName;
        }

        // Method to add header cells to the worksheet
        private static void AddHeaderCells(ExcelWorksheet worksheet, List<ColumnDefinition> columns)
        {
            var column = 1;
            foreach (var colDef in columns)
            {
                worksheet.Cells[1, column++].Value = colDef.Header;
            }
        }

        // Method to add data cells to the worksheet
        private void AddDataCells(ExcelWorksheet worksheet, List<BaseCommittedProjectDTO> committedProjectDTOs, List<ColumnDefinition> columns)
        {
            var row = 2;
            foreach (var project in committedProjectDTOs
                .OrderBy(_ => _.LocationKeys[_networkKeyField])
                .ThenByDescending(_ => _.Year))
            {
                var column = 1;
                foreach (var colDef in columns)
                {
                    worksheet.Cells[row, column++].Value = colDef.GetValue(project);
                }
                row++;
            }
        }

        // Public method to export committed projects to an Excel file
        public FileInfoDTO ExportCommittedProjectsFile(Guid simulationId)
        {
            var simulation = _unitOfWork.SimulationRepo.GetSimulation(simulationId);
            var simulationName = simulation.Name;

            var committedProjectDTOs = _unitOfWork.CommittedProjectRepo.GetCommittedProjectsForExport(simulationId);

            var fileName = $"CommittedProjects_{simulationName.Trim().Replace(" ", "_")}.xlsx";

            using var excelPackage = new ExcelPackage();

            var worksheet = excelPackage.Workbook.Worksheets.Add("Committed Projects");

            var columns = GetColumnDefinitions(simulationId);

            if (committedProjectDTOs.Any())
            {
                AddHeaderCells(worksheet, columns);
                AddDataCells(worksheet, committedProjectDTOs, columns);
            }
            else
            {
                // Add headers even if there are no data rows
                AddHeaderCells(worksheet, columns);
            }

            return new FileInfoDTO
            {
                FileName = fileName,
                FileData = Convert.ToBase64String(excelPackage.GetAsByteArray()),
                MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
        }
    }
}
