using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Utils;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        // Thread-safe collections for error tracking and project management
        private ConcurrentDictionary<ErrorType, ConcurrentBag<string>> _validationErrorMessages = new();
        private ConcurrentDictionary<(string locationIdentifier, int projectYear, string treatment), SectionCommittedProjectDTO> _projectsPerKey = new();
        private ConcurrentDictionary<(string locationIdentifier, int projectYear, string treatment), bool> _processedKeys = new();


        // Configurable batch sizes for notifications
        private const int MAX_ERROR_BATCH_SIZE = 10;

        public enum ErrorType
        {
            MissingValue,
            InvalidValue,
            DuplicateEntry,
            AssetNotFound,
            ParsingError,
            GeneralError
        }

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
            // Reset collections for each import
            _validationErrorMessages = new ConcurrentDictionary<ErrorType, ConcurrentBag<string>>();
            _projectsPerKey = new ConcurrentDictionary<(string, int, string), SectionCommittedProjectDTO>();
            _processedKeys = new ConcurrentDictionary<(string locationIdentifier, int projectYear, string treatment), bool>();

            var headers = GetHeadersFromWorksheet(worksheet);
            if (!ValidateWorkSheet(worksheet, _hubService, _networkKeyField, headers, userId))
            {
                return new List<SectionCommittedProjectDTO>();
            }
            
            var budgets = _unitOfWork.BudgetRepo.GetScenarioBudgets(simulation.Id)
                .ToDictionary(b => b.Name, b => b.Id, StringComparer.OrdinalIgnoreCase);

            // Validate columns once
            var columnIndices = ValidateAndGetColumnIndices(
                worksheet,
                CompleteNetworkHeaders[_networkKeyField]);

            var locationColumnNames = GetLocationColumnNamesAndKeyColumn(worksheet, headers, out var keyColumn);           
            var projectsPerKey = new Dictionary<(string locationIdentifier, int projectYear, string treatment), SectionCommittedProjectDTO>();

            var maintainableAssetIdsPerLocationId = GetMaintainableAssetsPerLocationId(simulation.NetworkId);
            if (maintainableAssetIdsPerLocationId.IsNullOrEmpty())
            {
                HandleValidationError(_hubService, userId, $"No maintainable assets were found for Network Id: {simulation.NetworkId}");
                return new List<SectionCommittedProjectDTO>();
            }

            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            Parallel.For(2, worksheet.Dimension.End.Row + 1, options, row =>
            {
                try
                {
                    var project = ProcessRow(
                        worksheet,
                        row,
                        columnIndices,
                        locationColumnNames,
                        maintainableAssetIdsPerLocationId,
                        budgets,
                        simulation.Id);

                    if (project != null)
                    {
                        var key = (project.LocationKeys[_networkKeyField], project.Year, project.Treatment);
                        _projectsPerKey.TryAdd(key, project);
                    }
                }
                catch (Exception)
                {
                    AddValidationError(ErrorType.GeneralError, $"Error processing row {row}");
                }
            });
          
            // Batch error reporting
            NotifyValidationErrors(userId);
            return _projectsPerKey.Values.ToList();
        }

        /// <summary>
        /// Processes a single row in the worksheet and creates a committed project DTO.
        /// </summary>
        private SectionCommittedProjectDTO ProcessRow(
            ExcelWorksheet worksheet,
            int row,
            Dictionary<string, int> columnIndices,
            Dictionary<int, string> locationColumnNames,
            Dictionary<string, Guid> maintainableAssetIdsPerLocationId,
            Dictionary<string, Guid> budgets,
            Guid simulationId)
        {
            // Read all cell values for the row once
            var rowValues = new Dictionary<int, object>();
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                rowValues[col] = worksheet.Cells[row, col].Value;
            }

            // Extract key components
            var locationId = SafeGetLocationIdentifier(rowValues, columnIndices, _networkKeyField, row);
            var projectYear = SafeGetProjectYear(rowValues, columnIndices, CommittedProjectsColumnHeaders.Year, row);
            var treatment = SafeGetTreatment(rowValues, columnIndices, CommittedProjectsColumnHeaders.Treatment, row);

            var key = (locationIdentifier: locationId, projectYear, treatment);
            // Check for duplicates
            if (!_processedKeys.TryAdd(key, true))
            {
                // Duplicate found
                AddValidationError(ErrorType.DuplicateEntry,
                    $"Duplicate Project Removed: Row {row}, Asset {locationId}, Year {projectYear}, Treatment '{treatment}'");
                return null; // Skip further processing
            }

            var assetId = ValidateLocationIdHasAssets(maintainableAssetIdsPerLocationId, locationId, row);
            var locationInformation = BuildLocationInformation(locationColumnNames, rowValues, assetId, row);        

            return new SectionCommittedProjectDTO
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                ScenarioBudgetId = SafeGetBudgetId(budgets, rowValues, columnIndices, CommittedProjectsColumnHeaders.Budget, row),
                LocationKeys = locationInformation,
                Treatment = treatment,
                Year = projectYear,
                ProjectSource = SafeGetProjectSource(rowValues, columnIndices, CommittedProjectsColumnHeaders.ProjectSource, row),
                ProjectId = SafeGetProjectSourceId(rowValues, columnIndices, CommittedProjectsColumnHeaders.ProjectSourceId, row),
                Cost = SafeGetProjectCost(rowValues, columnIndices, CommittedProjectsColumnHeaders.Cost, row),
                Category = SafeGetTreatmentCategory(rowValues, columnIndices, CommittedProjectsColumnHeaders.Category, row)
            };
        }

        private static bool ValidateWorkSheet(
            ExcelWorksheet worksheet,
            IHubService _hubService,
            string _networkKeyField,
            List<string> headers,
            string userId)
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

        private static readonly Dictionary<string, List<string>> CompleteNetworkHeaders = new(StringComparer.OrdinalIgnoreCase)
        {
            { CommittedProjectsColumnHeaders.BRKey, CommittedProjectsColumnHeaders.BridgeHeaders },
            { CommittedProjectsColumnHeaders.CRS, CommittedProjectsColumnHeaders.RoadHeaders }
        };

        private Guid ValidateLocationIdHasAssets(
            Dictionary<string, Guid> maintainableAssetIdsPerLocationId,
            string locationId,
            int row)
        {
            if (!maintainableAssetIdsPerLocationId.TryGetValue(locationId, out var assetId))
            {
                AddValidationError(ErrorType.AssetNotFound, $"Row {row}: Location '{locationId}' does not match any network asset.");
            }

            return assetId;
        }

        private void AddValidationError(ErrorType errorType, string message)
        {
            var errorBag = _validationErrorMessages.GetOrAdd(errorType, new ConcurrentBag<string>());
            errorBag.Add(message);
        }


        private Dictionary<int, string> GetLocationColumnNamesAndKeyColumn(
            ExcelWorksheet worksheet,
            List<string> headers,
            out int keyColumn)
        {
            var locationColumnNames = new Dictionary<int, string>();
            keyColumn = -1;
            _keyFields = _keyFields.Intersect(headers, StringComparer.OrdinalIgnoreCase).ToList();

            for (var column = 1; column <= worksheet.Dimension.Columns; column++)
            {
                var columnName = worksheet.GetCellValue<string>(1, column) ?? string.Empty;
                locationColumnNames[column] = columnName;

                if (string.Equals(columnName, _networkKeyField, StringComparison.OrdinalIgnoreCase))
                {
                    keyColumn = column;
                }
            }

            if (keyColumn == -1)
            {
                AddValidationError(ErrorType.MissingValue, $"Key location column '{_networkKeyField}' not found.");
            }

            return locationColumnNames;
        }

        private Dictionary<string, string> BuildLocationInformation(
            Dictionary<int, string> locationColumnNames,
            Dictionary<int, object> rowValues,
            Guid assetId,
            int row)
        {
            var locationInformation = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["ID"] = assetId.ToString()
            };

            foreach (var kvp in locationColumnNames)
            {
                var column = kvp.Key;
                var columnName = kvp.Value;
                var value = rowValues.TryGetValue(column, out var cellValue) && cellValue != null
                    ? cellValue.ToString()
                    : string.Empty;

                if (string.IsNullOrWhiteSpace(value))
                {
                    AddValidationError(ErrorType.MissingValue, $"Row {row}, Column {column} ('{columnName}'): Value is null or empty.");
                }

                locationInformation[columnName] = value;
            }

            return locationInformation;
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
        /// Retrieves a mapping of location identifiers to maintainable asset IDs.
        /// </summary>
        private Dictionary<string, Guid> GetMaintainableAssetsPerLocationId(Guid networkId)
        {
            var assets = _unitOfWork.MaintainableAssetRepo.GetAllInNetworkWithLocations(networkId);
            if (!assets.Any())
            {
                return null;
            }

            return assets.ToDictionary(
                asset => asset.Location.LocationIdentifier,
                asset => asset.Id,
                StringComparer.OrdinalIgnoreCase);
        }

        private string SafeGetLocationIdentifier(
            Dictionary<int, object> rowValues,
            Dictionary<string, int> columnIndices,
            string columnName,
            int row)
        {

            var columnIndex = columnIndices[columnName];
            if (rowValues.TryGetValue(columnIndex, out var cellValue) && cellValue != null)
            {
                var identifier = cellValue.ToString();
                if (string.IsNullOrWhiteSpace(identifier))
                {
                    AddValidationError(ErrorType.MissingValue, $"Row {row}, Column {columnIndex} ('{columnName}'): Location identifier is null or empty.");
                }
                return identifier;
            }
            else
            {
                AddValidationError(ErrorType.MissingValue, $"Row {row}, Column {columnIndex} ('{columnName}'): Location identifier is missing.");
                return string.Empty;
            }
        }   
                
        private string SafeGetProjectSourceId(
            Dictionary<int, object> rowValues,
            Dictionary<string, int> columnIndices,
            string columnName,
            int row)
        {
            var columnIndex = columnIndices[columnName];
            if (columnIndex == -1)
            {
                return "None"; //default value
            }

            if (rowValues.TryGetValue(columnIndex, out var cellValue) && cellValue != null)
            {
                var sourceId = cellValue.ToString();
                return string.IsNullOrWhiteSpace(sourceId)
                    ? "None"
                    : sourceId.Trim();
            }
            else
            {
                AddValidationError(ErrorType.MissingValue, $"Row {row}, Column {columnIndex} ('{columnName}'): Project Source ID is missing.");
                return "None";
            }
        }

        private ProjectSourceDTO SafeGetProjectSource(
            Dictionary<int, object> rowValues,
            Dictionary<string, int> columnIndices,
            string columnName,
            int row)
        {
            var columnIndex = columnIndices[columnName];
            if (columnIndex == -1)
            {
                return ProjectSourceDTO.None; // Default value
            }

            if (rowValues.TryGetValue(columnIndex, out var cellValue) && cellValue != null)
            {
                var projectSourceValue = cellValue.ToString();
                if (Enum.TryParse(projectSourceValue, true, out ProjectSourceDTO projectSource))
                {
                    return projectSource;
                }
                else
                {
                    AddValidationError(ErrorType.InvalidValue, $"Row {row}, Column {columnIndex} ('{columnName}'): Invalid project source '{projectSourceValue}'.");
                    return ProjectSourceDTO.None;
                }
            }
            else
            {
                AddValidationError(ErrorType.MissingValue, $"Row {row}, Column {columnIndex} ('{columnName}'): Project source is missing..");
                return ProjectSourceDTO.None;
            }
        }


        private Guid? SafeGetBudgetId(
            Dictionary<string, Guid> budgets,
            Dictionary<int, object> rowValues,
            Dictionary<string, int> columnIndices,
            string columnName,
            int row)
        {
            var columnIndex = columnIndices[columnName];
            if (columnIndex == -1)
            {
                return Guid.Empty; // Default value
            }

            if (rowValues.TryGetValue(columnIndex, out var cellValue) && cellValue != null)
            {
                var budgetName = cellValue.ToString();
                if (budgets.TryGetValue(budgetName, out var budgetId))
                {
                    return budgetId;
                }
                else
                {
                    AddValidationError(ErrorType.MissingValue, $"Row {row}, Column {columnIndex} ('{columnName}'): Budget '{budgetName}' not found.");
                    return Guid.Empty;
                }
            }
            else
            {
                AddValidationError(ErrorType.MissingValue, $"Row {row}, Column {columnIndex} ('{columnName}'): Budget is missing.");
                return Guid.Empty;
            }
        }


        private double SafeGetProjectCost(
            Dictionary<int, object> rowValues,
            Dictionary<string, int> columnIndices,
            string columnName,
            int row)
        {
            var columnIndex = columnIndices[columnName];
            if (columnIndex == -1)
            {
                return 0.0; // Default value
            }

            if (rowValues.TryGetValue(columnIndex, out var cellValue) && cellValue != null)
            {
                var costValue = cellValue.ToString().Replace("$", "").Replace(",", "");
                if (double.TryParse(costValue, out double doubleVal))
                {
                    return doubleVal;
                }
                else
                {
                    AddValidationError(ErrorType.InvalidValue, $"Row {row}, Column {columnIndex} ('{columnName}'): Invalid cost '{cellValue}'.");
                    return 0.0;
                }
            }
            else
            {
                AddValidationError(ErrorType.MissingValue, $"Row {row}, Column {columnIndex} ('{columnName}'): Cost is missing.");
                return 0.0;
            }
        }


        private string SafeGetTreatment(
             Dictionary<int, object> rowValues,
             Dictionary<string, int> columnIndices,
             string columnName,
             int row)
        {
            var columnIndex = columnIndices[columnName];
            if (columnIndex == -1)
            {
                return "Default treatment"; // Default value
            }

            if (rowValues.TryGetValue(columnIndex, out var cellValue) && cellValue != null)
            {
                var treatment = cellValue.ToString();
                if (string.IsNullOrWhiteSpace(treatment))
                {
                    AddValidationError(ErrorType.InvalidValue, $"Row {row}, Column {columnIndex} ('{columnName}'): Treatment is null or empty.");
                    return "Default treatment";
                }
                return treatment.Trim();
            }
            else
            {
                AddValidationError(ErrorType.MissingValue, $"Row {row}, Column {columnIndex} ('{columnName}'): Treatment is missing.");
                return "Default treatment";
            }
        }


        private int SafeGetProjectYear(
            Dictionary<int, object> rowValues,
            Dictionary<string, int> columnIndices,
            string columnName,
            int row)
        {
            var columnIndex = columnIndices[columnName];
            if (columnIndex == -1)
            {
                return 0; //default value
            }

            if (rowValues.TryGetValue(columnIndex, out var cellValue) && cellValue != null)
            {
                var projectYearStr = cellValue.ToString();
                if (int.TryParse(projectYearStr, out var intValue) && intValue >= 1000 && intValue <= 9999)
                {
                    return intValue;
                }
                else
                {
                    AddValidationError(ErrorType.InvalidValue, $"Row {row}, Column {columnIndex} ('{columnName}'): Invalid project year '{projectYearStr}'.");
                    return 0;
                }
            }
            else
            {
                AddValidationError(ErrorType.MissingValue, $"Row {row}, Column {columnIndex} ('{columnName}'): Project year is missing.");
                return 0;
            }
        }

        private TreatmentCategory SafeGetTreatmentCategory(
            Dictionary<int, object> rowValues,
            Dictionary<string, int> columnIndices,
            string columnName,
            int row)
        {
            var columnIndex = columnIndices[columnName];
            if (columnIndex == -1)
            {
                return TreatmentCategory.Other; // Default value
            }

            if (rowValues.TryGetValue(columnIndex, out var cellValue) && cellValue != null)
            {
                var categoryValue = cellValue.ToString();
                if (Enum.TryParse(categoryValue, true, out TreatmentCategory category))
                {
                    return category;
                }
                else
                {
                    AddValidationError(ErrorType.InvalidValue, $"Row {row}, Column {columnIndex} ('{columnName}'): Invalid treatment category '{categoryValue}'.");
                    return TreatmentCategory.Other;
                }
            }
            else
            {
                AddValidationError(ErrorType.MissingValue, $"Row {row}, Column {columnIndex} ('{columnName}'): Treatment category is missing.");
                return TreatmentCategory.Other;
            }
        }


        private Dictionary<string, int> ValidateAndGetColumnIndices(
            ExcelWorksheet worksheet,
            List<string> requiredColumns)
        {
            var headers = GetHeadersFromWorksheet(worksheet);
            var columnIndices = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var missingColumns = new List<string>();

            foreach (var column in requiredColumns)
            {
                var index = headers.FindIndex(h => h.Equals(column, StringComparison.OrdinalIgnoreCase));
                if (index != -1)
                {
                    columnIndices[column] = index + 1; // Excel columns are 1-based
                }
                else
                {
                    missingColumns.Add(column);
                    columnIndices[column] = -1; // Indicate column not found
                }
            }

            // Broadcast missing columns if any
            if (missingColumns.Any())
            {
                var errors = new StringBuilder();
                errors.Append($"Missing columns: {string.Join(", ", missingColumns)}. These will use default values.");
                AddValidationError(ErrorType.MissingValue, errors.ToString());
            }

            return columnIndices;
        }

        private void NotifyValidationErrors(string userId)
        {
            if (_validationErrorMessages == null || !_validationErrorMessages.Any())
            {
                return;
            }

            foreach (var errorType in _validationErrorMessages.Keys)
            {
                var errors = _validationErrorMessages[errorType].Distinct().ToList();
                var errorCount = errors.Count;

                if (errorCount > 0)
                {
                    var messagesToShow = errors.Take(MAX_ERROR_BATCH_SIZE).ToList();

                    if (errorCount > MAX_ERROR_BATCH_SIZE)
                    {
                        messagesToShow.Add($"... and {errorCount - MAX_ERROR_BATCH_SIZE} more errors");
                    }

                    var errorTitle = GetErrorTitle(errorType, errorCount);

                    _hubService.SendRealTimeMessage(
                        userId,
                        HubConstant.BroadcastWarning,
                        $"{errorTitle}\n{string.Join("\n", messagesToShow)}"
                    );
                }
            }
        }

        private static string GetErrorTitle(ErrorType errorType, int count)
        {
            return errorType switch
            {
                ErrorType.MissingValue => $"Missing Values Detected ({count} total):",
                ErrorType.InvalidValue => $"Invalid Values Detected ({count} total):",
                ErrorType.DuplicateEntry => $"Duplicate Entries Detected ({count} total):",
                ErrorType.AssetNotFound => $"Assets Not Found ({count} total):",
                ErrorType.ParsingError => $"Parsing Errors ({count} total):",
                ErrorType.GeneralError => $"General Errors ({count} total):",
                _ => $"Errors ({count} total):",
            };
        }
    }
}
