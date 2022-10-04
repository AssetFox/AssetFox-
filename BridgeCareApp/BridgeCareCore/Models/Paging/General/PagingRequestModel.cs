using System;
using System.Collections.Generic;

namespace BridgeCareCore.Models
{
    public class PagingRequestModel<T> : BasePagingRequest
    {
        public PagingRequestModel()
        {
            PagingSync = new PagingSyncModel<T>();
        }
        public PagingSyncModel<T> PagingSync {get;set;}
    }
}
