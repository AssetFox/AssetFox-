using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Services.Paging.Generics;
using MoreLinq;

namespace BridgeCareCore.Services
{
    public class SimulationPagingService : BasePagingService<SimulationDTO>,  ISimulationPagingService
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
            var rows = _simulationRepository.GetUserScenarios();

            return HandlePaging(rows, request);
        }

        public PagingPageModel<SimulationDTO> GetSharedScenarioPage(PagingRequestModel<SimulationDTO> request, bool hasAdminAccess, bool hasSimulationAccess)
        {
            var rows = _simulationRepository.GetSharedScenarios(hasAdminAccess, hasSimulationAccess);

            return HandlePaging(rows, request, false);
        }

        protected override List<SimulationDTO> SearchRows(List<SimulationDTO> simulations, string search)
        {
            return simulations
                .Where(_ =>
                _.Name!=null && _.Name.ToLower().Contains(search.Trim().ToLower()) ||
                _.NetworkName!=null && _.NetworkName.ToLower().Contains(search.Trim().ToLower()) ||
                (_.Status?.ToLower().Contains(search.Trim().ToLower()) ?? false) ||
                (_.ReportStatus?.ToLower().Contains(search.Trim().ToLower()) ?? false) ||
                (_.RunTime?.ToLower().Contains(search.Trim().ToLower()) ?? false) ||
                _.Creator!=null && _.Creator.ToLower().Contains(search.Trim().ToLower()) ||
                _.Owner!=null && _.Owner.ToLower().Contains(search.Trim().ToLower()) ||
                _.CreatedDate.ToString().Contains(search.Trim()) ||
                _.LastModifiedDate.ToString().Contains(search.Trim()) ||
                _.LastRun.ToString().Contains(search.Trim())).ToList();
        }

        protected override List<SimulationDTO> OrderByColumn(List<SimulationDTO> simulations, string sortColumn, bool isDescending)
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
