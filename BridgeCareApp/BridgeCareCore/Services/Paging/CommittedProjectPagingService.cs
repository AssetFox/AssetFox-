using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Services.Paging.Generics;
using BridgeCareCore.Utils;

namespace BridgeCareCore.Services
{
    public class CommittedProjectPagingService :BasePagingService<SectionCommittedProjectDTO>,  ICommittedProjectPagingService
    {
        private static IUnitOfWork _unitOfWork;
        public const string UnknownBudgetName = "Unknown";

        // TODO: Determine based on associated network
        private readonly string _networkKeyField = "BRKEY_";

        public CommittedProjectPagingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public PagingPageModel<SectionCommittedProjectDTO> GetCommittedProjectPage(List<SectionCommittedProjectDTO> committedProjects, PagingRequestModel<SectionCommittedProjectDTO> request)
        {
            var skip = 0;
            var take = 0;
            var items = new List<SectionCommittedProjectDTO>();
            var budgetdict = new Dictionary<Guid, string>();
            var totalItems = 0;

            request.search = request.search == null ? "" : request.search;
            request.sortColumn = request.sortColumn == null ? "" : request.sortColumn;



            committedProjects = SyncDataset(committedProjects, request.SyncModel);
            committedProjects = committedProjects.OrderBy(_ => _.Id).ToList();
            totalItems = committedProjects.Count;

            if (request.search.Trim() != "" || request.sortColumn.Trim() != "")
            {
                var budgetIds = committedProjects.Select(_ => _.ScenarioBudgetId).Distinct().Where(x => x!=null).Select(x => x.Value).ToList();
                budgetdict = _unitOfWork.BudgetRepo.GetScenarioBudgetDictionary(budgetIds);
            }

            if (request.search.Trim() != "")
                committedProjects = SearchRows(committedProjects, request.search, budgetdict);
            if (request.sortColumn.Trim() != "")
                committedProjects = OrderByColumn(committedProjects, request.sortColumn, request.isDescending, budgetdict);

            if (request.RowsPerPage > 0)
            {
                take = request.RowsPerPage;
                skip = request.RowsPerPage * (request.Page - 1);
                items = committedProjects.Skip(skip).Take(take).ToList();
            }
            else
            {
                items = committedProjects;
                return new PagingPageModel<SectionCommittedProjectDTO>()
                {
                    Items = items,
                    TotalItems = items.Count
                };
            }

            return new PagingPageModel<SectionCommittedProjectDTO>()
            {
                Items = items,
                TotalItems = totalItems
            };
        }

        public List<SectionCommittedProjectDTO> GetSyncedDataset(Guid simulationId, PagingSyncModel<SectionCommittedProjectDTO> request)
        {
            var committedProjects = _unitOfWork.CommittedProjectRepo.GetSectionCommittedProjectDTOs(simulationId);
            return SyncDataset(committedProjects, request);
        }


        private List<SectionCommittedProjectDTO> OrderByColumn(List<SectionCommittedProjectDTO> committedProjects,
            string sortColumn,
            bool isDescending,
            Dictionary<Guid, string> budgetDict)
        {
            sortColumn = sortColumn?.ToLower();
            switch (sortColumn)
            {
            case "brkey":
                if (isDescending)
                    return committedProjects.OrderByDescending(_ => _.LocationKeys[_networkKeyField], new AlphanumericComparator()).ToList();
                else
                    return committedProjects.OrderBy(_ => _.LocationKeys[_networkKeyField], new AlphanumericComparator()).ToList();
            case "year":
                if (isDescending)
                    return committedProjects.OrderByDescending(_ => _.Year).ToList();
                else
                    return committedProjects.OrderBy(_ => _.Year).ToList();
            case "treatment":
                if (isDescending)
                    return committedProjects.OrderByDescending(_ => (_.Treatment ?? "").ToLower()).ToList();
                else
                    return committedProjects.OrderBy(_ => (_.Treatment ?? "").ToLower()).ToList();
            case "category":
                if (isDescending)
                    return committedProjects.OrderByDescending(_ => _.Category.ToString().ToLower()).ToList();
                else
                    return committedProjects.OrderBy(_ => _.Category.ToString().ToLower()).ToList();
            case "budget":
                if (isDescending)
                    return committedProjects.OrderByDescending(_ => _.ScenarioBudgetId == null ? "" : budgetDict[_.ScenarioBudgetId.Value].ToLower()).ToList();
                else
                    return committedProjects.OrderBy(_ => _.Category.ToString().ToLower()).ToList();
            case "cost":
                if (isDescending)
                    return committedProjects.OrderByDescending(_ => _.Cost).ToList();
                else
                    return committedProjects.OrderBy(_ => _.Cost).ToList();
            }
            return committedProjects;
        }

        private List<SectionCommittedProjectDTO> SearchRows(List<SectionCommittedProjectDTO> rows,
            string search,
            Dictionary<Guid, string> budgetDict)
        {
            search = search.ToLower();
            return rows
                .Where(_ => _.LocationKeys !=null && _.LocationKeys.ContainsKey(_networkKeyField) && _.LocationKeys[_networkKeyField].ToLower().Contains(search) ||
                    _.Year.ToString().Contains(search) ||
                    _.Treatment!=null && _.Treatment.ToLower().Contains(search) ||
                    _.Category.ToString().ToLower().Contains(search) ||
                    (_.ScenarioBudgetId == null ? "" : budgetDict[_.ScenarioBudgetId.Value]).Contains(search) ||
                    _.Cost.ToString().Contains(search)).ToList();
        }
    }
}
