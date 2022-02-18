using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.Validation;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;

namespace BridgeCareCore.Services
{
    public class SimulationAnalysisService : ISimulationAnalysis
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly IHubService _hubService;
        private readonly SequentialTaskQueue _sequentialTaskQueue;

        public SimulationAnalysisService(UnitOfDataPersistenceWork unitOfWork, IHubService hubService, SequentialTaskQueue sequentialTaskQueue)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
            _sequentialTaskQueue = sequentialTaskQueue ?? throw new ArgumentNullException(nameof(sequentialTaskQueue));
        }

        private readonly HashSet<string> LoggedMessages = new HashSet<string>();

        public Task CreateAndRunPermitted(Guid networkId, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found for given scenario.");
            }

            if (!_unitOfWork.Context.Simulation.Any(_ =>
                _.Id == simulationId && _.SimulationUserJoins.Any(__ => __.UserId == _unitOfWork.UserEntity.Id && __.CanModify)))
            {
                throw new UnauthorizedAccessException("You are not authorized to modify this simulation.");
            }

            return CreateAndRun(networkId, simulationId);
        }

        public Task CreateAndRun(Guid networkId, Guid simulationId)
        {
            Task analysisTask = new(_createAndRun, TaskCreationOptions.LongRunning);
            _sequentialTaskQueue.Add(analysisTask);
            return analysisTask;

            void _createAndRun()
            {
                var simulationAnalysisDetail = new SimulationAnalysisDetailDTO
                {
                    SimulationId = simulationId,
                    LastRun = DateTime.Now,
                    Status = "Creating input..."
                };
                _unitOfWork.SimulationAnalysisDetailRepo.UpsertSimulationAnalysisDetail(simulationAnalysisDetail);
                _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);

                var explorer = _unitOfWork.AttributeRepo.GetExplorer();

                simulationAnalysisDetail.Status = "Getting simulation analysis network";
                UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);
                _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);

                var network = _unitOfWork.NetworkRepo.GetSimulationAnalysisNetwork(networkId, explorer);
                _unitOfWork.SimulationRepo.GetSimulationInNetwork(simulationId, network);

                simulationAnalysisDetail.Status = "Getting investment plan";
                UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);
                _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);

                var simulation = network.Simulations.Single(_ => _.Id == simulationId);
                _unitOfWork.InvestmentPlanRepo.GetSimulationInvestmentPlan(simulation);
                _unitOfWork.AnalysisMethodRepo.GetSimulationAnalysisMethod(simulation);

                simulationAnalysisDetail.Status = "Getting performance curve";
                UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);
                _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);

                _unitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulation);

                simulationAnalysisDetail.Status = "Getting selectable treatments";
                UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);
                _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);

                _unitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulation);
                _unitOfWork.CommittedProjectRepo.GetSimulationCommittedProjects(simulation);

                simulationAnalysisDetail.Status = "Populating calculated fields";
                UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);
                _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);

                _unitOfWork.CalculatedAttributeRepo.PopulateScenarioCalculatedFields(simulation);

                var runner = new SimulationRunner(simulation);

                runner.Progress += (sender, eventArgs) =>
                {
                    switch (eventArgs.ProgressStatus)
                    {
                    case ProgressStatus.Started:
                        simulationAnalysisDetail.Status = "Simulation initializing ...";
                        UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);

                        _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastScenarioStatusUpdate, "Simulation initializing ...", simulationId);
                        break;
                    case ProgressStatus.Running:
                        simulationAnalysisDetail.Status = $"Simulating {eventArgs.Year} - {Math.Round(eventArgs.PercentComplete)}%";
                        UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);

                        _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastScenarioStatusUpdate, simulationAnalysisDetail.Status, simulationId);
                        break;
                    case ProgressStatus.Completed:
                        simulationAnalysisDetail.Status = $"Simulation complete. {100}%";
                        UpdateSimulationAnalysisDetail(simulationAnalysisDetail, DateTime.Now);
                        _unitOfWork.SimulationOutputRepo.CreateSimulationOutput(simulationId, simulation.Results);

                        _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastScenarioStatusUpdate, simulationAnalysisDetail.Status, simulationId);
                        break;
                    }
                    _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);
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
                        _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastScenarioStatusUpdate, eventArgs.MessageBuilder.Message, simulationId);
                        break;
                    case SimulationLogStatus.Error:
                    case SimulationLogStatus.Fatal:
                        simulationAnalysisDetail.Status = message.Message;
                        UpdateSimulationAnalysisDetail(simulationAnalysisDetail, DateTime.Now);
                        _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastScenarioStatusUpdate, eventArgs.MessageBuilder.Message, simulationId);
                        _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);
                        break;
                    }
                };

                // resetting the report generation status.
                var reportDetailDto = new SimulationReportDetailDTO { SimulationId = simulationId, Status = "" };
                _unitOfWork.SimulationReportDetailRepo.UpsertSimulationReportDetail(reportDetailDto);
                _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);

                RunValidation(runner);
                runner.Run(false);
            }
        }

        private void RunValidation(SimulationRunner runner)
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

        private void UpdateSimulationAnalysisDetail(SimulationAnalysisDetailDTO simulationAnalysisDetail, DateTime? stopDateTime)
        {
            if (stopDateTime != null)
            {
                var interval = stopDateTime - simulationAnalysisDetail.LastRun;
                simulationAnalysisDetail.RunTime = interval.Value.ToString(@"hh\:mm\:ss");
            }
            _unitOfWork.SimulationAnalysisDetailRepo.UpsertSimulationAnalysisDetail(simulationAnalysisDetail);
        }
    }
}
