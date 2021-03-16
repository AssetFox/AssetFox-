using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
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
            var broadcastingMessage = "Starting report generation";
            var userInfo = _esecSecurity.GetUserInformation(Request).ToDto();

            try
            {
                await _hubContext
                    .Clients
                    .All
                    .SendAsync("BroadcastSummaryReportGenerationStatus", broadcastingMessage, simulationId);

                var response = await Task.Factory.StartNew(() => _summaryReportGenerator.GenerateReport(networkId, simulationId, userInfo));

                const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Response.ContentType = contentType;
                HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");

                var fileContentResult = new FileContentResult(response, contentType)
                {
                    FileDownloadName = "SummaryReportTestData.xlsx"
                };

                broadcastingMessage = "Finished generating the summary report.";
                SendRealTimeMessage(broadcastingMessage, simulationId, userInfo);

                return fileContentResult;
            }
            catch (Exception e)
            {
                broadcastingMessage = $"Error::{e.Message}";
                SendRealTimeMessage(broadcastingMessage, simulationId, userInfo);
                throw;
            }
        }

        private void SendRealTimeMessage(string message, Guid simulationId, UserInfoDTO userInfo)
        {
            var dto = new SimulationReportDetailDTO {SimulationId = simulationId, Status = message};

            _hubContext
                .Clients
                .All
                .SendAsync("BroadcastSummaryReportGenerationStatus", dto);

            _unitOfDataPersistenceWork.SimulationReportDetailRepo.UpsertSimulationReportDetail(dto, userInfo);
        }
    }
}
