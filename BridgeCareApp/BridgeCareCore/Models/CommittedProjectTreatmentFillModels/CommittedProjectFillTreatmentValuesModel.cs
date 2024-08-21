using System;

namespace BridgeCareCore.Models
{
    public class CommittedProjectFillTreatmentValuesModel
    {
        public Guid CommittedProjectId { get; set; }
        public Guid TreatmentId { get; set; }
        public string KeyAttributeValue { get; set; }
        public Guid NetworkId { get; set; }
    }
}
