using System;

namespace BridgeCareCore.Models
{
    public class CommittedProjectFillTreatmentValuesModel
    {
        public Guid CommittedProjectId { get; set; }
        public Guid TreatmentLibraryId { get; set; }
        public string TreatmentName { get; set; }
        public string Brkey_Value { get; set; }
        public Guid NetworkId { get; set; }
    }
}
