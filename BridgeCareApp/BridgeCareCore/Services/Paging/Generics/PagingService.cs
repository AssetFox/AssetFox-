using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using BridgeCareCore.Models;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Org.BouncyCastle.Asn1.Ocsp;

namespace BridgeCareCore.Services.Paging.Generics
{
    public abstract class PagingService<T, Y> : BasePagingService<T>
        where T : BaseDTO
        where Y : BaseLibraryDTO 
    {
        public virtual PagingPageModel<T> GetScenarioPage(Guid scenarioId, PagingRequestModel<T> request)
        {
            var rows = request.SyncModel.LibraryId == null ? GetScenarioRows(scenarioId) :
                GetLibraryRows(request.SyncModel.LibraryId.Value);

            return HandlePaging(rows, request);
        }

        public virtual PagingPageModel<T> GetLibraryPage(Guid libraryId, PagingRequestModel<T> request)
        {
            var rows = GetLibraryRows(libraryId);

            return HandlePaging(rows, request);
        }

        public virtual List<T> GetSyncedScenarioDataSet(Guid scenarioId, PagingSyncModel<T> syncModel)
        {
            var rows = syncModel.LibraryId == null ?
                    GetScenarioRows(scenarioId) :
                    GetLibraryRows(syncModel.LibraryId.Value);
            rows = SyncDataset(rows, syncModel);
            if (syncModel.LibraryId != null)
                rows = CreateAsNewDataset(rows);
            return rows;
        }

        public virtual List<T> GetSyncedLibraryDataset(LibraryUpsertPagingRequestModel<Y, T> upsertRequest)
        {
            Guid? libraryId = null;

            var rows = new List<T>();
            if (upsertRequest.ScenarioId != null)
                rows = GetSyncedScenarioDataSet(upsertRequest.ScenarioId.Value, upsertRequest.PagingSync);
            else
            {
                if (upsertRequest.PagingSync.LibraryId != null)
                    libraryId = upsertRequest.PagingSync.LibraryId.Value;
                else if (!upsertRequest.IsNewLibrary)
                    libraryId = upsertRequest.Library.Id;

                if (libraryId != null)
                    rows = GetLibraryRows(libraryId.Value);
                rows = SyncDataset(rows, upsertRequest.PagingSync);
            }

            if (upsertRequest.IsNewLibrary)
            {
                rows = CreateAsNewDataset(rows);
            }

            return rows;
        }

        protected virtual List<T> GetScenarioRows(Guid scenarioId) => throw new NotImplementedException();
        protected virtual List<T> GetLibraryRows(Guid libraryId) => throw new NotImplementedException();
        protected virtual List<T> CreateAsNewDataset(List<T> rows) => throw new NotImplementedException();
    }
}
