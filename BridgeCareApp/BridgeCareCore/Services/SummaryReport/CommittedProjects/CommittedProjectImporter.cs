using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Utils;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BridgeCareCore.Services.SummaryReport.CommittedProjects
{
    /// <summary>
    /// Handles the import of committed projects from an Excel worksheet.
    /// </summary>
    public class CommittedProjectImporter
    {
        // Dependencies required for importing
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubService _hubService;
        private readonly string _networkKeyField;
        private readonly Dictionary<string, List<KeySegmentDatum>> _keyProperties;
        private List<string> _keyFields;

        public CommittedProjectImporter(
            IUnitOfWork unitOfWork,
            IHubService hubService,
            string networkKeyField,
            Dictionary<string, List<KeySegmentDatum>> keyProperties,
            List<string> keyFields)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
            _networkKeyField = networkKeyField ?? throw new ArgumentNullException(nameof(networkKeyField));
            _keyProperties = keyProperties ?? throw new ArgumentNullException(nameof(keyProperties));
            _keyFields = keyFields ?? throw new ArgumentNullException(nameof(keyFields));
        }

        public List<SectionCommittedProjectDTO> ImportProjectsFromWorksheet(
            ExcelWorksheet worksheet,
            Guid simulationId,
            string userId)
        {
            var headers = GetHeadersFromWorksheet(worksheet);
            ValidateRequiredColumns(headers);
            var columnIndices = GetColumnIndices(headers);

            var simulation = GetSimulation(simulationId);
            var maintainableAssetIdsPerLocationIdentifier = GetMaintainableAssetsPerLocationIdentifier(simulation.NetworkId);

            int keyColumn;
            var locationColumnNames = GetLocationColumnNamesAndKeyColumn(worksheet, headers, out keyColumn);

            var budgets = _unitOfWork.BudgetRepo.GetScenarioBudgets(simulationId)
                .ToDictionary(b => b.Name, b => b.Id, StringComparer.OrdinalIgnoreCase);

            var projectsPerKey = new Dictionary<(string locationIdentifier, int projectYear, string treatment), SectionCommittedProjectDTO>();
            var invalidLocationIdentifiers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                try
                {
                    var project = ProcessRow(
                        worksheet,
                        row,
                        columnIndices,
                        keyColumn,
                        locationColumnNames,
                        maintainableAssetIdsPerLocationIdentifier,
                        budgets,
                        invalidLocationIdentifiers,
                        simulationId);

                    if (project != null)
                    {
                        var key = (project.LocationKeys[_networkKeyField], project.Year, project.Treatment);
                        if (!projectsPerKey.ContainsKey(key))
                        {
                            projectsPerKey.Add(key, project);
                        }
                        else
                        {
                            // Overwrite the existing entry or handle duplicates as needed
                            projectsPerKey[key] = project;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log or handle the exception as appropriate
                    _hubService.SendRealTimeMessage(userId, HubConstant.BroadcastWarning, $"Error processing row {row}: {ex.Message}");
                }
            }

            if (invalidLocationIdentifiers.Any())
            {
                var invalidIdsMessage = $"The following location identifiers do not match any asset in the network: {string.Join(", ", invalidLocationIdentifiers)}";
                _hubService.SendRealTimeMessage(userId, HubConstant.BroadcastWarning, invalidIdsMessage);
            }

            return projectsPerKey.Values.ToList();

        }

        /// <summary>
        /// Retrieves headers from the first row of the worksheet.
        /// </summary>
        private List<string> GetHeadersFromWorksheet(ExcelWorksheet worksheet)
        {
            return worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column]
                .Select(cell => cell.GetValue<string>())
                .Where(header => !string.IsNullOrEmpty(header))
                .ToList();
        }

        /// <summary>
        /// Validates that all required columns are present in the worksheet.
        /// </summary>
        private void ValidateRequiredColumns(List<string> headers)
        {
            var requiredColumns = new List<string>
            {
                CommittedProjectsColumnHeaders.Category,
                CommittedProjectsColumnHeaders.ProjectSource,
                CommittedProjectsColumnHeaders.ProjectSourceId,
                _networkKeyField // Network key field is required
            };

            foreach (var column in requiredColumns)
            {
                if (!headers.Contains(column))
                {
                    throw new InvalidOperationException($"Required column '{column}' is missing in the Excel sheet.");
                }
            }
        }

        /// <summary>
        /// Gets a mapping of column names to their indices.
        /// </summary>
        private Dictionary<string, int> GetColumnIndices(List<string> headers)
        {
            var columnIndices = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < headers.Count; i++)
            {
                columnIndices[headers[i]] = i + 1; // Excel columns are 1-based
            }

            return columnIndices;
        }

        /// <summary>
        /// Retrieves location column names and identifies the key column index.
        /// </summary>
        private Dictionary<int, string> GetLocationColumnNamesAndKeyColumn(
            ExcelWorksheet worksheet,
            List<string> headers,
            out int keyColumn)
        {
            var locationColumnNames = new Dictionary<int, string>();
            keyColumn = -1;

            // Ensure _keyFields are consistent with headers
            _keyFields = _keyFields.Intersect(headers, StringComparer.OrdinalIgnoreCase).ToList();

            for (var column = 1; column <= _keyFields.Count; column++)
            {
                var columnName = worksheet.GetCellValue<string>(1, column);
                if (!_keyFields.Contains(columnName, StringComparer.OrdinalIgnoreCase))
                {
                    var keyFieldList = string.Join(", ", _keyFields);
                    throw new InvalidOperationException(
                        $"Column '{columnName}' is not a key field of the provided network. Possible key fields are: {keyFieldList}.");
                }
                locationColumnNames[column] = columnName;
                if (string.Equals(columnName, _networkKeyField, StringComparison.OrdinalIgnoreCase))
                {
                    keyColumn = column;
                }
            }

            if (keyColumn == -1)
            {
                throw new InvalidOperationException($"The key location column '{_networkKeyField}' was not found in the worksheet.");
            }

            return locationColumnNames;
        }

        /// <summary>
        /// Retrieves a mapping of location identifiers to maintainable asset IDs.
        /// </summary>
        private Dictionary<string, Guid> GetMaintainableAssetsPerLocationIdentifier(Guid networkId)
        {
            var assets = _unitOfWork.MaintainableAssetRepo.GetAllInNetworkWithLocations(networkId);
            if (!assets.Any())
            {
                throw new InvalidOperationException("There are no maintainable assets in the database.");
            }

            return assets.ToDictionary(
                asset => asset.Location.LocationIdentifier,
                asset => asset.Id,
                StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Processes a single row in the worksheet and creates a committed project DTO.
        /// </summary>
        private SectionCommittedProjectDTO ProcessRow(
            ExcelWorksheet worksheet,
            int row,
            Dictionary<string, int> columnIndices,
            int keyColumn,
            Dictionary<int, string> locationColumnNames,
            Dictionary<string, Guid> maintainableAssetIdsPerLocationIdentifier,
            Dictionary<string, Guid> budgets,
            HashSet<string> invalidLocationIdentifiers,
            Guid simulationId)
        {
            // Get location identifier
            var locationIdentifier = worksheet.GetCellValue<string>(row, keyColumn);

            if (string.IsNullOrWhiteSpace(locationIdentifier))
            {
                throw new InvalidOperationException($"Location identifier in row {row} is null or empty.");
            }

            if (!maintainableAssetIdsPerLocationIdentifier.TryGetValue(locationIdentifier, out var assetId))
            {
                invalidLocationIdentifiers.Add(locationIdentifier);
                // Skip this project
                return null;
            }

            var locationInformation = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["ID"] = assetId.ToString()
            };

            foreach (var kvp in locationColumnNames)
            {
                var column = kvp.Key;
                var columnName = kvp.Value;
                var value = worksheet.GetCellValue<string>(row, column);
                locationInformation[columnName] = value;
            }

            // Get project year
            var projectYear = GetCellValueAsInt(worksheet, row, columnIndices, CommittedProjectsColumnHeaders.Year, "project year");

            // Get treatment
            var treatment = GetCellValueAsString(worksheet, row, columnIndices, CommittedProjectsColumnHeaders.Treatment);

            // Get project source
            var projectSourceValue = GetCellValueAsString(worksheet, row, columnIndices, CommittedProjectsColumnHeaders.ProjectSource);
            if (!Enum.TryParse(projectSourceValue, true, out ProjectSourceDTO projectSource))
            {
                projectSource = ProjectSourceDTO.None; // Default value if parsing fails
            }

            // Get project ID
            var projectIdValue = GetCellValueAsString(worksheet, row, columnIndices, CommittedProjectsColumnHeaders.ProjectSourceId);

            // Get budget
            var budgetName = GetCellValueAsString(worksheet, row, columnIndices, CommittedProjectsColumnHeaders.Budget);
            Guid? budgetId = null;

            if (!string.IsNullOrWhiteSpace(budgetName))
            {
                if (!budgets.TryGetValue(budgetName, out var bId))
                {
                    throw new InvalidOperationException($"Budget '{budgetName}' does not exist in the applied budget library.");
                }
                budgetId = bId;
            }

            // Get treatment category
            var treatmentCategoryValue = GetCellValueAsString(worksheet, row, columnIndices, CommittedProjectsColumnHeaders.Category);
            if (!Enum.TryParse(treatmentCategoryValue, true, out TreatmentCategory convertedCategory))
            {
                convertedCategory = TreatmentCategory.Other;
            }

            // Get cost
            var cost = GetCellValueAsDouble(worksheet, row, columnIndices, CommittedProjectsColumnHeaders.Cost, "cost");

            // Build the committed project object
            return new SectionCommittedProjectDTO
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                ScenarioBudgetId = budgetId,
                LocationKeys = locationInformation,
                Treatment = treatment,
                Year = projectYear,
                ProjectSource = projectSource,
                ProjectId = projectIdValue,
                Cost = cost,
                Category = convertedCategory
            };
        }

        /// <summary>
        /// Parses a cell value as an integer.
        /// </summary>
        private int GetCellValueAsInt(ExcelWorksheet worksheet, int row, Dictionary<string, int> columnIndices, string columnName, string valueDescription)
        {
            if (!columnIndices.TryGetValue(columnName, out var col))
            {
                throw new InvalidOperationException($"Column '{columnName}' not found in column indices.");
            }

            var cellValue = worksheet.GetCellValue<string>(row, col);
            if (!int.TryParse(cellValue, out var intValue))
            {
                throw new InvalidOperationException($"Invalid {valueDescription} '{cellValue}' at row {row}.");
            }
            return intValue;
        }

        /// <summary>
        /// Parses a cell value as a double.
        /// </summary>
        private double GetCellValueAsDouble(ExcelWorksheet worksheet, int row, Dictionary<string, int> columnIndices, string columnName, string valueDescription)
        {
            if (!columnIndices.TryGetValue(columnName, out var col))
            {
                throw new InvalidOperationException($"Column '{columnName}' not found in column indices.");
            }

            var cellValue = worksheet.GetCellValue<string>(row, col);
            if (double.TryParse(cellValue, out var doubleValue))
            {
                return doubleValue;
            }
            else
            {
                return -1.0; // Default value if parsing fails
            }
        }

        /// <summary>
        /// Retrieves a cell value as a string.
        /// </summary>
        private string GetCellValueAsString(ExcelWorksheet worksheet, int row, Dictionary<string, int> columnIndices, string columnName)
        {
            if (!columnIndices.TryGetValue(columnName, out var col))
            {
                throw new InvalidOperationException($"Column '{columnName}' not found in column indices.");
            }

            return worksheet.GetCellValue<string>(row, col);
        }

        /// <summary>
        /// Retrieves the simulation by ID.
        /// </summary>
        private SimulationDTO GetSimulation(Guid simulationId)
        {
            var simulation = _unitOfWork.SimulationRepo.GetSimulation(simulationId)
                ?? throw new ArgumentException($"No simulation was found for the given scenario with ID {simulationId}.");
            return simulation;
        }
    }
}
