using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem;
using BridgeCareCore.Interfaces.SummaryReport;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport
{
    public class SummaryReportGenerator : ISummaryReportGenerator
    {
        private readonly ISimulationOutputRepository _simulationOutputFileRepo;
        private readonly ILogger<SummaryReportGenerator> _logger;
        private readonly IBridgeDataForSummaryReport _bridgeDataForSummaryReport;
        private readonly IPennDotReportARepository _pennDotReportARepository;
        private readonly IUnfundedRecommendations _unfundedRecommendations;

        public SummaryReportGenerator(ISimulationOutputRepository simulationOutputFileRepo,
            IBridgeDataForSummaryReport bridgeDataForSummaryReport,
            ILogger<SummaryReportGenerator> logger,
            IPennDotReportARepository pennDotReportARepository,
            IUnfundedRecommendations unfundedRecommendations)
        {
            _simulationOutputFileRepo = simulationOutputFileRepo ?? throw new ArgumentNullException(nameof(simulationOutputFileRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _bridgeDataForSummaryReport = bridgeDataForSummaryReport ?? throw new ArgumentNullException(nameof(bridgeDataForSummaryReport));
            _pennDotReportARepository = pennDotReportARepository ?? throw new ArgumentNullException(nameof(pennDotReportARepository));
            _unfundedRecommendations = unfundedRecommendations ?? throw new ArgumentNullException(nameof(unfundedRecommendations));
        }

        public byte[] GenerateReport(Guid networkId, Guid simulationId)
        {
            var reportOutputData = _simulationOutputFileRepo.GetSimulationResults(networkId, simulationId);
            var brKeys = new List<int>();
            foreach (var item in reportOutputData.Years)
            {
                brKeys = item.Sections.Select(_ => Convert.ToInt32(_.FacilityName)).ToList();
            }
            var pennDotReportAData = _pennDotReportARepository.GetPennDotReportAData(brKeys);

            using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo("SummaryReportTestData.xlsx")))
            {
                // Bridge Data TAB
                var worksheet = excelPackage.Workbook.Worksheets.Add("Bridge Data");
                var workSummaryModel = _bridgeDataForSummaryReport.Fill(worksheet, reportOutputData, pennDotReportAData);

                // Unfunded Recommendations TAB
                var unfundedRecommendationWorksheet = excelPackage.Workbook.Worksheets.Add("Unfunded Recommendations");
                _unfundedRecommendations.Fill(unfundedRecommendationWorksheet, reportOutputData);

                var folderPathForSimulation = $"DownloadedNewReports\\{simulationId}";
                string relativeFolderPath = Path.Combine(Environment.CurrentDirectory, folderPathForSimulation);
                Directory.CreateDirectory(relativeFolderPath);
                var filePath = Path.Combine(Environment.CurrentDirectory, folderPathForSimulation, "SummaryReportTestData.xlsx");
                byte[] bin = excelPackage.GetAsByteArray();
                File.WriteAllBytes(filePath, bin);

                return bin;
            }
        }
    }
}
