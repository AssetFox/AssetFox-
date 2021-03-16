using System.Collections.Generic;

namespace BridgeCareCore.Models
{
    public class AttributeSelectValuesResult
    {
        public string Attribute { get; set; }
        public List<string> Values { get; set; }
        public string ResultMessage { get; set; }
        public string ResultType { get; set; }
    }
}
