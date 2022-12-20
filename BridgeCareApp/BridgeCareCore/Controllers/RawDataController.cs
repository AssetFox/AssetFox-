using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Linq;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BridgeCareCore.Services;
using System.Data;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage;

namespace BridgeCareCore.Controllers
{

    [Route("api/[controller]")]
    public class RawDataController : BridgeCareCoreBaseController
    {
        public const string RawDataError = "Raw Data Error";

        private readonly IExcelRawDataImportService _excelSpreadsheetImportService;

        public RawDataController(
            IEsecSecurity esecSecurity,
            IUnitOfWork unitOfWork,
            IHubService hubService,
            IHttpContextAccessor contextAccessor,
            IExcelRawDataImportService excelSpreadsheetImportService
            ) : base(esecSecurity, unitOfWork, hubService, contextAccessor)
        {
            _excelSpreadsheetImportService = excelSpreadsheetImportService;
        }

        [HttpPost]
        [Route("ImportExcelSpreadsheet/{dataSourceId}")]
        [Authorize]
        public async Task<IActionResult> ImportExcelSpreadsheet(
            Guid dataSourceId)
        {
            try
            {
                if (!ContextAccessor.HttpContext.Request.HasFormContentType)
                {
                    throw new ConstraintException("Request MIME type is invalid.");
                }

                if (ContextAccessor.HttpContext.Request.Form.Files.Count < 1)
                {
                    throw new ConstraintException("Attributes file not found.");
                }

                var excelPackage = new ExcelPackage(ContextAccessor.HttpContext.Request.Form.Files[0].OpenReadStream());
                var worksheet = excelPackage.Workbook.Worksheets[0];

                var result = await Task.Factory.StartNew(() =>
                {
                    return _excelSpreadsheetImportService.ImportRawData(dataSourceId, worksheet);
                });

                if (result.WarningMessage != null)
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWarning, result.WarningMessage);
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{RawDataError}::ImportExcelSpreadsheet - {e.Message}");
                throw;
            }
        }
        [HttpGet]
        [Route("GetExcelSpreadsheetColumnHeaders/{dataSourceId}")]
        [Authorize]
        public async Task<IActionResult> GetExcelSpreadsheetColumnHeaders(Guid dataSourceId)
        {
            try {
                var result = await Task.Factory.StartNew(() =>
                {
                    var dataSource = UnitOfWork.DataSourceRepo.GetDataSource(dataSourceId);
                    string warningMessage = null;
                    if (dataSource == null)
                    {
                        warningMessage = $"No dataSource found with id {dataSourceId}";
                    }
                    else if (dataSource.Type.ToUpperInvariant()!="EXCEL")
                    {
                        warningMessage = @$"DataSource found. Its type was {dataSource.Type}. Expected the type to be ""EXCEL""";
                    }
                    
                    if (warningMessage!=null)
                    {
                        return new GetRawDataSpreadsheetColumnHeadersResultDTO
                        {
                            WarningMessage = warningMessage,
                        };
                    }
                    var excelSpreadsheet = UnitOfWork.ExcelWorksheetRepository.GetExcelRawDataByDataSourceId(dataSourceId);
                    if (excelSpreadsheet == null)
                    {
                        warningMessage = $@"Found a DataSource with id {dataSourceId}. The DataSource was of type ""EXCEL"". However, we did not find an ExcelRawData with DataSourceId {dataSourceId}. This is unexpected.";
                        return new GetRawDataSpreadsheetColumnHeadersResultDTO
                        {
                            WarningMessage = warningMessage,
                        };
                    }
                    var worksheet = ExcelRawDataSpreadsheetSerializer.Deserialize(excelSpreadsheet.SerializedWorksheetContent);
                    var columnHeaders = worksheet.Worksheet.Columns.Select(c => c.Entries[0].ObjectValue().ToString()).ToList();
                    return new GetRawDataSpreadsheetColumnHeadersResultDTO
                    {
                        ColumnHeaders = columnHeaders,
                    };
                });

                if (result.WarningMessage != null)
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWarning, result.WarningMessage);
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{RawDataError}::GetExcelSpreadsheetColumnHeaders - {e.Message}");
                throw;
            }
        }
    }
}
