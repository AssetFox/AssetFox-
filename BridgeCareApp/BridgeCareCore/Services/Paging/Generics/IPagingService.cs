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
    public interface IPagingService<T, Y>
        where T : BaseDTO
        where Y : BaseLibraryDTO
    {
        PagingPageModel<T> GetScenarioPage(Guid scenarioId, PagingRequestModel<T> request)
        {
            var rows = request.SyncModel.LibraryId == null ? GetScenarioRows(scenarioId) :
                GetLibraryRows(request.SyncModel.LibraryId.Value);

            return HandlePaging(rows, request);
        }

        PagingPageModel<T> GetLibraryPage(Guid libraryId, PagingRequestModel<T> request)
        {
            var rows = GetLibraryRows(libraryId);

            return HandlePaging(rows, request);
        }

        List<T> GetSyncedScenarioDataSet(Guid scenarioId, PagingSyncModel<T> syncModel)
        {
            var rows = syncModel.LibraryId == null ?
                    GetScenarioRows(scenarioId) :
                    GetLibraryRows(syncModel.LibraryId.Value);
            rows = SyncDataset(rows, syncModel);
            if (syncModel.LibraryId != null)
                rows = CreateAsNewDataset(rows);
            return rows;
        }

        List<T> GetSyncedLibraryDataset(LibraryUpsertPagingRequestModel<Y, T> upsertRequest)
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

        protected PagingPageModel<T> HandlePaging(List<T> rows, PagingRequestModel<T> request)
        {
            var skip = 0;
            var take = 0;
            var items = new List<T>();

            rows = SyncDataset(rows, request.SyncModel);

            if (request.RowsPerPage > 0)
            {
                take = request.RowsPerPage;
                skip = request.RowsPerPage * (request.Page - 1);
                items = rows.Skip(skip).Take(take).ToList();
            }
            else
            {
                items = rows;
                return new PagingPageModel<T>()
                {
                    Items = items,
                    TotalItems = items.Count
                };
            }

            return new PagingPageModel<T>()
            {
                Items = items,
                TotalItems = rows.Count()
            };
        }

        protected List<T> SyncDataset(List<T> rows, PagingSyncModel<T> syncModel)
        {
            rows = rows.Concat(syncModel.AddedRows).Where(_ => !syncModel.RowsForDeletion.Contains(_.Id)).ToList();

            for (var i = 0; i < rows.Count; i++)
            {
                var item = syncModel.UpdateRows.FirstOrDefault(row => row.Id == rows[i].Id);
                if (item != null)
                    rows[i] = item;
            }

            return rows;
        }
        protected List<T> OrderByColumn(List<T> rows, string sortColumn, bool isDescending);
        protected List<T> SearchRows(List<T> rows, string search);
        protected List<T> GetScenarioRows(Guid scenarioId);
        protected List<T> GetLibraryRows(Guid libraryId);
        protected List<T> CreateAsNewDataset(List<T> rows);

    }
}
