﻿using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Reporting;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimulationLogController : BridgeCareCoreBaseController
    {
        public SimulationLogController(
            IEsecSecurity esecSecurity,
            UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService,
            IHttpContextAccessor httpContextAccessor) :
            base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            
        }
            

        [HttpPost]
        [Route("GetSimulationLog/{networkId}/{simulationId}")]
        [Authorize]
        public async Task<FileResult> GetSimulationLog(Guid networkId, Guid simulationId)
        {

            var reportDetailDto = new SimulationReportDetailDTO { SimulationId = simulationId, Status = "Generating" };

            try
            {
                UpdateSimulationAnalysisDetail(reportDetailDto);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);

                var logDtos = await UnitOfWork.SimulationLogRepo.GetLog(simulationId);
                var log = SimulationLogReport.ToLog(logDtos);
                var bytes = Encoding.Unicode.GetBytes(log);
                const string contentType = "text/plain";
                HttpContext.Response.ContentType = contentType;
                HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");

                var fileContentResult = new FileContentResult(bytes, contentType)
                {
                    FileDownloadName = "SimulationLog.txt"
                };

                reportDetailDto.Status = "Completed";
                UpdateSimulationAnalysisDetail(reportDetailDto);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);

                return fileContentResult;
            }
            catch (Exception e)
            {
                reportDetailDto.Status = $"Failed to generate";
                UpdateSimulationAnalysisDetail(reportDetailDto);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Summary Report error::{e.Message}");
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);
                throw;
            }
        }

        private void UpdateSimulationAnalysisDetail(SimulationReportDetailDTO dto) =>
            UnitOfWork.SimulationReportDetailRepo.UpsertSimulationReportDetail(dto);
    }
}