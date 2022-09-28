using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using System;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using AppliedResearchAssociates.iAM.DTOs;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;

namespace BridgeCareCore.Services
{
    public class BudgetPriortyService : IBudgetPriortyService
    {
        private static UnitOfDataPersistenceWork _unitOfWork;

        public BudgetPriortyService(UnitOfDataPersistenceWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public PagingPageModel<BudgetPriorityDTO> GetBudgetPriortyPage(Guid simulationId, PagingRequestModel<BudgetPriorityDTO> request)
        {
            var rows = _unitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(simulationId);

            return HandlePaging(rows, request);
        }
        public PagingPageModel<BudgetPriorityDTO> GetLibraryBudgetPriortyPage(Guid libraryId, PagingRequestModel<BudgetPriorityDTO> request)
        {
            var rows = _unitOfWork.BudgetPriorityRepo.GetBudgetPrioritiesByLibraryId(libraryId);

            return HandlePaging(rows, request);
        }
        public List<BudgetPriorityDTO> GetSyncedLibraryDataset(Guid libraryId, PagingSyncModel<BudgetPriorityDTO> request)
        {
            var rows = _unitOfWork.BudgetPriorityRepo.GetBudgetPrioritiesByLibraryId(libraryId);
            return SyncedDataset(rows, request);
        }
        public List<BudgetPriorityDTO> GetSyncedScenarioDataset(Guid simulationId, PagingSyncModel<BudgetPriorityDTO> request)
        {
            var rows = request.LibraryId == null ?
                    _unitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(simulationId) :
                    _unitOfWork.BudgetPriorityRepo.GetBudgetPrioritiesByLibraryId(request.LibraryId.Value);
            return SyncedDataset(rows, request);
        }

        private PagingPageModel<BudgetPriorityDTO> HandlePaging(List<BudgetPriorityDTO> rows, PagingRequestModel<BudgetPriorityDTO> request)
        {
            var skip = 0;
            var take = 0;
            var items = new List<BudgetPriorityDTO>();

            if (request.search.Trim() != "")
                rows = SearchRows(rows, request.search);
            if (request.sortColumn.Trim() != "")
                rows = OrderByColumn(rows, request.sortColumn, request.isDescending);

            rows = SyncedDataset(rows, request.PagingSync);

            if (request.RowsPerPage > 0)
            {
                take = request.RowsPerPage;
                skip = request.RowsPerPage * (request.Page - 1);
                items = rows.Skip(skip).Take(take).ToList();
            }
            else
            {
                items = rows;
                return new PagingPageModel<BudgetPriorityDTO>()
                {
                    Items = items,
                    TotalItems = items.Count
                };
            }

            return new PagingPageModel<BudgetPriorityDTO>()
            {
                Items = items,
                TotalItems = rows.Count()
            };
        }

        private List<BudgetPriorityDTO> OrderByColumn(List<BudgetPriorityDTO> rows, string sortColumn, bool isDescending)
        {
            sortColumn = sortColumn?.ToLower();
            switch (sortColumn)
            {
            case "priorityLevel":
                if (isDescending)
                    return rows.OrderByDescending(_ => _.PriorityLevel).ToList();
                else
                    return rows.OrderBy(_ => _.PriorityLevel).ToList();
            }
            return rows;
        }

        private List<BudgetPriorityDTO> SearchRows(List<BudgetPriorityDTO> rows, string search)
        {
            return rows
                .Where(_ => _.PriorityLevel.ToString().Contains(search) ||
                    _.Year.ToString().Contains(search) ||
                    (_.CriterionLibrary.MergedCriteriaExpression != null && _.CriterionLibrary.MergedCriteriaExpression.ToLower().Contains(search))).ToList();
        }

        private List<BudgetPriorityDTO> SyncedDataset(List<BudgetPriorityDTO> rows, PagingSyncModel<BudgetPriorityDTO> request)
        {
            rows = rows.Concat(request.AddedRows).Where(_ => !request.RowsForDeletion.Contains(_.Id)).ToList();

            for (var i = 0; i < rows.Count; i++)
            {
                var item = request.UpdateRows.FirstOrDefault(row => row.Id == rows[i].Id);
                if (item != null)
                    rows[i] = item;
            }

            return rows;
        }
    }
}
