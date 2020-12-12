using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces.Simulation;
using Microsoft.AspNetCore.SignalR;
using MoreLinq;

namespace BridgeCareCore.Services.SimulationAnalysis
{
    public class SimulationAnalysis : ISimulationAnalysis
    {
        private readonly IAttributeRepository _attributeRepo;
        private readonly INetworkRepository _networkRepo;
        private readonly IInvestmentPlanRepository _investmentPlanRepo;
        private readonly IAnalysisMethodRepository _analysisMethodRepo;
        private readonly IPerformanceCurveRepository _performanceCurveRepo;
        private readonly ISelectableTreatmentRepository _selectableTreatmentRepo;
        private readonly ISimulationRepository _simulationRepo;
        private readonly ISimulationOutputRepository _simulationOutputRepo;
        private readonly IHubContext<BridgeCareHub> HubContext;

        public SimulationAnalysis(IAttributeRepository attributeRepo, INetworkRepository networkRepo,
            IInvestmentPlanRepository investmentPlanRepo, IAnalysisMethodRepository analysisMethodRepo,
            IPerformanceCurveRepository performanceCurveRepo, ISelectableTreatmentRepository selectableTreatmentRepo,
            ISimulationRepository simulationRepo, ISimulationOutputRepository simulationOutputRepo,
            IHubContext<BridgeCareHub> hub)
        {
            _attributeRepo = attributeRepo ?? throw new ArgumentNullException(nameof(attributeRepo));
            _networkRepo = networkRepo ?? throw new ArgumentNullException(nameof(networkRepo));
            _investmentPlanRepo = investmentPlanRepo ?? throw new ArgumentNullException(nameof(investmentPlanRepo));
            _analysisMethodRepo = analysisMethodRepo ?? throw new ArgumentNullException(nameof(analysisMethodRepo));
            _performanceCurveRepo = performanceCurveRepo ?? throw new ArgumentNullException(nameof(performanceCurveRepo));
            _selectableTreatmentRepo = selectableTreatmentRepo ?? throw new ArgumentNullException(nameof(selectableTreatmentRepo));
            _simulationRepo = simulationRepo ?? throw new ArgumentNullException(nameof(simulationRepo));
            _simulationOutputRepo = simulationOutputRepo ?? throw new ArgumentNullException(nameof(simulationOutputRepo));
            HubContext = hub ?? throw new ArgumentNullException(nameof(hub));
        }
        public void CreateAndRun(Guid networkId, Guid simulationId)
        {
            var explorer = _attributeRepo.GetExplorer();
            var network = _networkRepo.GetSimulationAnalysisNetwork(networkId, explorer);
            _simulationRepo.GetAllInNetwork(network);

            var simulation = network.Simulations.Where(_ => _.Id == simulationId).FirstOrDefault();
            _investmentPlanRepo.GetSimulationInvestmentPlan(simulation);
            _analysisMethodRepo.GetSimulationAnalysisMethod(simulation);
            _performanceCurveRepo.GetSimulationPerformanceCurves(simulation);
            _selectableTreatmentRepo.GetSimulationTreatments(simulation);

            var runner = new SimulationRunner(simulation);

            runner.Failure += (sender, eventArgs) => {
                sendRealTimeMessage(eventArgs.Message, simulationId);
            };
            runner.Information += (sender, eventArgs) => {
                sendRealTimeMessage(eventArgs.Message, simulationId);
            };
            runner.Warning += (sender, eventArgs) => {
                sendRealTimeMessage(eventArgs.Message, simulationId);
            };

            runner.Run();

            _simulationOutputRepo.CreateSimulationOutput(simulationId, simulation.Results);
        }

        public void CreateSimulation(string simulationName)
        {

        }

        public void GetAllSimulations(Guid networkId)
        {
            var explorer = _attributeRepo.GetExplorer();
            var network = _networkRepo.GetSimulationAnalysisNetwork(networkId, explorer);
            _simulationRepo.GetAllInNetwork(network);
        }

        private void sendRealTimeMessage(string message, Guid simulationId)
        {
            HubContext
                        .Clients
                        .All
                        .SendAsync("BroadcastScanarioStatusUpdate", message, simulationId);
        }
    }
}
