using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using System.Collections.Generic;
using System;
using BridgeCareCore.Services.Treatment;
using System.Linq;
using BridgeCareCore.Interfaces;

namespace BridgeCareCore.Services
{
    public class TreatmentPagingService : ITreatmentPagingService
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public TreatmentPagingService(UnitOfDataPersistenceWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public TreatmentLibraryDTO GetSyncedLibraryDataset(LibraryUpsertPagingRequestModel<TreatmentLibraryDTO, TreatmentDTO> upsertRequest)
        {
            Guid? libraryId = null;

            var rows = new List<TreatmentDTO>();
            if (upsertRequest.PagingSync.LibraryId != null)
                libraryId = upsertRequest.PagingSync.LibraryId.Value;
            else if (!upsertRequest.IsNewLibrary)
                libraryId = upsertRequest.Library.Id;

            if (libraryId != null)
                rows = _unitOfWork.SelectableTreatmentRepo.GetSelectableTreatments(libraryId.Value);
            rows = SyncedDataset(rows, upsertRequest.PagingSync);

            if (upsertRequest.PagingSync.LibraryId != null && upsertRequest.PagingSync.LibraryId != upsertRequest.Library.Id)
            {
                rows.ForEach(_ =>
                {
                    _.Id = Guid.NewGuid();
                    _.CriterionLibrary.Id = Guid.NewGuid();
                    _.Consequences.ForEach(__ =>
                    {
                        __.Id = Guid.NewGuid();
                        __.CriterionLibrary.Id = Guid.NewGuid();
                        __.Equation.Id = Guid.NewGuid();
                    });
                    _.Costs.ForEach(__ =>
                    {
                        __.Id = Guid.NewGuid();
                        __.Equation.Id = Guid.NewGuid();
                        __.CriterionLibrary.Id = Guid.NewGuid();
                    });
                });
            }
            var dto = upsertRequest.Library;
            dto.Treatments = rows;

            return dto;
        }
        public List<TreatmentDTO> GetSyncedScenarioDataset(Guid simulationId, PagingSyncModel<TreatmentDTO> request)
        {
            var rows = request.LibraryId == null ?
                    _unitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulationId) :
                    _unitOfWork.SelectableTreatmentRepo.GetSelectableTreatments(request.LibraryId.Value);
            rows = SyncedDataset(rows, request);

            if (request.LibraryId != null)
                rows.ForEach(_ =>
                {
                    _.Id = Guid.NewGuid();
                    _.CriterionLibrary.Id = Guid.NewGuid();
                    _.Consequences.ForEach(__ =>
                    {
                        __.Id = Guid.NewGuid();
                        __.CriterionLibrary.Id = Guid.NewGuid();
                        __.Equation.Id = Guid.NewGuid();
                    });
                    _.Costs.ForEach(__ =>
                    {
                        __.Id = Guid.NewGuid();
                        __.Equation.Id = Guid.NewGuid();
                        __.CriterionLibrary.Id = Guid.NewGuid();
                    });
                });
            return rows;
        }

        private List<TreatmentDTO> SyncedDataset(List<TreatmentDTO> rows, PagingSyncModel<TreatmentDTO> request)
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
