using System;
using System.Collections.Generic;

namespace BridgeCareCore.Models
{
    public class PagingSyncModel<T>
    {
        public Guid? LibraryId { get; set; }
        public List<Guid> RowsForDeletion { get; set; }
        public List<T> UpdateRows { get; set; }
        public List<T> AddedRows { get; set; }
    }
}
