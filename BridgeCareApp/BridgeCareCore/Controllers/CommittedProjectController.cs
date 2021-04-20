using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommittedProjectController : HubControllerBase
    {
        private static IEsecSecurity _esecSecurity;
        private static ICommittedProjectService _committedProjectService;

        public CommittedProjectController(IEsecSecurity esecSecurity, ICommittedProjectService committedProjectService,
            IHubService hubService) : base(hubService)
        {
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            _committedProjectService = committedProjectService ??
                                       throw new ArgumentNullException(nameof(committedProjectService));
        }

        [HttpPost]
        [Route("ImportCommittedProjects")]
        [Authorize]
        public async Task<IActionResult> ImportCommittedProjects()
        {
            try
            {
                await Task.Factory.StartNew(() => _committedProjectService.ImportCommittedProjectFiles(Request));
                return Ok();
            }
            catch (Exception e)
            {
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Committed Project error::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("ExportCommittedProjects/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> ExportCommittedProjects(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() =>
                    _committedProjectService.ExportCommittedProjectsFile(Request, simulationId));

                /*return new FileContentResult(result.Item2, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = result.Item1
                };*/
                return Ok(new FileInfoDTO
                {
                    FileData = Convert.ToBase64String(result.Item2),
                    FileName = result.Item1,
                    MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                });
            }
            catch (Exception e)
            {
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Committed Project error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteCommittedProjects/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> DeleteCommittedProjects(Guid simulationId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                    _committedProjectService.DeleteCommittedProjects(Request, simulationId));

                return Ok();
            }
            catch (Exception e)
            {
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Committed Project error::{e.Message}");
                throw;
            }
        }
    }
}
