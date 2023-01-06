using System;
using System.Collections.Generic;

namespace BridgeCareCore.Models
{
    public class PagingRequestModel<T> : BasePagingRequest
    {
        public PagingRequestModel() :base()
        {
            SyncModel = new PagingSyncModel<T>();
        }
        public PagingSyncModel<T> SyncModel {get;set;}
    }
}
