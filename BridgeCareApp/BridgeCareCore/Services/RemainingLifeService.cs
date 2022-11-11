using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using System.Linq;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;

namespace BridgeCareCore.Services
{
    public class RemainingLifeLimitService : IRemainingLifeLimitService
    {
        private static IUnitOfWork _unitOfWork;

        public RemainingLifeLimitService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public PagingPageModel<RemainingLifeLimitDTO> GetRemainingLifeLimitPage(Guid simulationId, PagingRequestModel<RemainingLifeLimitDTO> request)
        {
            var rows = request.PagingSync.LibraryId == null ? _unitOfWork.RemainingLifeLimitRepo.GetScenarioRemainingLifeLimits(simulationId) :
                _unitOfWork.RemainingLifeLimitRepo.GetRemainingLifeLimitsByLibraryId(request.PagingSync.LibraryId.Value);

            return HandlePaging(rows, request);
        }
        public PagingPageModel<RemainingLifeLimitDTO> GetLibraryRemainingLifeLimitPage(Guid libraryId, PagingRequestModel<RemainingLifeLimitDTO> request)
        {
            var rows = _unitOfWork.RemainingLifeLimitRepo.GetRemainingLifeLimitsByLibraryId(libraryId);

            return HandlePaging(rows, request);
        }
        public List<RemainingLifeLimitDTO> GetSyncedLibraryDataset(Guid libraryId, PagingSyncModel<RemainingLifeLimitDTO> request)
        {
            var rows = _unitOfWork.RemainingLifeLimitRepo.GetRemainingLifeLimitsByLibraryId(libraryId);
            return SyncedDataset(rows, request);
        }
        public List<RemainingLifeLimitDTO> GetSyncedScenarioDataset(Guid simulationId, PagingSyncModel<RemainingLifeLimitDTO> request)
        {
            var rows = request.LibraryId == null ?
                    _unitOfWork.RemainingLifeLimitRepo.GetScenarioRemainingLifeLimits(simulationId) :
                    _unitOfWork.RemainingLifeLimitRepo.GetRemainingLifeLimitsByLibraryId(request.LibraryId.Value);
            rows = SyncedDataset(rows, request);

            if (request.LibraryId != null)
                rows.ForEach(_ =>
                {
                    _.Id = Guid.NewGuid();
                    _.CriterionLibrary.Id = Guid.NewGuid();
                });
            return rows;
        }

        private PagingPageModel<RemainingLifeLimitDTO> HandlePaging(List<RemainingLifeLimitDTO> rows, PagingRequestModel<RemainingLifeLimitDTO> request)
        {
            var skip = 0;
            var take = 0;
            var items = new List<RemainingLifeLimitDTO>();

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
                return new PagingPageModel<RemainingLifeLimitDTO>()
                {
                    Items = items,
                    TotalItems = items.Count
                };
            }

            return new PagingPageModel<RemainingLifeLimitDTO>()
            {
                Items = items,
                TotalItems = rows.Count()
            };
        }

        private List<RemainingLifeLimitDTO> SyncedDataset(List<RemainingLifeLimitDTO> rows, PagingSyncModel<RemainingLifeLimitDTO> request)
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
