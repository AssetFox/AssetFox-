using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
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
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;

namespace BridgeCareCore.Services
{
    public class SimulationPagingService : ISimulationPagingService
    {
        private static IUnitOfWork _unitOfWork;
        private static ISimulationRepository _simulationRepository;

        public SimulationPagingService(IUnitOfWork unitOfWork, ISimulationRepository simulationRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _simulationRepository = simulationRepository ?? throw new ArgumentNullException(nameof(simulationRepository));
        }

        public PagingPageModel<SimulationDTO> GetUserScenarioPage(PagingRequestModel<SimulationDTO> request)
        {
            var skip = 0;
            var take = 0;
            var items = new List<SimulationDTO>();

            var simulations = _simulationRepository.GetUserScenarios();
              
            if (request.search.Trim() != "")
                simulations = SearchSimulations(simulations, request.search);
            if (request.sortColumn.Trim() != "")
                simulations = OrderByColumn(simulations, request.sortColumn, request.isDescending);

            if (request.RowsPerPage > 0)
            {
                take = request.RowsPerPage;
                skip = request.RowsPerPage * (request.Page - 1);
                items = simulations.Skip(skip).Take(take).ToList();
            }
            else
            {
                items = simulations.ToList();
                return new PagingPageModel<SimulationDTO>()
                {
                    Items = items,
                    TotalItems = items.Count
                };
            }

            return new PagingPageModel<SimulationDTO>()
            {
                Items = items,
                TotalItems = simulations.Count()
            };
        }

        public PagingPageModel<SimulationDTO> GetSharedScenarioPage(PagingRequestModel<SimulationDTO> request, bool hasAdminAccess, bool hasSimulationAccess)
        {
            var skip = 0;
            var take = 0;
            var items = new List<SimulationDTO>();

            var simulations = _simulationRepository.GetSharedScenarios(hasAdminAccess, hasSimulationAccess);

            if (request.search.Trim() != "")
                simulations = SearchSimulations(simulations, request.search);
            if (request.sortColumn.Trim() != "")
                simulations = OrderByColumn(simulations, request.sortColumn, request.isDescending);

            if (request.RowsPerPage > 0)
            {
                take = request.RowsPerPage;
                skip = request.RowsPerPage * (request.Page - 1);
                items = simulations.Skip(skip).Take(take).ToList();
            }
            else
            {
                items = simulations;
                return new PagingPageModel<SimulationDTO>()
                {
                    Items = items,
                    TotalItems = items.Count
                };
            }

            return new PagingPageModel<SimulationDTO>()
            {
                Items = items,
                TotalItems = simulations.Count()
            };
        }

        private List<SimulationDTO> SearchSimulations(List<SimulationDTO> simulations, string search)
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


        private List<SimulationDTO> OrderByColumn(List<SimulationDTO> simulations, string sortColumn, bool isDescending)
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
}
