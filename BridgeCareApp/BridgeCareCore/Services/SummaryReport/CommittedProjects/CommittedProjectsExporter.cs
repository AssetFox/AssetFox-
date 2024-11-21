using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;

namespace BridgeCareCore.Services.SummaryReport.CommittedProjects
{
    public class CommittedProjectsExporter
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _networkKeyField;
        private readonly Dictionary<string, List<KeySegmentDatum>> _keyProperties;
        private readonly List<string> _keyFields;
        private readonly IList<string> _primaryKeyFieldNames;

        public CommittedProjectsExporter(
            IUnitOfWork unitOfWork,
            SimulationDTO simulation,
            string networkKeyField,
            List<string> keyFields,
            Dictionary<string, List<KeySegmentDatum>> keyProperties,
            List<string> primaryKeyFieldNames)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _networkKeyField = networkKeyField;
            _primaryKeyFieldNames = primaryKeyFieldNames;

            // Filter key properties
            _keyProperties = keyProperties
                .Where(kvp => _primaryKeyFieldNames.Contains(kvp.Key) || kvp.Key == _networkKeyField)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            _keyFields = _keyProperties.Keys.Where(key => key != "ID").ToList();
        }

        private class ColumnDefinition
        {
            public string Header { get; set; }
            public Func<BaseCommittedProjectDTO, object> GetValue { get; set; }
        }

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
                    GetValue = project => GetProjectKeyValue(project, field)
                });
            }

            // Other columns from the NoKeyHeaders list
            foreach (var header in CommittedProjectsColumnHeaders.NoKeyHeaders)
            {
                columns.Add(new ColumnDefinition
                {
                    Header = header,
                    GetValue = project => GetProjectValue(project, header)
                });
            }

            return columns;
        }

        private object GetProjectKeyValue(BaseCommittedProjectDTO project, string field)
        {
            if (project.LocationKeys.TryGetValue(field, out var value))
            {
                return value;
            }

            var assetId = _keyProperties[_networkKeyField]
                .FirstOrDefault(k => k.KeyValue.Value == project.LocationKeys[_networkKeyField])?.AssetId;

            return assetId != null
                ? _keyProperties[field].FirstOrDefault(k => k.AssetId == assetId)?.KeyValue.Value ?? ""
                : "";
        }

        private object GetProjectValue(BaseCommittedProjectDTO project, string header)
        {
            return header switch
            {
                CommittedProjectsColumnHeaders.Treatment => project.Treatment,
                CommittedProjectsColumnHeaders.Year => project.Year,
                CommittedProjectsColumnHeaders.Budget => GetBudgetName(project),
                CommittedProjectsColumnHeaders.Cost => project.Cost,
                CommittedProjectsColumnHeaders.ProjectSource => project.ProjectSource,
                CommittedProjectsColumnHeaders.ProjectSourceId => project.ProjectId,
                CommittedProjectsColumnHeaders.Category => project.Category.ToString(),
                _ => null
            };
        }

        private string GetBudgetName(BaseCommittedProjectDTO project)
        {
            var linkedBudget = _unitOfWork.BudgetRepo.GetScenarioBudgets(project.SimulationId)
                .FirstOrDefault(b => b.Id == project.ScenarioBudgetId);

            return linkedBudget?.Name ?? "";
        }

        private static void AddHeaderCells(ExcelWorksheet worksheet, List<ColumnDefinition> columns)
        {
            for (int i = 0; i < columns.Count; i++)
            {
                worksheet.Cells[1, i + 1].Value = columns[i].Header;
            }
        }

        private void AddDataCells(ExcelWorksheet worksheet, List<BaseCommittedProjectDTO> projects, List<ColumnDefinition> columns)
        {
            int row = 2;
            foreach (var project in projects.OrderBy(p => p.LocationKeys[_networkKeyField]).ThenByDescending(p => p.Year))
            {
                for (int col = 0; col < columns.Count; col++)
                {
                    worksheet.Cells[row, col + 1].Value = columns[col].GetValue(project);
                }
                row++;
            }
        }

        public FileInfoDTO ExportCommittedProjectsFile(Guid simulationId)
        {
            var simulation = _unitOfWork.SimulationRepo.GetSimulation(simulationId);
            var simulationName = simulation.Name.Replace(" ", "_").Trim();

            var projects = _unitOfWork.CommittedProjectRepo.GetCommittedProjectsForExport(simulationId);

            var fileName = $"CommittedProjects_{simulationName}.xlsx";

            using var excelPackage = new ExcelPackage();
            var worksheet = excelPackage.Workbook.Worksheets.Add("Committed Projects");

            var columns = GetColumnDefinitions(simulationId);

            AddHeaderCells(worksheet, columns);
            if (projects.Any())
            {
                AddDataCells(worksheet, projects, columns);
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
