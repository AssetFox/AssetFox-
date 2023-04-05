using System;
using System.Collections.Generic;

namespace BridgeCareCore.Models
{
    public class PagingSyncModel<T>
    {
        public PagingSyncModel()
        {
            RowsForDeletion = new List<Guid>();
            UpdateRows = new List<T>();
            AddedRows = new List<T>();
        }
        public List<Guid> RowsForDeletion { get; set; }
        public List<T> UpdateRows { get; set; }
        public List<T> AddedRows { get; set; }
        public Guid? LibraryId { get; set; }
        public bool IsModified { get; set; }
    }
}
