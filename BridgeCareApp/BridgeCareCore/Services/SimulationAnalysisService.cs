using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces.Simulation;
using Microsoft.AspNetCore.SignalR;

namespace BridgeCareCore.Services
{
    public class SimulationAnalysisService : ISimulationAnalysis
    {
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;
        private readonly IHubContext<BridgeCareHub> _hubContext;

        public SimulationAnalysisService(UnitOfDataPersistenceWork unitOfDataPersistenceWork, IHubContext<BridgeCareHub> hub)
        {
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
            _hubContext = hub ?? throw new ArgumentNullException(nameof(hub));
        }

        public Task CreateAndRunPermitted(UserInfoDTO userInfo, Guid networkId, Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ =>
                _.Id == simulationId && _.SimulationUserJoins.Any(__ => __.User.Username == userInfo.Sub && __.CanModify)))
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
            _unitOfDataPersistenceWork.SimulationAnalysisDetailRepo.UpsertSimulationAnalysisDetail(simulationAnalysisDetail);
            SendSimulationAnalysisDetail(simulationAnalysisDetail);

            var explorer = _unitOfDataPersistenceWork.AttributeRepo.GetExplorer();
            var network = _unitOfDataPersistenceWork.NetworkRepo.GetSimulationAnalysisNetwork(networkId, explorer);
            _unitOfDataPersistenceWork.SimulationRepo.GetSimulationInNetwork(simulationId, network);

            var simulation = network.Simulations.Single(_ => _.Id == simulationId);
            _unitOfDataPersistenceWork.InvestmentPlanRepo.GetSimulationInvestmentPlan(simulation);
            _unitOfDataPersistenceWork.AnalysisMethodRepo.GetSimulationAnalysisMethod(simulation);
            _unitOfDataPersistenceWork.PerformanceCurveRepo.SimulationPerformanceCurves(simulation);
            _unitOfDataPersistenceWork.SelectableTreatmentRepo.GetSimulationTreatments(simulation);
            _unitOfDataPersistenceWork.CommittedProjectRepo.GetSimulationCommittedProjects(simulation);

            var runner = new SimulationRunner(simulation);

            runner.Failure += (sender, eventArgs) =>
            {
                simulationAnalysisDetail.Status = eventArgs.Message;
                UpdateSimulationAnalysisDetail(simulationAnalysisDetail, DateTime.Now);
                SendRealTimeMessage(eventArgs.Message, simulationId);
                SendSimulationAnalysisDetail(simulationAnalysisDetail);
            };
            runner.Information += (sender, eventArgs) =>
            {
                if (eventArgs.Message == "Simulation complete.")
                {
                    simulationAnalysisDetail.Status = eventArgs.Message;
                    UpdateSimulationAnalysisDetail(simulationAnalysisDetail, DateTime.Now);
                    _unitOfDataPersistenceWork.SimulationOutputRepo.CreateSimulationOutput(simulationId, simulation.Results);
                }
                else
                {
                    simulationAnalysisDetail.Status = eventArgs.Message;
                    UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);
                }

                SendRealTimeMessage(eventArgs.Message, simulationId);
                SendSimulationAnalysisDetail(simulationAnalysisDetail);
            };
            runner.Warning += (sender, eventArgs) =>
            {
                SendRealTimeMessage(eventArgs.Message, simulationId);
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
            _unitOfDataPersistenceWork.SimulationAnalysisDetailRepo.UpsertSimulationAnalysisDetail(simulationAnalysisDetail);
        }

        private void SendRealTimeMessage(string message, Guid simulationId) =>
            _hubContext
                .Clients
                .All
                .SendAsync("BroadcastScenarioStatusUpdate", message, simulationId);

        private void SendSimulationAnalysisDetail(SimulationAnalysisDetailDTO simulationAnalysisDetail) =>
            _hubContext
                .Clients
                .All
                .SendAsync("BroadcastSimulationAnalysisDetail", simulationAnalysisDetail);
    }
}
