using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
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
    public class DataSourceController : BridgeCareCoreBaseController
    {
        public const string DataSourceError = "DataSource error";

        public DataSourceController(
            IEsecSecurity esecSecurity,
            UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService,
            IHttpContextAccessor contextAccessor)
            : base(esecSecurity,
                  unitOfWork,
                  hubService,
                  contextAccessor)
        {
        }

        [HttpPost]
        [Route("UpsertDataSource")]
        [Authorize]
        public async Task<IActionResult> UpsertDataSource(BaseDataSourceDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.DataSourceRepo.UpsertDatasource(dto);
                    UnitOfWork.Commit();
                });
                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
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
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.DataSourceRepo.DeleteDataSource(dataSourceId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DataSourceError}::{e.Message}");
                throw;
            }
        }
    }
}
