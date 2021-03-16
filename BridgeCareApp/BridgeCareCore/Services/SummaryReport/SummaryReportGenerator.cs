using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Services.SummaryReport.Parameters;
using BridgeCareCore.Services.SummaryReport.ShortNameGlossary;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using FileSystemRepository = AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem;

namespace BridgeCareCore.Services.SummaryReport
{
    public class SummaryReportGenerator : ISummaryReportGenerator
    {
        private readonly IYearlyInvestmentRepository _yearlyInvestmentRepository;
        private readonly ILogger<SummaryReportGenerator> _logger;
        private readonly IBridgeDataForSummaryReport _bridgeDataForSummaryReport;
        private readonly IPennDotReportARepository _pennDotReportARepository;
        private readonly IUnfundedRecommendations _unfundedRecommendations;
        private readonly IBridgeWorkSummary _bridgeWorkSummary;
        private readonly IBridgeWorkSummaryByBudget _bridgeWorkSummaryByBudget;
        private readonly SummaryReportGlossary _summaryReportGlossary;
        private readonly SummaryReportParameters _summaryReportParameters;
        private readonly IHubContext<BridgeCareHub> _hubContext;
        private readonly IAddGraphsInTabs _addGraphsInTabs;
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public SummaryReportGenerator(IBridgeDataForSummaryReport bridgeDataForSummaryReport,
            ILogger<SummaryReportGenerator> logger,
            IPennDotReportARepository pennDotReportARepository,
            IUnfundedRecommendations unfundedRecommendations,
            IBridgeWorkSummary bridgeWorkSummary, IBridgeWorkSummaryByBudget workSummaryByBudget,
            IYearlyInvestmentRepository yearlyInvestmentRepository,
            SummaryReportGlossary summaryReportGlossary, SummaryReportParameters summaryReportParameters,
            IHubContext<BridgeCareHub> hub,
            IAddGraphsInTabs addGraphsInTabs,
            UnitOfDataPersistenceWork unitOfDataPersistenceWork)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _bridgeDataForSummaryReport = bridgeDataForSummaryReport ?? throw new ArgumentNullException(nameof(bridgeDataForSummaryReport));
            _pennDotReportARepository = pennDotReportARepository ?? throw new ArgumentNullException(nameof(pennDotReportARepository));
            _unfundedRecommendations = unfundedRecommendations ?? throw new ArgumentNullException(nameof(unfundedRecommendations));
            _bridgeWorkSummary = bridgeWorkSummary ?? throw new ArgumentNullException(nameof(bridgeWorkSummary));
            _bridgeWorkSummaryByBudget = workSummaryByBudget ?? throw new ArgumentNullException(nameof(workSummaryByBudget));
            _yearlyInvestmentRepository = yearlyInvestmentRepository ?? throw new ArgumentNullException(nameof(yearlyInvestmentRepository));
            _summaryReportGlossary = summaryReportGlossary ?? throw new ArgumentNullException(nameof(summaryReportGlossary));
            _summaryReportParameters = summaryReportParameters ?? throw new ArgumentNullException(nameof(summaryReportParameters));
            _hubContext = hub ?? throw new ArgumentNullException(nameof(hub));
            _addGraphsInTabs = addGraphsInTabs ?? throw new ArgumentNullException(nameof(addGraphsInTabs));
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        public byte[] GenerateReport(Guid networkId, Guid simulationId, UserInfoDTO userInfo)
        {
            var reportOutputData = _unitOfDataPersistenceWork.SimulationOutputRepo.GetSimulationOutput(simulationId);

            // sorting the sections based on facility name. This is helpful throughout the report generation process
            reportOutputData.InitialSectionSummaries.Sort(
                    (a, b) => int.Parse(a.FacilityName).CompareTo(int.Parse(b.FacilityName))
                    );

            foreach (var yearlySectionData in reportOutputData.Years)
            {
                yearlySectionData.Sections.Sort(
                    (a, b) => int.Parse(a.FacilityName).CompareTo(int.Parse(b.FacilityName))
                    );
            }
            var brKeys = new List<int>();
            var simulationYears = new List<int>();
            foreach (var item in reportOutputData.Years)
            {
                brKeys = item.Sections.Select(_ => Convert.ToInt32(_.FacilityName)).ToList();
                simulationYears.Add(item.Year);
            }
            var pennDotReportAData = _pennDotReportARepository.GetPennDotReportAData(brKeys);

            var yearlyBudgetAmount = _yearlyInvestmentRepository.GetYearlyBudgetAmount(simulationId, simulationYears[0], simulationYears.Count);
            var simulationYearsCount = simulationYears.Count;

            using var excelPackage = new ExcelPackage(new FileInfo("SummaryReportTestData.xlsx"));

            // Simulation parameters TAB
            var parametersWorksheet = excelPackage.Workbook.Worksheets.Add("Parameters");

            var broadcastingMessage = $"Creating Bridge Data TAB";
            SendRealTimeMessage(broadcastingMessage, simulationId, userInfo);

            // Bridge Data TAB
            var worksheet = excelPackage.Workbook.Worksheets.Add("Bridge Data");
            var workSummaryModel = _bridgeDataForSummaryReport.Fill(worksheet, reportOutputData, pennDotReportAData);

            // Filling up parameters tab
            _summaryReportParameters.Fill(parametersWorksheet, simulationYearsCount, workSummaryModel.ParametersModel,
                simulationId, networkId);

            broadcastingMessage = $"Creating Unfunded recommendations TAB";
            SendRealTimeMessage(broadcastingMessage, simulationId, userInfo);
            // Unfunded Recommendations TAB
            var unfundedRecommendationWorksheet = excelPackage.Workbook.Worksheets.Add("Unfunded Recommendations");
            _unfundedRecommendations.Fill(unfundedRecommendationWorksheet, reportOutputData);

            // Simulation Legend TAB
            var shortNameWorksheet = excelPackage.Workbook.Worksheets.Add("Legend");
            _summaryReportGlossary.Fill(shortNameWorksheet);


            broadcastingMessage = $"Creating Bridge work summary TAB";
            SendRealTimeMessage(broadcastingMessage, simulationId, userInfo);
            // Bridge work summary TAB
            var bridgeWorkSummaryWorksheet = excelPackage.Workbook.Worksheets.Add("Bridge Work Summary");
            var chartRowModel = _bridgeWorkSummary.Fill(bridgeWorkSummaryWorksheet, reportOutputData,
                simulationYears, workSummaryModel, yearlyBudgetAmount);


            broadcastingMessage = $"Creating Bridge work summary By Budget TAB";
            SendRealTimeMessage(broadcastingMessage, simulationId, userInfo);
            // Bridge work summary by Budget TAB
            var summaryByBudgetWorksheet = excelPackage.Workbook.Worksheets.Add("Bridge Work Summary By Budget");
            _bridgeWorkSummaryByBudget.Fill(summaryByBudgetWorksheet, reportOutputData, simulationYears, yearlyBudgetAmount);


            broadcastingMessage = $"Creating Graph TABs";
            SendRealTimeMessage(broadcastingMessage, simulationId, userInfo);

            _addGraphsInTabs.Add(excelPackage, worksheet, bridgeWorkSummaryWorksheet, chartRowModel, simulationYearsCount);

            var folderPathForSimulation = $"DownloadedNewReports\\{simulationId}";
            var relativeFolderPath = Path.Combine(Environment.CurrentDirectory, folderPathForSimulation);
            Directory.CreateDirectory(relativeFolderPath);
            var filePath = Path.Combine(Environment.CurrentDirectory, folderPathForSimulation, "SummaryReportTestData.xlsx");
            var bin = excelPackage.GetAsByteArray();
            File.WriteAllBytes(filePath, bin);

            return bin;
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
