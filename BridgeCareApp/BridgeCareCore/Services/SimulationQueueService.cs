﻿using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models.Validation;
using OfficeOpenXml;
using MoreLinq;
using Microsoft.EntityFrameworkCore;
using System.IO;
using BridgeCareCore.Models;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using System.Data;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;

namespace BridgeCareCore.Services
{
    public class SimulationQueueService : ISimulationQueueService
    {
        private static IUnitOfWork _unitOfWork;
        private static ISimulationRepository _simulationRepository;
        private ISimulationAnalysis _simulationAnalysis;
        private ISimulationService _simulationService;
        private SequentialWorkQueue _sequentialWorkQueue;

        public SimulationQueueService(IUnitOfWork unitOfWork, SequentialWorkQueue sequentialWorkQueue, ISimulationService simulationService, ISimulationAnalysis simulationAnalysis, ISimulationRepository simulationRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _sequentialWorkQueue = sequentialWorkQueue ?? throw new ArgumentNullException(nameof(sequentialWorkQueue));

            _simulationAnalysis = simulationAnalysis ?? throw new ArgumentNullException(nameof(simulationAnalysis));
            _simulationService = simulationService ?? throw new ArgumentNullException(nameof(simulationService));
            _simulationRepository = simulationRepository ?? throw new ArgumentNullException(nameof(simulationRepository));
        }

        public PagingPageModel<QueuedSimulationDTO> GetSimulationQueuePage(PagingRequestModel<QueuedSimulationDTO> request, bool hasAdminAccess, bool hasSimulationAccess)
        {
            var skip = 0;
            var take = 0;
            var items = new List<QueuedSimulationDTO>();
            var simulationQueue = _sequentialWorkQueue.Snapshot;

            var simulationQueueIds = simulationQueue.Select(_ => new Guid(_.WorkId)).ToList();

            var simulations = _simulationRepository.GetScenariosWithIds(simulationQueueIds)
                .Select(_ => _.ToQueuedSimulationDTO())
                .ToList();

            foreach (var simulation in simulations)
            {
                var queuedWorkHandle = simulationQueue.SingleOrDefault(_ => new Guid(_.WorkId) == simulation.Id);
                simulation.QueueEntryTimestamp = queuedWorkHandle.QueueEntryTimestamp;
                simulation.WorkStartedTimestamp = queuedWorkHandle.WorkStartTimestamp;
                simulation.QueueingUser = queuedWorkHandle.UserInfo.Name;
                simulation.QueuePosition = queuedWorkHandle.QueueIndex;
                if (queuedWorkHandle.WorkHasStarted)
                {
                    simulation.CurrentRunTime = DateTime.Now.Subtract(simulation.WorkStartedTimestamp.Value).ToString(@"hh\:mm\:ss", null);
                }
                else
                {
                    simulation.WorkStartedTimestamp = null;
                    simulation.CurrentRunTime = null;
                    simulation.Status = $"Queued to run.";
                }
            }

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
        public static QueuedSimulationDTO ToQueuedSimulationDTO(this SimulationDTO simulationDTO) =>
            new QueuedSimulationDTO
            {
                Id = simulationDTO.Id,
                Name = simulationDTO.Name,
                PreviousRunTime = simulationDTO.RunTime,
                Status = simulationDTO.Status,
            };
    }

}
