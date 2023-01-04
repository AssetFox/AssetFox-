using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using System.Linq;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;

namespace BridgeCareCore.Services
{
    public class CashFlowService : ICashFlowService
    {
        private static IUnitOfWork _unitOfWork;

        public CashFlowService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public PagingPageModel<CashFlowRuleDTO> GetCashFlowPage(Guid simulationId, PagingRequestModel<CashFlowRuleDTO> request)
        {
            var rows = request.PagingSync.LibraryId == null ? _unitOfWork.CashFlowRuleRepo.GetScenarioCashFlowRules(simulationId) :
                _unitOfWork.CashFlowRuleRepo.GetCashFlowRulesByLibraryId(request.PagingSync.LibraryId.Value);

            return HandlePaging(rows, request);
        }
        public PagingPageModel<CashFlowRuleDTO> GetLibraryCashFlowPage(Guid libraryId, PagingRequestModel<CashFlowRuleDTO> request)
        {
            var rows = _unitOfWork.CashFlowRuleRepo.GetCashFlowRulesByLibraryId(libraryId);

            return HandlePaging(rows, request);
        }
        public List<CashFlowRuleDTO> GetSyncedLibraryDataset(Guid libraryId, PagingSyncModel<CashFlowRuleDTO> request)
        {
            var rows = _unitOfWork.CashFlowRuleRepo.GetCashFlowRulesByLibraryId(libraryId);
            return SyncedDataset(rows, request);
        }
        public List<CashFlowRuleDTO> GetSyncedScenarioDataset(Guid simulationId, PagingSyncModel<CashFlowRuleDTO> request)
        {
            var rows = request.LibraryId == null ?
                    _unitOfWork.CashFlowRuleRepo.GetScenarioCashFlowRules(simulationId) :
                    _unitOfWork.CashFlowRuleRepo.GetCashFlowRulesByLibraryId(request.LibraryId.Value);
            rows = SyncedDataset(rows, request);

            if (request.LibraryId != null)
                rows.ForEach(_ =>
                {
                    _.Id = Guid.NewGuid();
                    _.CriterionLibrary.Id = Guid.NewGuid();
                    _.CashFlowDistributionRules.ForEach(__ => __.Id = Guid.NewGuid());
                });
            return rows;
        }

        public List<CashFlowRuleDTO> GetNewLibraryDataset(PagingSyncModel<CashFlowRuleDTO> pagingSync)
        {
            var rows = new List<CashFlowRuleDTO>();
            return SyncedDataset(rows, pagingSync);
        }

        private PagingPageModel<CashFlowRuleDTO> HandlePaging(List<CashFlowRuleDTO> rows, PagingRequestModel<CashFlowRuleDTO> request)
        {
            var skip = 0;
            var take = 0;
            var items = new List<CashFlowRuleDTO>();

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
                return new PagingPageModel<CashFlowRuleDTO>()
                {
                    Items = items,
                    TotalItems = items.Count
                };
            }

            return new PagingPageModel<CashFlowRuleDTO>()
            {
                Items = items,
                TotalItems = rows.Count()
            };
        }

        private List<CashFlowRuleDTO> SyncedDataset(List<CashFlowRuleDTO> rows, PagingSyncModel<CashFlowRuleDTO> request)
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
