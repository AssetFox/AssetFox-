using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using BridgeCareCore.Controllers.BaseController;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Models.Validation;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Linq;
using BridgeCareCore.Security;
using AppliedResearchAssociates.iAM.Hubs.Services;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataSourceController : BridgeCareCoreBaseController
    {
        public const string DataSourceError = "DataSource Error";

        public DataSourceController(
            IEsecSecurity esecSecurity,
            IUnitOfWork unitOfWork,
            IHubService hubService,
            IHttpContextAccessor contextAccessor)
            : base(esecSecurity,
                  unitOfWork,
                  hubService,
                  contextAccessor)
        {
        }

        [HttpPost]
        [Route("UpsertSqlDataSource")]
        [ClaimAuthorize("DataSourceModifyAccess")]
        public async Task<IActionResult> UpsertSqlDataSource(SQLDataSourceDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.DataSourceRepo.UpsertDatasource(dto);
                });
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DataSourceError}::UpsertSqlDataSource - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertExcelDataSource")]
        [ClaimAuthorize("DataSourceModifyAccess")]
        public async Task<IActionResult> UpsertExcelDataSource(ExcelDataSourceDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.DataSourceRepo.UpsertDatasource(dto);
                });
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DataSourceError}::UpsertExcelDataSource - {e.Message}");
                throw;
            }
        }
        [HttpDelete]
        [Route("DeleteDataSource/{dataSourceId}")]
        [ClaimAuthorize("DataSourceModifyAccess")]
        public async Task<IActionResult> DeleteDataSource(Guid dataSourceId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.DataSourceRepo.DeleteDataSource(dataSourceId);
                });

                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DataSourceError}::DeleteDataSource - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetDataSources")]
        [ClaimAuthorize("DataSourceViewAccess")]
        public async Task<IActionResult> GetDataSources()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.DataSourceRepo.GetDataSources());
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DataSourceError}::GetDataSources - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetDataSource/{dataSourceId}")]
        [ClaimAuthorize("DataSourceViewAccess")]
        public async Task<IActionResult> GetDataSource(Guid dataSourceId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.DataSourceRepo.GetDataSource(dataSourceId));
                if (result is SQLDataSourceDTO)
                {
                    return Ok((SQLDataSourceDTO)result);
                }
                else if (result is ExcelDataSourceDTO)
                {
                    return Ok((ExcelDataSourceDTO)result);
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DataSourceError}::GetDataSource - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetDataSourceTypes")]
        [ClaimAuthorize("DataSourceViewAccess")]
        public Task<IActionResult> GetDataSourceTypes()
        {
            try
            {
                var dataSourceArray = (DataSourceTypeStrings[])Enum.GetValues(typeof(DataSourceTypeStrings));
                //All and None are internal data types we do not want to expose to the UI
                var result = dataSourceArray.Where(q => q.ToString() != "All" && q.ToString() != "None")
                    .Select(v => v.ToString()).ToList();
                return Task.FromResult<IActionResult>(Ok(result));
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Treatment Error::GetDataSourceTypes - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("CheckSqlConnection")]
        [ClaimAuthorize("DataSourceViewAccess")]
        public async Task<IActionResult> CheckSqlConnection(TestStringData stringData)
        {
            try
            {
                try
                {
                    await Task.Factory.StartNew(() =>
                    {
                        using (SqlConnection conn = new SqlConnection(stringData.testString))
                        {
                            conn.Open(); // throws if invalid
                        }
                    });
                    return Ok(new ValidationResult() { IsValid = true, ValidationMessage = "Connection string is valid"});
                }
                catch (Exception ex)
                {
                    return Ok(new ValidationResult() { IsValid = false, ValidationMessage = "Connection string is not valid: " + ex.Message });
                }

            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Attribute Error::CheckSqlConnection - {e.Message}");
                throw;
            }
        }
    }
}
