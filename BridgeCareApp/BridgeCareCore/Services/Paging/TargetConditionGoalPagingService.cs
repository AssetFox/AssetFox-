using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using System.Linq;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;

namespace BridgeCareCore.Services
{
    public class TargetConditionGoalPagingService : ITargetConditionGoalPagingService
    {
        private static IUnitOfWork _unitOfWork;

        public TargetConditionGoalPagingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public PagingPageModel<TargetConditionGoalDTO> GetTargetConditionGoalPage(Guid simulationId, PagingRequestModel<TargetConditionGoalDTO> request)
        {
            var rows = request.PagingSync.LibraryId == null ? _unitOfWork.TargetConditionGoalRepo.GetScenarioTargetConditionGoals(simulationId) :
                _unitOfWork.TargetConditionGoalRepo.GetTargetConditionGoalsByLibraryId(request.PagingSync.LibraryId.Value);

            return HandlePaging(rows, request);
        }
        public PagingPageModel<TargetConditionGoalDTO> GetLibraryTargetConditionGoalPage(Guid libraryId, PagingRequestModel<TargetConditionGoalDTO> request)
        {
            var rows = _unitOfWork.TargetConditionGoalRepo.GetTargetConditionGoalsByLibraryId(libraryId);

            return HandlePaging(rows, request);
        }
        public List<TargetConditionGoalDTO> GetSyncedLibraryDataset(Guid libraryId, PagingSyncModel<TargetConditionGoalDTO> request)
        {
            var rows = _unitOfWork.TargetConditionGoalRepo.GetTargetConditionGoalsByLibraryId(libraryId);
            return SyncedDataset(rows, request);
        }
        public List<TargetConditionGoalDTO> GetSyncedScenarioDataset(Guid simulationId, PagingSyncModel<TargetConditionGoalDTO> request)
        {
            var rows = request.LibraryId == null ?
                    _unitOfWork.TargetConditionGoalRepo.GetScenarioTargetConditionGoals(simulationId) :
                    _unitOfWork.TargetConditionGoalRepo.GetTargetConditionGoalsByLibraryId(request.LibraryId.Value);
            rows = SyncedDataset(rows, request);

            if (request.LibraryId != null)
                rows.ForEach(_ =>
                {
                    _.Id = Guid.NewGuid();
                    _.CriterionLibrary.Id = Guid.NewGuid();
                });
            return rows;
        }

        private PagingPageModel<TargetConditionGoalDTO> HandlePaging(List<TargetConditionGoalDTO> rows, PagingRequestModel<TargetConditionGoalDTO> request)
        {
            var skip = 0;
            var take = 0;
            var items = new List<TargetConditionGoalDTO>();

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
                return new PagingPageModel<TargetConditionGoalDTO>()
                {
                    Items = items,
                    TotalItems = items.Count
                };
            }

            return new PagingPageModel<TargetConditionGoalDTO>()
            {
                Items = items,
                TotalItems = rows.Count()
            };
        }

        private List<TargetConditionGoalDTO> SyncedDataset(List<TargetConditionGoalDTO> rows, PagingSyncModel<TargetConditionGoalDTO> request)
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
