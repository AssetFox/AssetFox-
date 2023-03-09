using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using MoreLinq;

namespace BridgeCareCore.Services
{
    public class SimulationQueueService : ISimulationQueueService
    {
        private static IUnitOfWork _unitOfWork;
        private static ISimulationRepository _simulationRepository;
        private ISimulationAnalysis _simulationAnalysis;
        private ISimulationPagingService _simulationService;
        private SequentialWorkQueue _sequentialWorkQueue;

        public SimulationQueueService(IUnitOfWork unitOfWork, SequentialWorkQueue sequentialWorkQueue, ISimulationPagingService simulationService, ISimulationAnalysis simulationAnalysis, ISimulationRepository simulationRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _sequentialWorkQueue = sequentialWorkQueue ?? throw new ArgumentNullException(nameof(sequentialWorkQueue));

            _simulationAnalysis = simulationAnalysis ?? throw new ArgumentNullException(nameof(simulationAnalysis));
            _simulationService = simulationService ?? throw new ArgumentNullException(nameof(simulationService));
            _simulationRepository = simulationRepository ?? throw new ArgumentNullException(nameof(simulationRepository));
        }

        public QueuedSimulationDTO GetQueuedSimulation(Guid simulationId)
        {
            var simulation = _simulationRepository.GetSimulation(simulationId);
            var simulationQueue = _sequentialWorkQueue.Snapshot;
            return simulation.ToQueuedSimulationDTO(simulationQueue);
        }


        public PagingPageModel<QueuedSimulationDTO> GetSimulationQueuePage(PagingRequestModel<QueuedSimulationDTO> request)
        {
            var skip = 0;
            var take = 0;
            var items = new List<QueuedSimulationDTO>();
            var simulationQueue = _sequentialWorkQueue.Snapshot;

            var simulationQueueIds = simulationQueue.Select(_ => new Guid(_.WorkId)).ToList();

            var simulations = _simulationRepository.GetScenariosWithIds(simulationQueueIds)
                .Select(_ => _.ToQueuedSimulationDTO(simulationQueue))
                .ToList();

            if (request.sortColumn.Trim() == "")
            {
                simulations = OrderByColumn(simulations, "queueposition", request.isDescending);
            }
            else
            {
                simulations = OrderByColumn(simulations, request.sortColumn, request.isDescending);
            }

            int totalItemCount = 0;
            if (request.RowsPerPage > 0)
            {
                take = request.RowsPerPage;
                skip = request.RowsPerPage * (request.Page - 1);
                items = simulations.Skip(skip).Take(take).ToList();
                totalItemCount = simulations.Count();
            }
            else
            {
                items = simulations.ToList();
                totalItemCount = items.Count;
            }

            return new PagingPageModel<QueuedSimulationDTO>()
            {
                Items = items,
                TotalItems = totalItemCount
            };
        }

        private List<QueuedSimulationDTO> OrderByColumn(List<QueuedSimulationDTO> simulations, string sortColumn, bool isDescending)
        {
            sortColumn = sortColumn?.ToLower();
            switch (sortColumn)
            {
            case "name":
                if (isDescending)
                    return simulations.OrderByDescending(_ => _.Name.ToLower()).ToList();
                else
                    return simulations.OrderBy(_ => _.Name.ToLower()).ToList();
            case "queueinguser":
                if (isDescending)
                    return simulations.OrderByDescending(_ => _.QueueingUser).ToList();
                else
                    return simulations.OrderBy(_ => _.QueueingUser).ToList();
            case "queueentrytimestamp":
                if (isDescending)
                    return simulations.OrderByDescending(_ => _.QueueEntryTimestamp).ToList();
                else
                    return simulations.OrderBy(_ => _.QueueEntryTimestamp).ToList();
            case "workstartedtimestamp":
                if (isDescending)
                    return simulations.OrderByDescending(_ => _.WorkStartedTimestamp).ToList();
                else
                    return simulations.OrderBy(_ => _.WorkStartedTimestamp).ToList();
            case "queueposition":
                if (isDescending)
                    return simulations.OrderByDescending(_ => _.QueuePosition).ToList();
                else
                    return simulations.OrderBy(_ => _.QueuePosition).ToList();
            }
            return simulations;
        }
    }


    public static class QueuedSimulationTransform
    {
        public static QueuedSimulationDTO ToQueuedSimulationDTO(this SimulationDTO simulationDTO, IReadOnlyList<IQueuedWorkHandle> simulationQueue)
        {
            var queuedWorkHandle = simulationQueue.SingleOrDefault(_ => new Guid(_.WorkId) == simulationDTO.Id);

            if (queuedWorkHandle == null)
            {
                throw new RowNotInTableException("No queued simulation was found for the given id.");
            }

            QueuedSimulationDTO queuedSimulationDTO = new QueuedSimulationDTO()
            {
                Id = simulationDTO.Id,
                Name = simulationDTO.Name,
                PreviousRunTime = simulationDTO.RunTime,
                Status = simulationDTO.Status,

                QueueEntryTimestamp = queuedWorkHandle.QueueEntryTimestamp,
                WorkStartedTimestamp = queuedWorkHandle.WorkStartTimestamp,
                QueueingUser = queuedWorkHandle.UserInfo.Name,
                QueuePosition = queuedWorkHandle.QueueIndex,
            };

            if (queuedWorkHandle.WorkHasStarted)
            {
                queuedSimulationDTO.CurrentRunTime = DateTime.Now.Subtract(queuedSimulationDTO.WorkStartedTimestamp.Value).ToString(@"hh\:mm\:ss", null);
            }
            else
            {
                queuedSimulationDTO.WorkStartedTimestamp = null;
                queuedSimulationDTO.CurrentRunTime = null;
                queuedSimulationDTO.Status = $"Queued to run.";
            }

            return queuedSimulationDTO;
        }
    }
}
