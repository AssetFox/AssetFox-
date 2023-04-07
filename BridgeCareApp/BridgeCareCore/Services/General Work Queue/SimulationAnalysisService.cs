using System;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.WorkQueue;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;

namespace BridgeCareCore.Services
{
    public class SimulationAnalysisService : ISimulationAnalysis
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly SequentialWorkQueue<WorkQueueMetadata> _sequentialWorkQueue;
        public const string NoSimulationFoundForGivenScenario = $"No simulation found for given scenario.";
        public const string YouAreNotAuthorizedToModifyThisSimulation = "You are not authorized to modify this simulation.";

        public SimulationAnalysisService(UnitOfDataPersistenceWork unitOfWork, SequentialWorkQueue<WorkQueueMetadata> sequentialWorkQueue)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _sequentialWorkQueue = sequentialWorkQueue ?? throw new ArgumentNullException(nameof(sequentialWorkQueue));
        }

        public IQueuedWorkHandle<WorkQueueMetadata> CreateAndRunPermitted(Guid networkId, Guid simulationId, UserInfo userInfo, string scenarioName)
        {
            var simulation = _unitOfWork.SimulationRepo.GetSimulation(simulationId);

            if (!simulation.Users.Any(u => u.UserId == _unitOfWork.CurrentUser.Id && u.CanModify))
            {
                throw new UnauthorizedAccessException(YouAreNotAuthorizedToModifyThisSimulation);
            }

            return CreateAndRun(networkId, simulationId, userInfo,  scenarioName);
        }

        public IQueuedWorkHandle<WorkQueueMetadata> CreateAndRun(Guid networkId, Guid simulationId, UserInfo userInfo, string scenarioName)
        {
            AnalysisWorkItem workItem = new(networkId, simulationId, userInfo, scenarioName);
            _sequentialWorkQueue.Enqueue(workItem, out var workHandle).Wait();
            return workHandle;
        }

        public bool Cancel(Guid simulationId)
        {
            return _sequentialWorkQueue.Cancel(simulationId);
        }
    }
}
