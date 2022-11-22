using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Hubs.Services;
using AppliedResearchAssociates.iAM.Reporting.Logging;
using AppliedResearchAssociates.Validation;
using BridgeCareCore.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BridgeCareCore.Services
{
    public record AnalysisWorkItem(Guid networkId, Guid simulationId, UserInfo userInfo) : IWorkItem
    {
        public string WorkId => simulationId.ToString();

        public DateTime StartTime { get; set; }

        public UserInfo UserInfo { get; } = userInfo;

        public void DoWork(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            HashSet<string> LoggedMessages = new();

            using var scope = serviceProvider.CreateScope();

            var _unitOfWork = scope.ServiceProvider.GetRequiredService<UnitOfDataPersistenceWork>();
            if (!string.IsNullOrEmpty(userInfo.Name))
            {
                if (!_unitOfWork.Context.User.Any(_ => _.Username == userInfo.Name))
                {
                    _unitOfWork.AddUser(userInfo.Name, userInfo.HasAdminAccess);
                }

                _unitOfWork.SetUser(userInfo.Name);
            }
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();

            var status = "Creating input...";
            StartTime = DateTime.Now;
            var simulationAnalysisDetail = CreateSimulationAnalysisDetailDto(status, StartTime);

            _unitOfWork.SimulationAnalysisDetailRepo.UpsertSimulationAnalysisDetail(simulationAnalysisDetail);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);

            var explorer = _unitOfWork.AttributeRepo.GetExplorer();

            if (CheckCanceled()) { return; }
            simulationAnalysisDetail.Status = "Getting simulation analysis network";
            UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);

            if (CheckCanceled()) { return; }
            var network = _unitOfWork.NetworkRepo.GetSimulationAnalysisNetwork(networkId, explorer);
            _unitOfWork.SimulationRepo.GetSimulationInNetwork(simulationId, network);

            if (CheckCanceled()) { return; }
            simulationAnalysisDetail.Status = "Getting investment plan";
            UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);

            var simulation = network.Simulations.Single(_ => _.Id == simulationId);
            _unitOfWork.InvestmentPlanRepo.GetSimulationInvestmentPlan(simulation);
            _unitOfWork.AnalysisMethodRepo.GetSimulationAnalysisMethod(simulation, _unitOfWork.CurrentUser?.CriterionLibrary?.MergedCriteriaExpression);

            if (CheckCanceled()) { return; }
            simulationAnalysisDetail.Status = "Getting performance curve";
            UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);

            _unitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulation);

            if (CheckCanceled()) { return; }
            simulationAnalysisDetail.Status = "Getting selectable treatments";
            UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);

            _unitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulation);
            _unitOfWork.CommittedProjectRepo.GetSimulationCommittedProjects(simulation);

            if (CheckCanceled()) { return; }
            simulationAnalysisDetail.Status = "Populating calculated fields";
            UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);

            _unitOfWork.CalculatedAttributeRepo.PopulateScenarioCalculatedFields(simulation);

            var runner = new SimulationRunner(simulation);

            runner.Progress += (sender, eventArgs) =>
            {
                switch (eventArgs.ProgressStatus)
                {
                case ProgressStatus.Started:
                    simulationAnalysisDetail.Status = "Simulation initializing...";
                    UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);

                    _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastScenarioStatusUpdate, simulationAnalysisDetail, simulationId);
                    _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);
                    break;

                case ProgressStatus.Running:
                    simulationAnalysisDetail.Status = $"Simulating {eventArgs.Year} - {Math.Round(eventArgs.PercentComplete)}%";
                    UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);

                    _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastScenarioStatusUpdate, simulationAnalysisDetail, simulationId);
                    _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);
                    break;

                case ProgressStatus.Completed:
                    simulationAnalysisDetail.Status = $"Analysis complete. Preparing to save to database.";
                    UpdateSimulationAnalysisDetail(simulationAnalysisDetail, DateTime.Now);
                    _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);
                    var hubServiceLogger = new HubServiceLogger(_hubService, HubConstant.BroadcastScenarioStatusUpdate, _unitOfWork.CurrentUser?.Username);
                    var updateSimulationAnalysisDetailLogger = new CallbackLogger(message => UpdateSimulationAnalysisDetailFromString(message));

                    _unitOfWork.SimulationOutputRepo.CreateSimulationOutput(simulationId, simulation.Results, updateSimulationAnalysisDetailLogger);

                    _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastScenarioStatusUpdate, simulationAnalysisDetail, simulationId);
                    break;

                case ProgressStatus.Canceled:
                    ReportCanceled();
                    break;
                }
                _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);
            };

            runner.SimulationLog += (sender, eventArgs) =>
            {
                var message = eventArgs.MessageBuilder;
                if (LoggedMessages.Add(message.Message))
                {
                    var dto = SimulationLogMapper.ToDTO(message);
                    _unitOfWork.SimulationLogRepo.CreateLog(dto);
                }
                switch (message.Status)
                {
                case SimulationLogStatus.Warning:
                    simulationAnalysisDetail.Status = eventArgs.MessageBuilder.Message;
                    _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastScenarioStatusUpdate, simulationAnalysisDetail, simulationId);
                    _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);
                    break;

                case SimulationLogStatus.Error:
                case SimulationLogStatus.Fatal:
                    simulationAnalysisDetail.Status = message.Message;
                    UpdateSimulationAnalysisDetail(simulationAnalysisDetail, DateTime.Now);
                    _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);
                    simulationAnalysisDetail.Status = eventArgs.MessageBuilder.Message;
                    _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastScenarioStatusUpdate, simulationAnalysisDetail, simulationId);
                    break;
                }
            };

            // resetting the report generation status.
            var reportDetailDto = new SimulationReportDetailDTO { SimulationId = simulationId, Status = "" };
            _unitOfWork.SimulationReportDetailRepo.UpsertSimulationReportDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto);

            if (CheckCanceled()) { return; }
            RunValidation(runner);

            runner.Run(false, cancellationToken);

            void RunValidation(SimulationRunner runner)
            {
                var validationResults = runner.Simulation.GetAllValidationResults(Enumerable.Empty<string>());
                _unitOfWork.SimulationLogRepo.ClearLog(runner.Simulation.Id);
                var simulationLogDtos = new List<SimulationLogDTO>();
                foreach (var result in validationResults)
                {
                    var breadcrumb = string.Join(".", result.Target.ValidationPath);
                    var simulationLogDto = new SimulationLogDTO
                    {
                        Message = $"{result.Message} {breadcrumb}",
                        Status = (int)result.Status,
                        SimulationId = runner.Simulation.Id,
                        Subject = (int)SimulationLogSubject.Validation,
                    };
                    simulationLogDtos.Add(simulationLogDto);
                }
                _unitOfWork.SimulationLogRepo.CreateLogs(simulationLogDtos);
                runner.HandleValidationFailures(validationResults);
            }

            bool CheckCanceled()
            {
                var canceled = cancellationToken.IsCancellationRequested;
                if (canceled)
                {
                    ReportCanceled();
                }
                return canceled;
            }

            void ReportCanceled()
            {
                var detail = new SimulationAnalysisDetailDTO
                {
                    SimulationId = simulationId,
                    Status = "Canceled",
                    LastRun = StartTime,
                };
                UpdateSimulationAnalysisDetail(detail, DateTime.Now);
                _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, detail);
            }

            void UpdateSimulationAnalysisDetailFromString(string message)
            {
                var detail = CreateSimulationAnalysisDetailDto(message, StartTime);
                UpdateSimulationAnalysisDetail(detail, DateTime.Now);
                _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, detail);
            }

            void UpdateSimulationAnalysisDetail(SimulationAnalysisDetailDTO simulationAnalysisDetail, DateTime? stopDateTime)
            {
                if (stopDateTime != null)
                {
                    var interval = stopDateTime - simulationAnalysisDetail.LastRun;
                    simulationAnalysisDetail.RunTime = interval.Value.ToString(@"hh\:mm\:ss");
                }
                _unitOfWork.SimulationAnalysisDetailRepo.UpsertSimulationAnalysisDetail(simulationAnalysisDetail);
            }
        }

        private SimulationAnalysisDetailDTO CreateSimulationAnalysisDetailDto(string status, DateTime lastRun) => new SimulationAnalysisDetailDTO
        {
            SimulationId = simulationId,
            LastRun = lastRun,
            Status = status,
        };
    }
}
