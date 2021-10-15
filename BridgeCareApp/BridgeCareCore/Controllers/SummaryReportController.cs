using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SummaryReportController : BridgeCareCoreBaseController
    {
        private readonly ISummaryReportGenerator _summaryReportGenerator;

        public SummaryReportController(ISummaryReportGenerator summaryReportGenerator, IEsecSecurity esecSecurity,
            UnitOfDataPersistenceWork unitOfWork, IHubService hubService, IHttpContextAccessor httpContextAccessor) :
            base(esecSecurity, unitOfWork, hubService, httpContextAccessor) =>
            _summaryReportGenerator = summaryReportGenerator ?? throw new ArgumentNullException(nameof(summaryReportGenerator));

        [HttpPost]
        [Route("GenerateSummaryReport/{networkId}/{simulationId}")]
        [Authorize]
        public IActionResult GenerateSummaryReport(Guid networkId, Guid simulationId)
        {
            var reportDetailDto = new SimulationReportDetailDTO { SimulationId = simulationId, Status = "Generating" };

            try
            {
                UpdateSimulationAnalysisDetail(reportDetailDto);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);
                _summaryReportGenerator.GenerateReport(networkId, simulationId);

                const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Response.ContentType = contentType;
                HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");

                return Ok();
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

        [HttpPost]
        [Route("DownloadSummaryReport/{networkId}/{simulationId}")]
        [Authorize]
        public async Task<FileResult> DownloadSummaryReport(Guid networkId, Guid simulationId)
        {
            var reportDetailDto = new SimulationReportDetailDTO { SimulationId = simulationId, Status = "Downloading from the server" };

            try
            {
                UpdateSimulationAnalysisDetail(reportDetailDto);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);

                var response = await
                    Task.Factory.StartNew(() =>
                    _summaryReportGenerator.FetchFromFileLocation(networkId, simulationId)
                    );

                const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Response.ContentType = contentType;
                HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");

                var fileContentResult = new FileContentResult(response, contentType)
                {
                    FileDownloadName = "SummaryReport.xlsx"
                };

                reportDetailDto.Status = "Completed";
                UpdateSimulationAnalysisDetail(reportDetailDto);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);

                return fileContentResult;
            }
            catch (Exception e)
            {
                reportDetailDto.Status = $"Failed to download";
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
