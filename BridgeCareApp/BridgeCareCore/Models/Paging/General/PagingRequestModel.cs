using System;
using System.Collections.Generic;

namespace BridgeCareCore.Models
{
    public class PagingRequestModel<T> : BasePagingRequest
    {
        public PagingRequestModel() :base()
        {
            PagingSync = new PagingSyncModel<T>();
        }
        public PagingSyncModel<T> PagingSync {get;set;}
    }
}
