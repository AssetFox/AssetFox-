using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models.AggregationObjects
{
    public class NetworkMetaData
    {
        public string NetworkName { get; set; }
        public Guid NetworkId { get; set; }
        public string Owner { get; set; }
        public string Creator { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastRun { get; set; }
    }
}
