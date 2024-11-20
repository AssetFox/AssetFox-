using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Utils;
using Microsoft.IdentityModel.Tokens;
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

        private readonly List<string> importErrors = new();

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
            bool validWorksheet = ValidateWorkSheet(worksheet, _hubService, _networkKeyField, userId);
            if (!validWorksheet)
            {
                return new List<SectionCommittedProjectDTO>();
            }

            var headers = GetHeadersFromWorksheet(worksheet); 
            var budgets = _unitOfWork.BudgetRepo.GetScenarioBudgets(simulation.Id)
                .ToDictionary(b => b.Name, b => b.Id, StringComparer.OrdinalIgnoreCase);

            // Validate columns once
            var columnIndices = ValidateAndGetColumnIndices(
                worksheet,
                CompleteNetworkHeaders[_networkKeyField],
                _hubService,
                userId);

            var locationColumnNames = GetLocationColumnNamesAndKeyColumn(worksheet, headers, out var keyColumn);           
            var projectsPerKey = new Dictionary<(string locationIdentifier, int projectYear, string treatment), SectionCommittedProjectDTO>();

            var maintainableAssetIdsPerLocationId = GetMaintainableAssetsPerLocationId(simulation.NetworkId);
            if (maintainableAssetIdsPerLocationId.IsNullOrEmpty())
            {
                HandleValidationError(_hubService, userId, $"No maintainable assets were found for Network Id: {simulation.NetworkId}");
                return new List<SectionCommittedProjectDTO>();
            }
            
            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
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
                        projectsPerKey[key] = project;
                    }

                   
                }
                catch (Exception ex)
                {
                    HandleValidationError(_hubService, userId, $"Error processing row {row}: {ex.Message}");
                }
            }
            
            return projectsPerKey.Values.ToList();
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
            var locationId = SafeGetLocationIdentifier(worksheet, row, columnIndices, _networkKeyField);
            var assetId = ValidateLocationIdHasAssets(maintainableAssetIdsPerLocationId, locationId);
            var locationInformation = BuildLocationInformation(locationColumnNames, worksheet, row, assetId);        

            return new SectionCommittedProjectDTO
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                ScenarioBudgetId = SafeGetBudgetId(budgets, worksheet, row, columnIndices, CommittedProjectsColumnHeaders.Budget),
                LocationKeys = locationInformation,
                Treatment = SafeGetTreatment(worksheet, row, columnIndices, CommittedProjectsColumnHeaders.Treatment),
                Year = SafeGetProjectYear(worksheet, row, columnIndices, CommittedProjectsColumnHeaders.Year),
                ProjectSource = SafeGetProjectSource(worksheet, row, columnIndices, CommittedProjectsColumnHeaders.ProjectSource),
                ProjectId = SafeGetProjectSourceId(worksheet, row, columnIndices, CommittedProjectsColumnHeaders.ProjectSourceId),
                Cost = SafeGetProjectCost(worksheet, row, columnIndices, CommittedProjectsColumnHeaders.Cost),
                Category = SafeGetTreatmentCategory(worksheet, row, columnIndices, CommittedProjectsColumnHeaders.Category)
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

        private static readonly Dictionary<string, List<string>> CompleteNetworkHeaders = new(StringComparer.OrdinalIgnoreCase)
        {
            { CommittedProjectsColumnHeaders.BRKey, CommittedProjectsColumnHeaders.BridgeHeaders },
            { CommittedProjectsColumnHeaders.CRS, CommittedProjectsColumnHeaders.RoadHeaders }
        };

        private Guid ValidateLocationIdHasAssets(
            Dictionary<string, Guid> maintainableAssetIdsPerLocationId,
            string locationId)
        {
            if (!maintainableAssetIdsPerLocationId.TryGetValue(locationId, out var assetId))
            {
                importErrors.Add($"Location '{locationId}' does not match any network asset");
            }

            return assetId;
        }

        private Dictionary<int, string> GetLocationColumnNamesAndKeyColumn(
            ExcelWorksheet worksheet,
            List<string> headers,
            out int keyColumn)
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
            ExcelWorksheet worksheet,
            int row,
            Dictionary<string, int> columnIndices,
            string columnName)
        { 

            var columnIndex = columnIndices[columnName];
            var identifier = worksheet.GetCellValue<string>(row, columnIndex);

            if (string.IsNullOrWhiteSpace(identifier))
            {
                importErrors.Add($"Parsing error in Row: {row}, Column: {columnIndex}. Location identifier is null or empty");
            }

            return identifier;
        }   
                
        private static string SafeGetProjectSourceId(
            ExcelWorksheet worksheet,
            int row,
            Dictionary<string, int> columnIndices,
            string columnName)
        {
            var columnIndex = columnIndices[columnName];
            if (columnIndex == -1)
            {
                return "None"; //default value
            }

            var sourceId = worksheet.GetCellValue<string>(row, columnIndex);
            return string.IsNullOrWhiteSpace(sourceId)
                ? "None"
                : sourceId.Trim();
        }

        private static ProjectSourceDTO SafeGetProjectSource(
            ExcelWorksheet worksheet,
            int row,
            Dictionary<string, int> columnIndices,
            string columnName)
        {
            var columnIndex = columnIndices[columnName];
            if (columnIndex == -1)
            {
                return ProjectSourceDTO.None; //default value
            }

            var projectSourceValue = worksheet.GetCellValue<string>(row, columnIndex);
            return Enum.TryParse(projectSourceValue, true, out ProjectSourceDTO projectSource)
                ? projectSource
                : ProjectSourceDTO.None;
        }

        private static Guid? SafeGetBudgetId(
            Dictionary<string, Guid> budgets,
            ExcelWorksheet worksheet,
            int row,
            Dictionary<string, int> columnIndices,
            string columnName)
        {
            var columnIndex = columnIndices[columnName];
            if (columnIndex == -1)
            {
                return Guid.Empty; //default value
            }

            var budgetName = worksheet.GetCellValue<string>(row, columnIndex);
            return budgets.TryGetValue(budgetName, out var budgetId)
                ? budgetId
                : Guid.Empty;
        }

        private static double SafeGetProjectCost(
            ExcelWorksheet worksheet,
            int row,
            Dictionary<string, int> columnIndices,
            string columnName)
        {
            var columnIndex = columnIndices[columnName];
            if (columnIndex == -1)
            {
                return 0.0; //default value
            }

            var costValue = worksheet.GetCellValue<string>(row, columnIndex);
            return double.TryParse(costValue.Replace("$", "").Replace(",", ""), out double doubleVal)
                ? doubleVal
                : 0.0;
        }

        private static string SafeGetTreatment(
            ExcelWorksheet worksheet,
            int row,
            Dictionary<string, int> columnIndices,
            string columnName)
        {
            var columnIndex = columnIndices[columnName];
            if (columnIndex == -1)
            {
                return "Default treatment"; //default value
            }

            var treatment = worksheet.GetCellValue<string>(row, columnIndex);
            return string.IsNullOrWhiteSpace(treatment)
                ? "Default treatment"
                : treatment.Trim();
        }

        private static int SafeGetProjectYear(
            ExcelWorksheet worksheet,
            int row,
            Dictionary<string, int> columnIndices,
            string columnName)
        {
            var columnIndex = columnIndices[columnName];
            if (columnIndex == -1)
            {
                return 0; //default value
            }

            var projectYear = worksheet.GetCellValue<string>(row, columnIndex);
            return int.TryParse(projectYear, out var intValue) && intValue >= 1000 && intValue <= 9999
                ? intValue
                : 0;
        }

        private static TreatmentCategory SafeGetTreatmentCategory(
            ExcelWorksheet worksheet,
            int row,
            Dictionary<string, int> columnIndices,
            string columnName)
        {
            var columnIndex = columnIndices[columnName];
            if (columnIndex == -1)
            {
                return TreatmentCategory.Other; //default value
            }

            var treatmentCategoryValue = worksheet.GetCellValue<string>(row, columnIndex);
            return Enum.TryParse(treatmentCategoryValue, true, out TreatmentCategory convertedCategory)
                ? convertedCategory
                : TreatmentCategory.Other;
        }


        private static Dictionary<string, int> ValidateAndGetColumnIndices(
            ExcelWorksheet worksheet,
            List<string> requiredColumns,
            IHubService hubService,
            string userId)
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
                hubService.SendRealTimeMessage(userId, HubConstant.BroadcastWarning,
                    $"Missing columns: {string.Join(", ", missingColumns)}. These will use default values.");
            }

            return columnIndices;
        }
    }
}
