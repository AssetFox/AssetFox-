using System.Collections.Generic;

namespace BridgeCareCore.Models
{
    public class QueryResponse
    {
        public string Attribute { get; set; }
        public List<string> Values { get; set; }
    }
}
