using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataSourceController : BridgeCareCoreBaseController
    {
        public const string DataSourceError = "DataSource error";

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
        [Authorize]
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DataSourceError}::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertExcelDataSource")]
        [Authorize]
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DataSourceError}::{e.Message}");
                throw;
            }
        }
        [HttpDelete]
        [Route("DeleteDataSource/{dataSourceId}")]
        [Authorize]
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DataSourceError}::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetDataSources")]
        [Authorize]
        public async Task<IActionResult> GetDataSources()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.DataSourceRepo.GetDataSources());
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DataSourceError}::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetDataSource/{dataSourceId}")]
        [Authorize]
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DataSourceError}::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetDataSourceTypes")]
        [Authorize]
        public async Task<IActionResult> GetDataSourceTypes()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.DataSourceRepo.GetDataSourceTypes());
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Treatment error::{e.Message}");
                throw;
            }
        }
    }
}
