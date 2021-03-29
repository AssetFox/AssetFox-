using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
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
    public class SummaryReportController : HubControllerBase
    {
        private readonly IEsecSecurity _esecSecurity;
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly ISummaryReportGenerator _summaryReportGenerator;
        private readonly ILogger<SummaryReportController> _logger;

        public SummaryReportController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfDataPersistenceWork,
            ISummaryReportGenerator summaryReportGenerator, ILogger<SummaryReportController> logger, IHubService hubService) : base(hubService)
        {
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            _unitOfWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
            _summaryReportGenerator = summaryReportGenerator ?? throw new ArgumentNullException(nameof(summaryReportGenerator));
        }

        [HttpPost]
        [Route("GenerateSummaryReport/{networkId}/{simulationId}")]
        [Authorize]
        public async Task<FileResult> GenerateSummaryReport(Guid networkId, Guid simulationId)
        {
            _unitOfWork.SetUser(_esecSecurity.GetUserInformation(Request).Name);
            var reportDetailDto = new SimulationReportDetailDTO {SimulationId = simulationId, Status = "Generating"};

            try
            {
                UpdateSimulationAnalysisDetail(reportDetailDto);
                _hubService.SendRealTimeMessage(HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);

                var response = await Task.Factory.StartNew(() =>
                    _summaryReportGenerator.GenerateReport(networkId, simulationId));

                const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Response.ContentType = contentType;
                HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");

                var fileContentResult = new FileContentResult(response, contentType)
                {
                    FileDownloadName = "SummaryReportTestData.xlsx"
                };

                reportDetailDto.Status = "Completed";
                UpdateSimulationAnalysisDetail(reportDetailDto);
                _hubService.SendRealTimeMessage(HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);

                return fileContentResult;
            }
            catch (Exception e)
            {
                reportDetailDto.Status = $"Failed to generate";
                UpdateSimulationAnalysisDetail(reportDetailDto);
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Summary Report error::{e.Message}");
                _hubService.SendRealTimeMessage(HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);
                throw;
            }
        }

        private void UpdateSimulationAnalysisDetail(SimulationReportDetailDTO dto) =>
            _unitOfWork.SimulationReportDetailRepo.UpsertSimulationReportDetail(dto);
    }
}
