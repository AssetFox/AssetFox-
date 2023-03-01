using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;

namespace BridgeCareCore.Services
{
    public class SimulationAnalysisService : ISimulationAnalysis
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly SequentialWorkQueue _sequentialWorkQueue;
        public const string NoSimulationFoundForGivenScenario = $"No simulation found for given scenario.";

        public SimulationAnalysisService(UnitOfDataPersistenceWork unitOfWork, SequentialWorkQueue sequentialWorkQueue)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _sequentialWorkQueue = sequentialWorkQueue ?? throw new ArgumentNullException(nameof(sequentialWorkQueue));
        }

        public IQueuedWorkHandle CreateAndRunPermitted(Guid networkId, Guid simulationId, UserInfo userInfo)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException(NoSimulationFoundForGivenScenario);
            }

            if (!_unitOfWork.Context.Simulation.Any(_ =>
                _.Id == simulationId && _.SimulationUserJoins.Any(__ => __.UserId == _unitOfWork.CurrentUser.Id && __.CanModify)))
            {
                throw new UnauthorizedAccessException("You are not authorized to modify this simulation.");
            }

            return CreateAndRun(networkId, simulationId, userInfo);
        }

        public IQueuedWorkHandle CreateAndRun(Guid networkId, Guid simulationId, UserInfo userInfo)
        {
            AnalysisWorkItem workItem = new(networkId, simulationId, userInfo);
            _sequentialWorkQueue.Enqueue(workItem, out var workHandle).Wait();
            return workHandle;
        }

        public bool Cancel(Guid simulationId)
        {
            return _sequentialWorkQueue.Cancel(simulationId);
        }
    }
}
