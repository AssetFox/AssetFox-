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
        private List<string> _keyFields;

        public CommittedProjectImporter(
            IUnitOfWork unitOfWork,
            IHubService hubService,
            string networkKeyField,
            List<string> keyFields)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
            _networkKeyField = networkKeyField ?? throw new ArgumentNullException(nameof(networkKeyField));
            _keyFields = keyFields ?? throw new ArgumentNullException(nameof(keyFields));
        }

        public List<SectionCommittedProjectDTO> ImportProjectsFromWorksheet(
            ExcelWorksheet worksheet,
            SimulationDTO simulation,
            string userId)
        {
            bool isValid = ValidateWorkSheet(worksheet, _hubService, _networkKeyField, userId);
            if (!isValid)
            {
                return new List<SectionCommittedProjectDTO>();
            }

            var headers = GetHeadersFromWorksheet(worksheet);

            var importErrors = new List<string>();
            var columnIndices = GetColumnIndices(headers);

            var simulationId = simulation.Id;
            var maintainableAssetIdsPerLocationIdentifier = GetMaintainableAssetsPerLocationIdentifier(simulation.NetworkId);
            var budgets = _unitOfWork.BudgetRepo.GetScenarioBudgets(simulation.Id)
                .ToDictionary(b => b.Name, b => b.Id, StringComparer.OrdinalIgnoreCase);

            var locationColumnNames = GetLocationColumnNamesAndKeyColumn(worksheet, headers, out var keyColumn, importErrors);            

            var projectsPerKey = new Dictionary<(string locationIdentifier, int projectYear, string treatment), SectionCommittedProjectDTO>();
            var invalidLocationIdentifiers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                var rowErrors = new List<string>();
                try
                {
                    var project = ProcessRow(
                        worksheet,
                        row,
                        columnIndices,
                        headers,
                        keyColumn,
                        locationColumnNames,
                        maintainableAssetIdsPerLocationIdentifier,
                        budgets,
                        invalidLocationIdentifiers,
                        simulationId,
                        rowErrors);

                    if (project != null)
                    {
                        var key = (project.LocationKeys[_networkKeyField], project.Year, project.Treatment);
                        projectsPerKey[key] = project;
                    }

                    // Log row-level errors
                    if (rowErrors.Any())
                    {
                        _hubService.SendRealTimeMessage(userId, HubConstant.BroadcastWarning, $"Row {row} processing issues: {string.Join(", ", rowErrors)}");
                    }
                }
                catch (Exception ex)
                {
                    _hubService.SendRealTimeMessage(userId, HubConstant.BroadcastWarning, $"Error processing row {row}: {ex.Message}");
                }
            }

            // Notify about invalid location identifiers
            if (invalidLocationIdentifiers.Any())
            {
                var firstColumnHeader = worksheet.GetCellValue<string>(1, 1) ?? string.Empty;
                string identifierType = firstColumnHeader == "BRKey_" ? "Bridge" :
                                        firstColumnHeader == "CRS" ? "Road" : "Location";

                var invalidIdsMessage = $"{identifierType} identifiers not matching network assets: {string.Join(", ", invalidLocationIdentifiers)}";
                //_hubService.SendRealTimeMessage(userId, HubConstant.BroadcastWarning, invalidIdsMessage);
            }

            return projectsPerKey.Values.ToList();
        }

        /// <summary>
        /// Processes a single row in the worksheet and creates a committed project DTO.
        /// </summary>
        private static SectionCommittedProjectDTO ProcessRow(
            ExcelWorksheet worksheet,
            int row,
            Dictionary<string, int> columnIndices,
            List<string> headers,
            int keyColumn,
            Dictionary<int, string> locationColumnNames,
            Dictionary<string, Guid> maintainableAssetIdsPerLocationIdentifier,
            Dictionary<string, Guid> budgets,
            HashSet<string> invalidLocationIdentifiers,
            Guid simulationId,
            List<string> rowErrors)
        {
            var locationIdentifier = SafeGetLocationIdentifier(worksheet, row, keyColumn, rowErrors);
            var assetId = Guid.Empty;

            if (!string.IsNullOrWhiteSpace(locationIdentifier))
            {
                if (!maintainableAssetIdsPerLocationIdentifier.TryGetValue(locationIdentifier, out assetId))
                {
                    invalidLocationIdentifiers.Add(locationIdentifier);
                    rowErrors.Add($"Location '{locationIdentifier}' does not match any network asset");
                }
            }

            var locationInformation = BuildLocationInformation(locationColumnNames, worksheet, row, assetId);
            var (budgetId, _) = SafeGetBudgetId(
                SafeGetCellValueAsString(worksheet, row, columnIndices, CommittedProjectsColumnHeaders.Budget, headers, rowErrors),
                budgets,
                rowErrors);

            return new SectionCommittedProjectDTO
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                ScenarioBudgetId = budgetId,
                LocationKeys = locationInformation,
                Treatment = SafeGetCellValueAsString(worksheet, row, columnIndices, CommittedProjectsColumnHeaders.Treatment, headers, rowErrors),
                Year = SafeGetCellValueAsInt(worksheet, row, columnIndices, CommittedProjectsColumnHeaders.Year, headers, rowErrors),
                ProjectSource = SafeGetProjectSource(worksheet, row, columnIndices, headers, rowErrors),
                ProjectId = SafeGetCellValueAsString(worksheet, row, columnIndices, CommittedProjectsColumnHeaders.ProjectSourceId, headers, rowErrors),
                Cost = SafeGetCellValueAsDouble(worksheet, row, columnIndices, CommittedProjectsColumnHeaders.Cost, headers, rowErrors),
                Category = SafeGetTreatmentCategory(worksheet, row, columnIndices, headers, rowErrors)
            };
        }

        private static bool ValidateWorkSheet(ExcelWorksheet worksheet, IHubService _hubService, string _networkKeyField, string userId)
        {
            bool isValid = true;

            // Validate worksheet existence
            if (worksheet == null)
            {
                HandleValidationError(_hubService, userId, "Invalid spreadsheet: No worksheet found");
                isValid = false;
            }
            // Validate worksheet dimensions
            else if (worksheet.Dimension?.End.Row < 2)
            {
                HandleValidationError(_hubService, userId, "Invalid spreadsheet: Spreadsheet must contain column headers and at least one data row");
                isValid = false;
            }
            else
            {
                var headers = new HashSet<string>(GetHeadersFromWorksheet(worksheet), StringComparer.OrdinalIgnoreCase);
                if (!KeyHeadersMapping.TryGetValue(_networkKeyField, out var keyHeaders))
                {
                    HandleValidationError(_hubService, userId, $"Invalid network key field: {_networkKeyField}");
                    isValid = false;
                }
                else
                {
                    // Validate key headers
                    var missingKeyHeaders = keyHeaders.Where(col => !headers.Contains(col, StringComparer.OrdinalIgnoreCase)).ToList();
                    if (missingKeyHeaders.Any())
                    {
                        var missingKeyHeaderNames = string.Join(", ", missingKeyHeaders);
                        HandleValidationError(_hubService, userId, $"Invalid spreadsheet: Missing required key columns: {missingKeyHeaderNames}");
                        isValid = false;
                    }
                }
            }

            return isValid;
        }

        private static readonly Dictionary<string, List<string>> KeyHeadersMapping = new(StringComparer.OrdinalIgnoreCase)
        {
            { CommittedProjectsColumnHeaders.BRKey, CommittedProjectsColumnHeaders.BridgeKeyHeaders },
            { CommittedProjectsColumnHeaders.CRS, CommittedProjectsColumnHeaders.RoadKeyHeaders }
        };

        private static void HandleValidationError(IHubService _hubService, string userId, string message)
        {
            _hubService.SendRealTimeMessage(userId, HubConstant.BroadcastWarning, message);
        }

        /// <summary>
        /// Retrieves headers from the first row of the worksheet.
        /// </summary>
        private static List<string> GetHeadersFromWorksheet(ExcelWorksheet worksheet)
        {
            return worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column]
                .Select(cell => cell.GetValue<string>())
                .Where(header => !string.IsNullOrEmpty(header))
                .ToList();
        }

        /// <summary>
        /// Gets a mapping of column names to their indices.
        /// </summary>
        private static Dictionary<string, int> GetColumnIndices(List<string> headers)
        {
            var columnIndices = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < headers.Count; i++)
            {
                columnIndices[headers[i]] = i + 1; // Excel columns are 1-based
            }

            return columnIndices;
        }

        private Dictionary<int, string> GetLocationColumnNamesAndKeyColumn(
            ExcelWorksheet worksheet,
            List<string> headers,
            out int keyColumn,
            List<string> importErrors)
        {
            var locationColumnNames = new Dictionary<int, string>();
            keyColumn = -1;

            // Dynamically determine first column based on worksheet
            var firstColumnHeader = worksheet.GetCellValue<string>(1, 1) ?? string.Empty;
            _keyFields = _keyFields.Intersect(headers, StringComparer.OrdinalIgnoreCase).ToList();

            // Customize error message based on first column
            string identifierType = firstColumnHeader == "BRKey_" ? "Bridge" :
                                    firstColumnHeader == "CRS" ? "Road" : "Location";

            for (var column = 1; column <= worksheet.Dimension.Columns; column++)
            {
                var columnName = worksheet.GetCellValue<string>(1, column) ?? string.Empty;

                if (!_keyFields.Contains(columnName, StringComparer.OrdinalIgnoreCase))
                {
                    importErrors.Add($"Column '{columnName}' is not a recognized key field.");
                    continue;
                }

                locationColumnNames[column] = columnName;

                if (string.Equals(columnName, _networkKeyField, StringComparison.OrdinalIgnoreCase))
                {
                    keyColumn = column;
                }
            }

            if (keyColumn == -1)
            {
                importErrors.Add($"{identifierType} key location column '{_networkKeyField}' not found.");
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

        private static string SafeGetLocationIdentifier(
           ExcelWorksheet worksheet,
           int row,
           int keyColumn,
           List<string> errors)
        {
            var identifier = worksheet.GetCellValue<string>(row, keyColumn) ?? string.Empty;

            if (string.IsNullOrWhiteSpace(identifier))
            {
                errors.Add("Location identifier is null or empty");
            }

            return identifier;
        }

        private static Dictionary<string, string> BuildLocationInformation(
            Dictionary<int, string> locationColumnNames,
            ExcelWorksheet worksheet,
            int row,
            Guid assetId)
        {
            var locationInformation = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["ID"] = assetId.ToString()
            };

            foreach (var kvp in locationColumnNames)
            {
                var column = kvp.Key;
                var columnName = kvp.Value;
                var value = worksheet.GetCellValue<string>(row, column) ?? string.Empty;
                locationInformation[columnName] = value;
            }

            return locationInformation;
        }

        private static (Guid? budgetId, string budgetName) SafeGetBudgetId(
            string budgetName,
            Dictionary<string, Guid> budgets,
            List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(budgetName))
            {
                return (Guid.Empty, string.Empty);
            }

            if (!budgets.TryGetValue(budgetName, out var budgetId))
            {
                errors.Add($"Budget '{budgetName}' not found in budget library");
                return (Guid.Empty, budgetName);
            }

            return (budgetId, budgetName);
        }

        /// <summary>
        /// Parses a cell value as an integer.
        /// </summary>
        private static int SafeGetCellValueAsInt(
            ExcelWorksheet worksheet,
            int row,
            Dictionary<string, int> columnIndices,
            string columnName,
            List<string> headers,
            List<string> errors)
        {
            if (!columnIndices.TryGetValue(columnName, out var col) || !headers.Contains(columnName))
            {
                errors.Add($"Column '{columnName}' not found. Using default: 0");
                return 0;
            }

            var cellValue = worksheet.GetCellValue<string>(row, col);
            return int.TryParse(cellValue, out var intValue) ? intValue : 0;
        }

        private static double SafeGetCellValueAsDouble(
            ExcelWorksheet worksheet,
            int row,
            Dictionary<string, int> columnIndices,
            string columnName,
            List<string> headers,
            List<string> errors)
        {
            if (!columnIndices.TryGetValue(columnName, out var col) || !headers.Contains(columnName))
            {
                errors.Add($"Column '{columnName}' not found. Using default: -1");
                return -1.0;
            }

            var cellValue = worksheet.GetCellValue<string>(row, col);
            return double.TryParse(cellValue, out var doubleValue) ? doubleValue : -1.0;
        }

        private static string SafeGetCellValueAsString(
            ExcelWorksheet worksheet,
            int row,
            Dictionary<string, int> columnIndices,
            string columnName,
            List<string> headers,
            List<string> errors)
        {
            if (!columnIndices.TryGetValue(columnName, out var col) || !headers.Contains(columnName))
            {
                errors.Add($"Column '{columnName}' not found. Using default: empty string");
                return string.Empty;
            }

            return worksheet.GetCellValue<string>(row, col) ?? string.Empty;
        }

        private static ProjectSourceDTO SafeGetProjectSource(
            ExcelWorksheet worksheet,
            int row,
            Dictionary<string, int> columnIndices,
            List<string> headers,
            List<string> errors)
        {
            if (!columnIndices.TryGetValue(CommittedProjectsColumnHeaders.ProjectSource, out var col) ||
                !headers.Contains(CommittedProjectsColumnHeaders.ProjectSource))
            {
                errors.Add($"Column 'Project Source' not found. Using default: None");
                return ProjectSourceDTO.None;
            }

            var projectSourceValue = worksheet.GetCellValue<string>(row, col);
            return Enum.TryParse(projectSourceValue, true, out ProjectSourceDTO projectSource)
                ? projectSource
                : ProjectSourceDTO.None;
        }

        private static TreatmentCategory SafeGetTreatmentCategory(
            ExcelWorksheet worksheet,
            int row,
            Dictionary<string, int> columnIndices,
            List<string> headers,
            List<string> errors)
        {
            if (!columnIndices.TryGetValue(CommittedProjectsColumnHeaders.Category, out var col) ||
                !headers.Contains(CommittedProjectsColumnHeaders.Category))
            {
                errors.Add($"Column 'Category' not found. Using default: Other");
                return TreatmentCategory.Other;
            }

            var treatmentCategoryValue = worksheet.GetCellValue<string>(row, col);
            return Enum.TryParse(treatmentCategoryValue, true, out TreatmentCategory convertedCategory)
                ? convertedCategory
                : TreatmentCategory.Other;
        }

    }
}
