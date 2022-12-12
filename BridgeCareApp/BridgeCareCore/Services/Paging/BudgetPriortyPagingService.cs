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
    public class BudgetPriortyPagingService : IBudgetPriortyPagingService
    {
        private static IUnitOfWork _unitOfWork;

        public BudgetPriortyPagingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public PagingPageModel<BudgetPriorityDTO> GetBudgetPriortyPage(Guid simulationId, PagingRequestModel<BudgetPriorityDTO> request)
        {
            var rows = request.PagingSync.LibraryId == null ? _unitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(simulationId) :
                _unitOfWork.BudgetPriorityRepo.GetBudgetPrioritiesByLibraryId(request.PagingSync.LibraryId.Value);

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
            rows = SyncedDataset(rows, request);

            if(request.LibraryId != null)
                rows.ForEach(_ =>
                {
                    _.Id = Guid.NewGuid();
                    _.CriterionLibrary.Id = Guid.NewGuid();
                });
            var budgets = _unitOfWork.BudgetRepo.GetScenarioSimpleBudgetDetails(simulationId);
            //this gets rid of percentage pairs that shouldn't be there and adds the ones that should
            rows.ForEach(row =>
            {
                row.BudgetPercentagePairs = row.BudgetPercentagePairs.Where(_ => budgets.Any(__ => __.Name == _.BudgetName)).ToList();
                budgets.ForEach(_ =>
                {
                    if (!row.BudgetPercentagePairs.Any(__ => __.BudgetName == _.Name))
                        row.BudgetPercentagePairs.Add(new BudgetPercentagePairDTO()
                        {
                            Id = Guid.NewGuid(),
                            BudgetId = _.Id,
                            BudgetName = _.Name,
                            Percentage = 100
                        });
                });
            });
            return rows;
        }

        public List<BudgetPriorityDTO> GetNewLibraryDataset(PagingSyncModel<BudgetPriorityDTO> pagingSync)
        {
            var rows = new List<BudgetPriorityDTO>();
            return SyncedDataset(rows, pagingSync);
        }

        private PagingPageModel<BudgetPriorityDTO> HandlePaging(List<BudgetPriorityDTO> rows, PagingRequestModel<BudgetPriorityDTO> request)
        {
            var skip = 0;
            var take = 0;
            var items = new List<BudgetPriorityDTO>();

            rows = SyncedDataset(rows, request.PagingSync);

            if (request.search.Trim() != "")
                rows = SearchRows(rows, request.search);
            if (request.sortColumn.Trim() != "")
                rows = OrderByColumn(rows, request.sortColumn, request.isDescending);

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
            case "prioritylevel":
                if (isDescending)
                    return rows.OrderByDescending(_ => _.PriorityLevel).ToList();
                else
                    return rows.OrderBy(_ => _.PriorityLevel).ToList();
            default://This is sorting the budget priorities by a given budget name
                if (isDescending)
                {
                    return rows.OrderByDescending(_ =>
                    {
                        var pair = _.BudgetPercentagePairs.FirstOrDefault(__ => __.BudgetName.ToLower() == sortColumn);
                        return pair != null ? pair.Percentage : 0;
                    }).ToList();
                }
                else
                {
                    return rows.OrderBy(_ =>
                    {
                        var pair = _.BudgetPercentagePairs.FirstOrDefault(__ => __.BudgetName.ToLower() == sortColumn);
                        return pair != null ? pair.Percentage : 0;
                    }).ToList();
                }
            }
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
