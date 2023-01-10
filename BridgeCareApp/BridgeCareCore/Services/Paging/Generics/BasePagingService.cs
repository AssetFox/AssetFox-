using System.Collections.Generic;
using System;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using BridgeCareCore.Models;
using System.Linq;

namespace BridgeCareCore.Services.Paging.Generics
{
    public abstract class BasePagingService<T>
        where T : BaseDTO
    {
        protected virtual PagingPageModel<T> HandlePaging(List<T> rows, PagingRequestModel<T> request, bool shouldSync = true)
        {
            var skip = 0;
            var take = 0;
            var items = new List<T>();

            if(shouldSync)
                rows = SyncDataset(rows, request.SyncModel);

            if (request.search != null && request.search.Trim() != "")
                rows = SearchRows(rows, request.search);
            if (request.sortColumn != null && request.sortColumn.Trim() != "")
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

        protected virtual List<T> SyncDataset(List<T> rows, PagingSyncModel<T> syncModel)
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
        protected virtual List<T> OrderByColumn(List<T> rows, string sortColumn, bool isDescending) => throw new NotImplementedException();

        protected virtual List<T> SearchRows(List<T> rows, string search) => throw new NotImplementedException();
    }
}
