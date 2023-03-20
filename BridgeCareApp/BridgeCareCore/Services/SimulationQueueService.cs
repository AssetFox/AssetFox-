using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.WorkQueue;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using Microsoft.CodeAnalysis.Operations;
using MoreLinq;

namespace BridgeCareCore.Services
{
    public class WorkQueueService : IWorkQueueService
    {
        private static IUnitOfWork _unitOfWork;
        private static ISimulationRepository _simulationRepository;
        private SequentialWorkQueue _sequentialWorkQueue;

        public WorkQueueService(IUnitOfWork unitOfWork, SequentialWorkQueue sequentialWorkQueue, ISimulationRepository simulationRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _sequentialWorkQueue = sequentialWorkQueue ?? throw new ArgumentNullException(nameof(sequentialWorkQueue));

            _simulationRepository = simulationRepository ?? throw new ArgumentNullException(nameof(simulationRepository));
        }


        public PagingPageModel<QueuedWorkDTO> GetWorkQueuePage(PagingRequestModel<QueuedWorkDTO> request)
        {
            var skip = 0;
            var take = 0;
            var items = new List<QueuedWorkDTO>();
            var workQueue = _sequentialWorkQueue.Snapshot;

            var queuedWork = GetQueuedWork(workQueue);

            if (request.sortColumn.Trim() == "")
            {
                queuedWork = OrderByColumn(queuedWork, "queueposition", request.isDescending);
            }
            else
            {
                queuedWork = OrderByColumn(queuedWork, request.sortColumn, request.isDescending);
            }

            int totalItemCount = 0;
            if (request.RowsPerPage > 0)
            {
                take = request.RowsPerPage;
                skip = request.RowsPerPage * (request.Page - 1);
                items = queuedWork.Skip(skip).Take(take).ToList();
                totalItemCount = queuedWork.Count();
            }
            else
            {
                items = queuedWork.ToList();
                totalItemCount = items.Count;
            }

            return new PagingPageModel<QueuedWorkDTO>()
            {
                Items = items,
                TotalItems = totalItemCount
            };
        }

        private List<QueuedWorkDTO> GetQueuedWork(IReadOnlyList<IQueuedWorkHandle> workQueue)
        {
            var simulationAnalysisIds = workQueue.Where(_ => _.WorkType == WorkType.SimulationAnalyis).Select(_ => Guid.Parse(_.WorkId)).ToList();

            var queuedWork = new List<QueuedWorkDTO>();

            queuedWork = _simulationRepository.GetScenariosWithIds(simulationAnalysisIds)
                .Select(_ => _.ToQueuedWorkDTO(workQueue))
                .ToList();

            queuedWork = queuedWork.Concat(workQueue.Where(_ => _.WorkType == WorkType.DeleteNetwork).Select(_ => _.ToQueuedWorkDTO())).ToList();

            return queuedWork;
        }

        private List<QueuedWorkDTO> OrderByColumn(List<QueuedWorkDTO> queuedWork, string sortColumn, bool isDescending)
        {
            sortColumn = sortColumn?.ToLower();
            switch (sortColumn)
            {
            case "name":
                if (isDescending)
                    return queuedWork.OrderByDescending(_ => _.Name.ToLower()).ToList();
                else
                    return queuedWork.OrderBy(_ => _.Name.ToLower()).ToList();
            case "queueinguser":
                if (isDescending)
                    return queuedWork.OrderByDescending(_ => _.QueueingUser).ToList();
                else
                    return queuedWork.OrderBy(_ => _.QueueingUser).ToList();
            case "queueentrytimestamp":
                if (isDescending)
                    return queuedWork.OrderByDescending(_ => _.QueueEntryTimestamp).ToList();
                else
                    return queuedWork.OrderBy(_ => _.QueueEntryTimestamp).ToList();
            case "workstartedtimestamp":
                if (isDescending)
                    return queuedWork.OrderByDescending(_ => _.WorkStartedTimestamp).ToList();
                else
                    return queuedWork.OrderBy(_ => _.WorkStartedTimestamp).ToList();
            case "queueposition":
                if (isDescending)
                    return queuedWork.OrderByDescending(_ => _.QueuePosition).ToList();
                else
                    return queuedWork.OrderBy(_ => _.QueuePosition).ToList();
            }
            return queuedWork;
        }
    }


    public static class QueuedWorkTransform
    {
        public static QueuedWorkDTO ToQueuedWorkDTO(this SimulationDTO simulationDTO, IReadOnlyList<IQueuedWorkHandle> workQueue)
        {
            var queuedWorkHandle = workQueue.SingleOrDefault(_ => new Guid(_.WorkId) == simulationDTO.Id);

            if (queuedWorkHandle == null)
            {
                throw new RowNotInTableException("No queued simulation was found for the given id.");
            }

            QueuedWorkDTO queuedSimulationDTO = new QueuedWorkDTO()
            {
                Id = simulationDTO.Id,
                Name = queuedWorkHandle.WorkName,
                PreviousRunTime = simulationDTO.RunTime,

                Status = queuedWorkHandle. MostRecentStatusMessage,
                QueueEntryTimestamp = queuedWorkHandle.QueueEntryTimestamp,
                WorkStartedTimestamp = queuedWorkHandle.WorkStartTimestamp,
                QueueingUser = queuedWorkHandle.UserId,
                QueuePosition = queuedWorkHandle.QueueIndex,
                WorkDescription = queuedWorkHandle.WorkDescription
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

        public static QueuedWorkDTO ToQueuedWorkDTO(this IQueuedWorkHandle queuedWorkHandle)
        {
            QueuedWorkDTO queuedSimulationDTO = new QueuedWorkDTO()
            {
                Id = Guid.Parse(queuedWorkHandle.WorkId),
                Name = queuedWorkHandle.WorkName,
                Status = queuedWorkHandle.MostRecentStatusMessage,                
                QueueEntryTimestamp = queuedWorkHandle.QueueEntryTimestamp,
                WorkStartedTimestamp = queuedWorkHandle.WorkStartTimestamp,
                QueueingUser = queuedWorkHandle.UserId,
                QueuePosition = queuedWorkHandle.QueueIndex,
                WorkDescription = queuedWorkHandle.WorkDescription
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
