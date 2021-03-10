﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Services.SummaryReport.Parameters;
using BridgeCareCore.Services.SummaryReport.ShortNameGlossary;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MoreLinq;
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
        private readonly IHubContext<BridgeCareHub> HubContext;
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
            HubContext = hub ?? throw new ArgumentNullException(nameof(hub));
            _addGraphsInTabs = addGraphsInTabs ?? throw new ArgumentNullException(nameof(addGraphsInTabs));
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        public byte[] GenerateReport(Guid simulationId, Guid networkId)
        {
            var requiredSections = new HashSet<string>()
            {
                $"{Properties.Resources.DeckSeeded}",
                $"{Properties.Resources.SupSeeded}",
                $"{Properties.Resources.SubSeeded}",
                $"{Properties.Resources.CulvSeeded}",
                $"{Properties.Resources.DeckDurationN}",
                $"{Properties.Resources.SupDurationN}",
                $"{Properties.Resources.SubDurationN}",
                $"{Properties.Resources.CulvDurationN}"
            };


            var reportOutputData = _unitOfDataPersistenceWork.SimulationOutputRepo.GetSimulationOutput(simulationId);

            var initialSectionValues = reportOutputData.InitialSectionSummaries[0].ValuePerNumericAttribute;

            var simulationAnalysisDetail = new SimulationAnalysisDetailDTO
            {
                SimulationId = simulationId,
                Status = "Starting report generation..."
            };

            foreach (var item in requiredSections)
            {
                if (!initialSectionValues.ContainsKey(item))
                {
                    simulationAnalysisDetail.Status = $"The attribute {item} not found in initial section";
                    UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);

                    var broadcastingMessage = $"The attribute {item} not found in initial section";
                    sendRealTimeMessage(broadcastingMessage, simulationId);
                    throw new KeyNotFoundException($"The attribute {item} not found in initial section");
                }
            }

            var sectionValueAttribute = reportOutputData.Years[0].Sections[0].ValuePerNumericAttribute;

            foreach (var item in requiredSections)
            {
                if (!sectionValueAttribute.ContainsKey(item))
                {
                    simulationAnalysisDetail.Status = $"The attribute {item} not found in sections";
                    UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);

                    var broadcastingMessage = $"The attribute {item} not found in sections";
                    sendRealTimeMessage(broadcastingMessage, simulationId);
                    throw new KeyNotFoundException($"The attribute {item} not found in sections");
                }
            }


            // sorting the sections based on facility name. This is helpful throught the report generation process
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

            // In this function, the simulation id is hard coded to 1189 (district 11)
            var yearlyBudgetAmount = _yearlyInvestmentRepository.GetYearlyBudgetAmount(simulationId, simulationYears[0], simulationYears.Count);
            var simulationYearsCount = simulationYears.Count;

            using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo("SummaryReportTestData.xlsx")))
            {
                // Simulation parameters TAB
                var parametersWorksheet = excelPackage.Workbook.Worksheets.Add("Parameters");

                simulationAnalysisDetail.Status = $"Creating Bridge Data TAB";
                UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);

                var broadcastingMessage = $"Creating Bridge Data TAB";
                sendRealTimeMessage(broadcastingMessage, simulationId);

                // Bridge Data TAB
                var worksheet = excelPackage.Workbook.Worksheets.Add("Bridge Data");
                var workSummaryModel = _bridgeDataForSummaryReport.Fill(worksheet, reportOutputData, pennDotReportAData);

                // Filling up parameters tab
                _summaryReportParameters.Fill(parametersWorksheet, simulationYearsCount, workSummaryModel.ParametersModel,
                    simulationId, networkId);

                simulationAnalysisDetail.Status = $"Creating Unfunded recommendations TAB";
                UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);

                broadcastingMessage = $"Creating Unfunded recommendations TAB";
                sendRealTimeMessage(broadcastingMessage, simulationId);
                // Unfunded Recommendations TAB
                var unfundedRecommendationWorksheet = excelPackage.Workbook.Worksheets.Add("Unfunded Recommendations");
                _unfundedRecommendations.Fill(unfundedRecommendationWorksheet, reportOutputData);

                // Simulation Legend TAB
                var shortNameWorksheet = excelPackage.Workbook.Worksheets.Add("Legend");
                _summaryReportGlossary.Fill(shortNameWorksheet);


                simulationAnalysisDetail.Status = $"Creating Bridge work summary TAB";
                UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);

                broadcastingMessage = $"Creating Bridge work summary TAB";
                sendRealTimeMessage(broadcastingMessage, simulationId);
                // Bridge work summary TAB
                var bridgeWorkSummaryWorksheet = excelPackage.Workbook.Worksheets.Add("Bridge Work Summary");
                var chartRowModel = _bridgeWorkSummary.Fill(bridgeWorkSummaryWorksheet, reportOutputData,
                    simulationYears, workSummaryModel, yearlyBudgetAmount);

                simulationAnalysisDetail.Status = $"Creating Bridge work summary By Budget TAB";
                UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);

                broadcastingMessage = $"Creating Bridge work summary By Budget TAB";
                sendRealTimeMessage(broadcastingMessage, simulationId);
                // Bridge work summary by Budget TAB
                var summaryByBudgetWorksheet = excelPackage.Workbook.Worksheets.Add("Bridge Work Summary By Budget");
                _bridgeWorkSummaryByBudget.Fill(summaryByBudgetWorksheet, reportOutputData, simulationYears, yearlyBudgetAmount);


                simulationAnalysisDetail.Status = $"Creating Graph TABs";
                UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);

                broadcastingMessage = $"Creating Graph TABs";
                sendRealTimeMessage(broadcastingMessage, simulationId);

                _addGraphsInTabs.Add(excelPackage, worksheet, bridgeWorkSummaryWorksheet, chartRowModel, simulationYearsCount);

                var folderPathForSimulation = $"DownloadedNewReports\\{simulationId}";
                string relativeFolderPath = Path.Combine(Environment.CurrentDirectory, folderPathForSimulation);
                Directory.CreateDirectory(relativeFolderPath);
                var filePath = Path.Combine(Environment.CurrentDirectory, folderPathForSimulation, "SummaryReportTestData.xlsx");
                byte[] bin = excelPackage.GetAsByteArray();
                File.WriteAllBytes(filePath, bin);

                simulationAnalysisDetail.Status = "Finished generating the summary report.";
                UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);

                return bin;
            }
        }

        private void sendRealTimeMessage(string message, Guid simulationId)
        {
            HubContext
                        .Clients
                        .All
                        .SendAsync("BroadcastSummaryReportGenerationStatus", message, simulationId);
        }

        private void UpdateSimulationAnalysisDetail(SimulationAnalysisDetailDTO simulationAnalysisDetail, DateTime? stopDateTime)
        {
            if (stopDateTime != null)
            {
                var interval = stopDateTime - simulationAnalysisDetail.LastRun;
                simulationAnalysisDetail.RunTime = interval.Value.ToString(@"hh\:mm\:ss");
            }
            _unitOfDataPersistenceWork.SimulationAnalysisDetailRepo.UpsertSimulationAnalysisDetail(simulationAnalysisDetail);
        }
    }
}
