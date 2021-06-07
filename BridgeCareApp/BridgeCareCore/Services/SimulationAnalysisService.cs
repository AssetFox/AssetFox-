using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Static;
using AppliedResearchAssociates.Validation;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;

namespace BridgeCareCore.Services
{
    public class SimulationAnalysisService : ISimulationAnalysis
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly IHubService _hubService;

        public SimulationAnalysisService(UnitOfDataPersistenceWork unitOfWork, IHubService hubService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
        }

        private readonly HashSet<string> WarningsSent = new HashSet<string>();
        private Task MessageWritingTask;

        public Task CreateAndRunPermitted(Guid networkId, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
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
            var simulationAnalysisDetail = new SimulationAnalysisDetailDTO
            {
                SimulationId = simulationId,
                LastRun = DateTime.Now,
                Status = "Creating input..."
            };
            _unitOfWork.SimulationAnalysisDetailRepo.UpsertSimulationAnalysisDetail(simulationAnalysisDetail);
            _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);

            var explorer = _unitOfWork.AttributeRepo.GetExplorer();
            var network = _unitOfWork.NetworkRepo.GetSimulationAnalysisNetwork(networkId, explorer);
            _unitOfWork.SimulationRepo.GetSimulationInNetwork(simulationId, network);

            var simulation = network.Simulations.Single(_ => _.Id == simulationId);
            _unitOfWork.InvestmentPlanRepo.GetSimulationInvestmentPlan(simulation);
            _unitOfWork.AnalysisMethodRepo.GetSimulationAnalysisMethod(simulation);
            _unitOfWork.PerformanceCurveRepo.SimulationPerformanceCurves(simulation);
            _unitOfWork.SelectableTreatmentRepo.GetSimulationTreatments(simulation);
            _unitOfWork.CommittedProjectRepo.GetSimulationCommittedProjects(simulation);

            var runner = new SimulationRunner(simulation);

            runner.Failure += (sender, eventArgs) =>
            {
                simulationAnalysisDetail.Status = eventArgs.Message;
                UpdateSimulationAnalysisDetail(simulationAnalysisDetail, DateTime.Now);
                _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastScenarioStatusUpdate, eventArgs.Message, simulationId);
                _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);
            };

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
            runner.Warning += (sender, eventArgs) =>
            {
                var message = eventArgs.Message;
                if (WarningsSent.Add(message))
                {
                    _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastScenarioStatusUpdate, message, simulationId);
                }
            };

            // resetting the report generation status.
            var reportDetailDto = new SimulationReportDetailDTO { SimulationId = simulationId, Status = "" };
            _unitOfWork.SimulationReportDetailRepo.UpsertSimulationReportDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);
            
            RunValidation(runner);
            var channel = Channel.CreateUnbounded<SimulationLogMessageBuilder>();
            MessageWritingTask = WriteMessages(channel.Reader);
            runner.Run(channel.Writer, false);
            channel.Writer.Complete();

            return Task.CompletedTask;
        }

        protected async Task WriteMessages(
            ChannelReader<SimulationLogMessageBuilder> reader
        )
        {
            var writtenMessages = new HashSet<string>();
            while (await reader.WaitToReadAsync())
            {
                var message = await reader.ReadAsync();
                if (writtenMessages.Add(message.Message))
                {
                    var dto = SimulationLogMapper.ToDTO(message);
                    _unitOfWork.SimulationLogRepo.CreateLog(dto);
                }
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
