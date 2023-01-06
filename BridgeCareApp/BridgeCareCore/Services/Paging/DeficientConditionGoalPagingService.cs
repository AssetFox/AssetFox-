using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using System.Linq;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;

namespace BridgeCareCore.Services
{
    public class DeficientConditionGoalPagingService : IDeficientConditionGoalPagingService
    {
        private static IUnitOfWork _unitOfWork;

        public DeficientConditionGoalPagingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public PagingPageModel<DeficientConditionGoalDTO> GetScenarioDeficientConditionGoalPage(Guid simulationId, PagingRequestModel<DeficientConditionGoalDTO> request)
        {
            var rows = request.SyncModel.LibraryId == null ? _unitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationId) :
                _unitOfWork.DeficientConditionGoalRepo.GetDeficientConditionGoalsByLibraryId(request.SyncModel.LibraryId.Value);

            return HandlePaging(rows, request);
        }
        public PagingPageModel<DeficientConditionGoalDTO> GetLibraryDeficientConditionGoalPage(Guid libraryId, PagingRequestModel<DeficientConditionGoalDTO> request)
        {
            var rows = _unitOfWork.DeficientConditionGoalRepo.GetDeficientConditionGoalsByLibraryId(libraryId);

            return HandlePaging(rows, request);
        }
        public List<DeficientConditionGoalDTO> GetSyncedLibraryDataset(Guid libraryId, PagingSyncModel<DeficientConditionGoalDTO> request)
        {
            var rows = _unitOfWork.DeficientConditionGoalRepo.GetDeficientConditionGoalsByLibraryId(libraryId);
            return SyncedDataset(rows, request);
        }
        public List<DeficientConditionGoalDTO> GetSyncedScenarioDataset(Guid simulationId, PagingSyncModel<DeficientConditionGoalDTO> request)
        {
            var rows = request.LibraryId == null ?
                    _unitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationId) :
                    _unitOfWork.DeficientConditionGoalRepo.GetDeficientConditionGoalsByLibraryId(request.LibraryId.Value);
            rows = SyncedDataset(rows, request);

            if (request.LibraryId != null)
                rows.ForEach(_ =>
                {
                    _.Id = Guid.NewGuid();
                    _.CriterionLibrary.Id = Guid.NewGuid();
                });
            return rows;
        }

        private PagingPageModel<DeficientConditionGoalDTO> HandlePaging(List<DeficientConditionGoalDTO> rows, PagingRequestModel<DeficientConditionGoalDTO> request)
        {
            var skip = 0;
            var take = 0;
            var items = new List<DeficientConditionGoalDTO>();

            rows = SyncedDataset(rows, request.SyncModel);

            if (request.RowsPerPage > 0)
            {
                take = request.RowsPerPage;
                skip = request.RowsPerPage * (request.Page - 1);
                items = rows.Skip(skip).Take(take).ToList();
            }
            else
            {
                items = rows;
                return new PagingPageModel<DeficientConditionGoalDTO>()
                {
                    Items = items,
                    TotalItems = items.Count
                };
            }

            return new PagingPageModel<DeficientConditionGoalDTO>()
            {
                Items = items,
                TotalItems = rows.Count()
            };
        }

        private List<DeficientConditionGoalDTO> SyncedDataset(List<DeficientConditionGoalDTO> rows, PagingSyncModel<DeficientConditionGoalDTO> request)
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
