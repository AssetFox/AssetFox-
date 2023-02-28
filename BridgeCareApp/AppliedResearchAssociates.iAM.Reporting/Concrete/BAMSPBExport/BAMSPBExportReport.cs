﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Reporting.Services;
using BridgeCareCore.Services;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class BAMSPBExportReport : IReport
    {
        protected readonly IHubService _hubService;
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private Guid _networkId;        
        private readonly ReportHelper _reportHelper;

        public BAMSPBExportReport(UnitOfDataPersistenceWork unitOfWork, string name, ReportIndexDTO results, IHubService hubService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
            ReportTypeName = name;
            _reportHelper = new ReportHelper();

            // Check for existing report id
            var reportId = results?.Id; if (reportId == null) { reportId = Guid.NewGuid(); }

            // Set report return default parameters
            ID = (Guid)reportId;
            Errors = new List<string>();
            Warnings = new List<string>();
            Status = "Report definition created.";
            Results = string.Empty;
            IsComplete = false;
        }

        public Guid ID { get; set; }

        public Guid? SimulationID { get; set; }

        public string Results { get; private set; }

        public ReportType Type => ReportType.File;

        public string ReportTypeName { get; private set; }

        public List<string> Errors { get; private set; }

        public List<string> Warnings { get; set; }

        public bool IsComplete { get; private set; }

        public string Status { get; private set; }

        public async Task Run(string parameters)
        {
            // Check for the parameters
            if (string.IsNullOrEmpty(parameters) || string.IsNullOrWhiteSpace(parameters))
            {
                Errors.Add("Parameters string is empty OR there are no parameters defined");
                IndicateError();
                return;
            }

            // Set simulation id
            if (!Guid.TryParse(parameters, out Guid _simulationId))
            {
                Errors.Add("Simulation ID could not be parsed to a Guid");
                IndicateError();
                return;
            }
            SimulationID = _simulationId;

            var simulationName = string.Empty;
            try
            {
                var simulationObject = _unitOfWork.SimulationRepo.GetSimulation(_simulationId);
                simulationName = simulationObject.Name;
                _networkId = simulationObject.NetworkId;
            }
            catch (Exception e)
            {
                IndicateError();
                Errors.Add("Failed to find simulation");
                Errors.Add(e.Message);
                return;
            }

            // Check for simulation existence                        
            if (simulationName == null)
            {
                IndicateError();
                Errors.Add($"Failed to find name using simulation ID {_simulationId}.");
                return;
            }

            // Generate report 
            var reportPath = string.Empty;
            try
            {
                reportPath = GenerateBAMSPBExportReport(_networkId, _simulationId);
            }
            catch (Exception e)
            {
                IndicateError();
                Errors.Add("Failed to generate BAMS PB Export report");
                Errors.Add(e.Message);
                return;
            }

            if (string.IsNullOrEmpty(reportPath) || string.IsNullOrWhiteSpace(reportPath))
            {
                Errors.Add("BAMS PB Export report path is missing or not set");
                IndicateError();
                return;
            }

            // Report success with location of file
            Results = reportPath;
            IsComplete = true;
            Status = "File generated.";
            return;
        }

        private string GenerateBAMSPBExportReport(Guid networkId, Guid simulationId)
        {
            var reportPath = string.Empty;
            var reportDetailDto = new SimulationReportDetailDTO
            {
                SimulationId = simulationId,
                Status = $"Generating..."
            };
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);

            var logger = new CallbackLogger(str => UpdateSimulationAnalysisDetailWithStatus(reportDetailDto, str));
            var simulationOutput = _unitOfWork.SimulationOutputRepo.GetSimulationOutputViaJson(simulationId);

            // Sort data
            simulationOutput.InitialAssetSummaries.Sort(
                    (a, b) => _reportHelper.CheckAndGetValue<double>(a.ValuePerNumericAttribute, "BRKEY_").CompareTo(_reportHelper.CheckAndGetValue<double>(b.ValuePerNumericAttribute, "BRKEY_"))
                    );

            foreach (var yearlySectionData in simulationOutput.Years)
            {
                yearlySectionData.Assets.Sort(
                    (a, b) => _reportHelper.CheckAndGetValue<double>(a.ValuePerNumericAttribute, "BRKEY_").CompareTo(_reportHelper.CheckAndGetValue<double>(b.ValuePerNumericAttribute, "BRKEY_"))
                    );
            }

            var explorer = _unitOfWork.AttributeRepo.GetExplorer();
            var network = _unitOfWork.NetworkRepo.GetSimulationAnalysisNetwork(networkId, explorer);
            _unitOfWork.SimulationRepo.GetSimulationInNetwork(simulationId, network);
            var simulation = network.Simulations.First();
            // Get required data for report..


            // Report
            using var excelPackage = new ExcelPackage(new FileInfo("BAMSPBExportReportData.xlsx"));

            // Tabs..


            // Check and generate folder
            reportDetailDto.Status = $"Creating Report file";
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);
            var folderPathForSimulation = $"Reports\\{simulationId}";
            Directory.CreateDirectory(folderPathForSimulation);
            reportPath = Path.Combine(folderPathForSimulation, "BAMSPBExportReport.xlsx");

            var bin = excelPackage.GetAsByteArray();
            File.WriteAllBytes(reportPath, bin);

            reportDetailDto.Status = $"Report generation completed";
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);

            return reportPath;
        }

        private void UpdateSimulationAnalysisDetail(SimulationReportDetailDTO dto) => _unitOfWork.SimulationReportDetailRepo.UpsertSimulationReportDetail(dto);

        private void UpdateSimulationAnalysisDetailWithStatus(SimulationReportDetailDTO dto, string message)
        {
            dto.Status = message;
            UpdateSimulationAnalysisDetail(dto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, dto.Status, dto.SimulationId);
        }

        private void IndicateError()
        {
            Status = "BAMS PB Export output report completed with errors";
            IsComplete = true;
        }
    }
}
