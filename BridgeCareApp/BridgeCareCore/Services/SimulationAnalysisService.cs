﻿using System;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;

namespace BridgeCareCore.Services
{
    public class SimulationAnalysisService : ISimulationAnalysis
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly SequentialWorkQueue _sequentialWorkQueue;

        public SimulationAnalysisService(UnitOfDataPersistenceWork unitOfWork, SequentialWorkQueue sequentialWorkQueue)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _sequentialWorkQueue = sequentialWorkQueue ?? throw new ArgumentNullException(nameof(sequentialWorkQueue));
        }

        public IQueuedWorkHandle CreateAndRunPermitted(Guid networkId, Guid simulationId, UserInfo userInfo)
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

            return CreateAndRun(networkId, simulationId, userInfo);
        }

        public IQueuedWorkHandle CreateAndRun(Guid networkId, Guid simulationId, UserInfo userInfo)
        {
            AnalysisWorkItem workItem = new(networkId, simulationId, userInfo);
            _sequentialWorkQueue.Enqueue(workItem, out var workHandle).Wait();
            return workHandle;
        }
    }
}
