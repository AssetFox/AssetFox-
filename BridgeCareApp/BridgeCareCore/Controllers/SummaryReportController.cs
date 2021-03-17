using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SummaryReportController : ControllerBase
    {
        private readonly ILogger<SummaryReportController> _logger;
        private readonly ISummaryReportGenerator _summaryReportGenerator;
        private readonly IHubContext<BridgeCareHub> _hubContext;
        private readonly IEsecSecurity _esecSecurity;
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public SummaryReportController(ISummaryReportGenerator summaryReportGenerator,
            ILogger<SummaryReportController> logger, IHubContext<BridgeCareHub> hub, IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfDataPersistenceWork)
        {
            _summaryReportGenerator = summaryReportGenerator ?? throw new ArgumentNullException(nameof(summaryReportGenerator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hubContext = hub ?? throw new ArgumentNullException(nameof(hub));
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        [HttpPost]
        [Route("GenerateSummaryReport/{networkId}/{simulationId}")]
        [Authorize]
        public async Task<FileResult> GenerateSummaryReport(Guid networkId, Guid simulationId)
        {
            var userInfo = _esecSecurity.GetUserInformation(Request).ToDto();
            var reportDetailDto = new SimulationReportDetailDTO
            {
                SimulationId = simulationId, Status = "Starting report generation..."
            };

            try
            {
                UpdateSimulationAnalysisDetail(reportDetailDto, userInfo);
                SendRealTimeMessage(reportDetailDto);

                var response = await Task.Factory.StartNew(() => _summaryReportGenerator.GenerateReport(networkId, simulationId, userInfo));

                const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Response.ContentType = contentType;
                HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");

                var fileContentResult = new FileContentResult(response, contentType)
                {
                    FileDownloadName = "SummaryReportTestData.xlsx"
                };

                reportDetailDto.Status = "Finished generating the summary report.";
                UpdateSimulationAnalysisDetail(reportDetailDto, userInfo);
                SendRealTimeMessage(reportDetailDto);

                return fileContentResult;
            }
            catch (Exception e)
            {
                reportDetailDto.Status = $"Error::{e.Message}";
                UpdateSimulationAnalysisDetail(reportDetailDto, userInfo);
                SendRealTimeMessage(reportDetailDto);
                throw;
            }
        }

        private void SendRealTimeMessage(SimulationReportDetailDTO dto) =>
            _hubContext
                .Clients
                .All
                .SendAsync("BroadcastSummaryReportGenerationStatus", dto);

        private void UpdateSimulationAnalysisDetail(SimulationReportDetailDTO dto, UserInfoDTO userInfo) =>
            _unitOfDataPersistenceWork.SimulationReportDetailRepo.UpsertSimulationReportDetail(dto, userInfo);
    }
}
