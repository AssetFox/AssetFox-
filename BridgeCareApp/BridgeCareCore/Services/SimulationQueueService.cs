using AppliedResearchAssociates.iAM.DTOs;
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


namespace BridgeCareCore.Services
{
    public class SimulationQueueService : ISimulationQueueService
    {
        private static UnitOfDataPersistenceWork _unitOfWork;
        private ISimulationAnalysis _simulationAnalysis;
        private ISimulationService _simulationService;
        private SequentialWorkQueue _sequentialWorkQueue;

        public SimulationQueueService(UnitOfDataPersistenceWork unitOfWork, SequentialWorkQueue sequentialWorkQueue, ISimulationService simulationService, ISimulationAnalysis simulationAnalysis)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _sequentialWorkQueue = sequentialWorkQueue ?? throw new ArgumentNullException(nameof(sequentialWorkQueue));

            _simulationAnalysis = simulationAnalysis ?? throw new ArgumentNullException(nameof(simulationAnalysis));
            _simulationService = simulationService ?? throw new ArgumentNullException(nameof(simulationService));
        }

        public PagingPageModel<QueuedSimulationDTO> GetSimulationQueuePage(PagingRequestModel<QueuedSimulationDTO> request, string role)
        {
            var skip = 0;
            var take = 0;
            var items = new List<QueuedSimulationDTO>();
            var users = _unitOfWork.Context.User.ToList();




            // TODO: DELETE BELOW

            items.Add(new QueuedSimulationDTO
            {
                Id = Guid.NewGuid(), QueueEntryTimestamp = DateTime.Now,
            });

            items.Add(new QueuedSimulationDTO
            {
                Id = Guid.NewGuid(),
                QueueEntryTimestamp = DateTime.Now,
            });

            items.Add(new QueuedSimulationDTO
            {
                Id = Guid.NewGuid(),
                QueueEntryTimestamp = DateTime.Now,
            });

            items.Add(new QueuedSimulationDTO
            {
                Id = Guid.NewGuid(),
                QueueEntryTimestamp = DateTime.Now,
            });


            return new PagingPageModel<QueuedSimulationDTO>()
            {
                Items = items,
                TotalItems = items.Count
            };

            // TODO: DELETE ABOVE



            var simulationQueue = _sequentialWorkQueue.Snapshot;

            var simulationQueueIds = simulationQueue.Select(_ => new Guid(_.WorkId)).ToList();

            var simulations = _unitOfWork.Context.Simulation
                .Include(_ => _.SimulationAnalysisDetail)
                .Include(_ => _.SimulationReportDetail)
                .Include(_ => _.SimulationUserJoins)
                .ThenInclude(_ => _.User)
                .Include(_ => _.Network)
            .ToList()
            .Where(_ => simulationQueueIds.Contains(_.Id))
            .Select(_ => _.ToDto(users.FirstOrDefault(__ => __.Id == _.CreatedBy)).ToQueuedSimulationDTO())
            .ToList();

            if (request.search.Trim() != "")
                simulations = SearchSimulations(simulations, request.search); 
            if (request.sortColumn.Trim() != "")
                simulations = OrderByColumn(simulations, request.sortColumn, request.isDescending);

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

            foreach (var simulation in items)
            {
                var queuedWorkHandle = simulationQueue.SingleOrDefault(_ => new Guid(_.WorkId) == simulation.Id);
                simulation.QueueEntryTimestamp = queuedWorkHandle.QueueEntryTimestamp;
                simulation.WorkStartedTimestamp = queuedWorkHandle.WorkStartTimestamp;
                simulation.QueueingUser = queuedWorkHandle.UserInfo.Name;
            }

            return new PagingPageModel<QueuedSimulationDTO>()
            {
                Items = items,
                TotalItems = totalItemCount
            };
        }

        private List<QueuedSimulationDTO> SearchSimulations(List<QueuedSimulationDTO> simulations, string search)
        {
            return simulations
                .Where(_ =>
                _.Name.ToLower().Contains(search.Trim().ToLower()) ||
                _.NetworkName.ToLower().Contains(search.Trim().ToLower()) ||
                (_.Status?.ToLower().Contains(search.Trim().ToLower()) ?? false) ||
                (_.ReportStatus?.ToLower().Contains(search.Trim().ToLower()) ?? false) ||
                (_.RunTime?.ToLower().Contains(search.Trim().ToLower()) ?? false) ||
                _.Creator.ToLower().Contains(search.Trim().ToLower()) ||
                _.Owner.ToLower().Contains(search.Trim().ToLower()) ||
                _.CreatedDate.ToString().Contains(search.Trim()) ||
                _.LastModifiedDate.ToString().Contains(search.Trim())).ToList();
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
            case "lastrun":
                if (isDescending)
                    return simulations.OrderByDescending(_ => _.LastRun).ToList();
                else
                    return simulations.OrderBy(_ => _.LastRun).ToList();
            case "lastmodifieddate":
                if (isDescending)
                    return simulations.OrderByDescending(_ => _.LastModifiedDate).ToList();
                else
                    return simulations.OrderBy(_ => _.LastModifiedDate).ToList();
            case "createddate":
                if (isDescending)
                    return simulations.OrderByDescending(_ => _.CreatedDate).ToList();
                else
                    return simulations.OrderBy(_ => _.CreatedDate).ToList();
            }
            return simulations;
        }
    }


    public static class QueuedSimulationMapper
    {
        public static QueuedSimulationDTO ToQueuedSimulationDTO(this SimulationDTO simulationDTO) =>
            new QueuedSimulationDTO
            {
                Id = simulationDTO.Id,
                Name = simulationDTO.Name,
                NetworkId = simulationDTO.NetworkId,
                NetworkName = simulationDTO.NetworkName,
                Creator = simulationDTO.Creator,
                Owner = simulationDTO.Owner,
                CreatedDate = simulationDTO.CreatedDate,
                LastModifiedDate = simulationDTO.LastModifiedDate,
                LastRun = simulationDTO.LastRun,
                RunTime = simulationDTO.RunTime,
                Status = simulationDTO.Status,
                ReportStatus = simulationDTO.ReportStatus,
                Users = simulationDTO.Users,
            };
    }

}
