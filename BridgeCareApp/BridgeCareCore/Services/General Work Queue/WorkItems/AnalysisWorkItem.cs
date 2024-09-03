using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.Common.PerformanceMeasurement;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Hubs.Services;
using AppliedResearchAssociates.iAM.Reporting.Logging;
using AppliedResearchAssociates.iAM.WorkQueue;
using AppliedResearchAssociates.Validation;
using BridgeCareCore.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BridgeCareCore.Services;

public record AnalysisWorkItem(Guid NetworkId, Guid SimulationId, UserInfo UserInfo, string ScenarioName) : IWorkSpecification<WorkQueueMetadata>
{
    public string WorkId => WorkQueueWorkIdFactory.CreateId(SimulationId, WorkType.SimulationAnalysis);

    public DateTime StartTime { get; set; }

    public string UserId => UserInfo.Name;

    public string WorkDescription => "Run Simulation";

    public WorkQueueMetadata Metadata => new()
    {
        WorkType = WorkType.SimulationAnalysis,
        DomainType = DomainType.Simulation,
        DomainId = SimulationId
    };

    public string WorkName => ScenarioName;

    public void DoWork(IServiceProvider serviceProvider, Action<string> updateStatusOnHandle, CancellationToken cancellationToken)
    {
        // [REVIEW] This implementation is old, with most of its code written elsewhere when the
        // work item abstraction wasn't even defined yet. This does not currently use the
        // "updateStatusOnHandle" delegate parameter because updates are already handled via hub
        // message-sending. There's no problem with updating this implementation to use the
        // injected delegate; it just requires knowledge of how our update system works
        // end-to-end. (Knowledge which I (WR) lacked at the time of writing.)

        var memos = EventMemoModelLists.GetFreshInstance("Simulation");

        HashSet<string> loggedMessages = new();

        using var scope = serviceProvider.CreateScope();

        var loggingService = scope.ServiceProvider.GetRequiredService<IAnalysisEventLoggingService>();
        void markAndLog(string message)
        {
            memos.Mark(message);
            loggingService.Log(new(SimulationId, ScenarioName, message));
        }

        markAndLog("start");

        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        if (!string.IsNullOrEmpty(UserInfo.Name))
        {
            if (!_unitOfWork.UserRepo.UserExists(UserInfo.Name))
            {
                _unitOfWork.AddUser(UserInfo.Name, UserInfo.HasAdminAccess);

            }

            _unitOfWork.SetUser(UserInfo.Name);

        }
        var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();

        var status = "Creating input...";
        markAndLog("CreatingInput");
        StartTime = DateTime.Now;

        var simulationAnalysisDetail = CreateSimulationAnalysisDetailDto(status, StartTime);

        _unitOfWork.SimulationAnalysisDetailRepo.UpsertSimulationAnalysisDetail(simulationAnalysisDetail);

        _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);

        #region Loading the simulation

        var simulation = AnalysisInputLoading.GetSimulationWithAssets(_unitOfWork, NetworkId, SimulationId,
            () =>
            {
                if (CheckCanceled()) { return false; }
                simulationAnalysisDetail.Status = "Getting simulation analysis network";
                UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);
                _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);
                if (CheckCanceled()) { return false; }
                return true;
            },
            () =>
            {
                markAndLog("GetSimulationAnalysisNetwork");
                return true;
            },
            () =>
            {
                if (CheckCanceled()) { return false; }
                simulationAnalysisDetail.Status = "Getting investment plan";
                UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);
                _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);
                return true;
            },
            () =>
            {
                markAndLog("GetSimulationInvestmentPlan");
                return true;
            },
            () =>
            {
                markAndLog("GetSimulationAnalysisMethod");
                if (CheckCanceled()) { return false; }
                simulationAnalysisDetail.Status = "Getting performance curve";
                UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);
                markAndLog("UpdateSimulationAnalysisDetail");
                _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);
                return true;
            },
            () =>
            {
                markAndLog("GetScenarioPerformanceCurves");
                if (CheckCanceled()) { return false; }
                simulationAnalysisDetail.Status = "Getting selectable treatments";
                UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);
                _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);
                return true;
            },
            () =>
            {
                markAndLog("GetScenarioSelectableTreatments");
                return true;
            },
            () =>
            {
                markAndLog("GetSimulationCommittedProjects");
                if (CheckCanceled()) { return false; }
                simulationAnalysisDetail.Status = "Populating calculated fields";
                UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);
                _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);
                return true;
            });

        if (simulation is null)
        {
            return;
        }

        #endregion

        var runner = new SimulationRunner(simulation);

        var logDtos = new List<SimulationLogDTO>();

        var databaseUsageLock = new object();

        runner.Progress += (sender, eventArgs) =>
        {
            lock (databaseUsageLock)
            {
                switch (eventArgs.ProgressStatus)
                {
                case ProgressStatus.Started:
                    simulationAnalysisDetail.Status = "Simulation initializing...";
                    UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);

                    _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastScenarioStatusUpdate, simulationAnalysisDetail, SimulationId);
                    _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);
                    break;

                case ProgressStatus.Running:
                    simulationAnalysisDetail.Status = $"Simulating {eventArgs.Year} - {Math.Round(eventArgs.PercentComplete)}%";
                    UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);
                    _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastScenarioStatusUpdate, simulationAnalysisDetail, SimulationId);
                    _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);
                    break;

                case ProgressStatus.Completed:
                    simulationAnalysisDetail.Status = "Analysis complete. Preparing to save to database.";
                    UpdateSimulationAnalysisDetail(simulationAnalysisDetail, DateTime.Now);
                    _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);

                    _unitOfWork.SimulationLogRepo.CreateLog(logDtos);

                    var hubServiceLogger = new HubServiceLogger(_hubService, HubConstant.BroadcastScenarioStatusUpdate, _unitOfWork.CurrentUser?.Username);
                    var updateSimulationAnalysisDetailLogger = new CallbackLogger(message => UpdateSimulationAnalysisDetailFromString(message));
                    _unitOfWork.SimulationOutputRepo.CreateSimulationOutputViaJson(SimulationId, simulation.Results);

                    simulationAnalysisDetail.Status = SimulationUserMessages.SimulationOutputSavedToDatabase;
                    UpdateSimulationAnalysisDetail(simulationAnalysisDetail, DateTime.Now);
                    _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastScenarioStatusUpdate, simulationAnalysisDetail, SimulationId);
                    break;

                case ProgressStatus.Canceled:
                    ReportCanceled();
                    break;
                }
                _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);
            }

            markAndLog(simulationAnalysisDetail.Status);
        };

        runner.SimulationLog += (sender, eventArgs) =>
        {
            lock (databaseUsageLock)
            {
                var message = eventArgs.MessageBuilder;
                if (loggedMessages.Add(message.Message))
                {
                    
                    var dto = SimulationLogMessageBuilderMapper.ToDTO(message);
                    logDtos.Add(dto);
                }

                switch (message.Status)
                {
                case SimulationLogStatus.Warning:
                    simulationAnalysisDetail.Status = eventArgs.MessageBuilder.Message;
                    break;

                case SimulationLogStatus.Error:
                    simulationAnalysisDetail.Status = message.Message;
                    break;
                case SimulationLogStatus.Fatal:
                    _unitOfWork.SimulationLogRepo.CreateLog(logDtos);
                    simulationAnalysisDetail.Status = message.Message;
                    UpdateSimulationAnalysisDetail(simulationAnalysisDetail, DateTime.Now);
                    _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);
                    simulationAnalysisDetail.Status = eventArgs.MessageBuilder.Message;
                    _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastScenarioStatusUpdate, simulationAnalysisDetail, SimulationId);
                    break;
                }
            }

            markAndLog(simulationAnalysisDetail.Status);
        };

        // resetting the report generation status.
        var reportDetailDto = new SimulationReportDetailDTO { SimulationId = SimulationId, Status = "" };

        _unitOfWork.SimulationReportDetailRepo.UpsertSimulationReportDetail(reportDetailDto);
        _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto);

        if (CheckCanceled()) { return; }

        RunValidation(runner);

        markAndLog("RunValidation");
        runner.Run(false, cancellationToken);
        markAndLog("Run complete");

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
                SimulationId = SimulationId,
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
            updateStatusOnHandle.Invoke(simulationAnalysisDetail.Status);
        }
    }

    public void OnFault(IServiceProvider serviceProvider, string errorMessage)
    {
        using var scope = serviceProvider.CreateScope();
        var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var message = new SimulationAnalysisDetailDTO()
        {
            SimulationId = SimulationId,
            Status = $"Run Failed. {errorMessage ?? "Unknown status."}",
            LastRun = DateTime.Now
        };
        _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, message);
        _hubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Scenario Error::RunSimulation - {errorMessage}");
        try
        {
            _unitOfWork.SimulationAnalysisDetailRepo.UpsertSimulationAnalysisDetail(message);
        }
        catch (Exception ex)
        {
            _hubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Scenario Error::RunSimulation - {ex.Message}");
        }     
    }

    public void OnCompletion(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
        _hubService.SendRealTimeMessage(UserId, HubConstant.BroadcastTaskCompleted, $"Analysis on {ScenarioName} has completed");
    }

    public void OnUpdate(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
        _hubService.SendRealTimeMessage(UserId, HubConstant.BroadcastWorkQueueUpdate, WorkId);
    }

    private SimulationAnalysisDetailDTO CreateSimulationAnalysisDetailDto(string status, DateTime lastRun) => new()
    {
        SimulationId = SimulationId,
        LastRun = lastRun,
        Status = status,
    };
}
