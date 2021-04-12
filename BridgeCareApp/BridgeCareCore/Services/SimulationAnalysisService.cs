using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Interfaces.Simulation;

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
                Status = "Starting analysis..."
            };
            _unitOfWork.SimulationAnalysisDetailRepo.UpsertSimulationAnalysisDetail(simulationAnalysisDetail);
            _hubService.SendRealTimeMessage(HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);

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
                _hubService.SendRealTimeMessage(HubConstant.BroadcastScenarioStatusUpdate, eventArgs.Message, simulationId);
                _hubService.SendRealTimeMessage(HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);
            };

            runner.Progress += (sender, eventArgs) =>
            {
                switch (eventArgs.ProgressStatus)
                {
                case ProgressStatus.Started:
                    simulationAnalysisDetail.Status = "Simulation initializing ...";
                    UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);

                    _hubService.SendRealTimeMessage(HubConstant.BroadcastScenarioStatusUpdate, "Simulation initializing ...", simulationId);
                    break;
                case ProgressStatus.Running:
                    simulationAnalysisDetail.Status = $"Simulating {eventArgs.Year} - {Math.Round(eventArgs.PercentComplete, 2)}%";
                    UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);

                    _hubService.SendRealTimeMessage(HubConstant.BroadcastScenarioStatusUpdate, simulationAnalysisDetail.Status, simulationId);
                    break;
                case ProgressStatus.Completed:
                    simulationAnalysisDetail.Status = $"Simulation complete. {100}%";
                    UpdateSimulationAnalysisDetail(simulationAnalysisDetail, DateTime.Now);
                    _unitOfWork.SimulationOutputRepo.CreateSimulationOutput(simulationId, simulation.Results);

                    _hubService.SendRealTimeMessage(HubConstant.BroadcastScenarioStatusUpdate, simulationAnalysisDetail.Status, simulationId);
                    break;
                }
                _hubService.SendRealTimeMessage(HubConstant.BroadcastSimulationAnalysisDetail, simulationAnalysisDetail);
            };
            runner.Warning += (sender, eventArgs) =>
            {
                _hubService.SendRealTimeMessage(HubConstant.BroadcastScenarioStatusUpdate, eventArgs.Message, simulationId);
            };

            runner.Run();

            return Task.CompletedTask;
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
