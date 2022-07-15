using System;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class ReportIndexDTO : BaseDTO
    {
        public Guid? SimulationId { get; set; }
        public string Type { get; set; }
        public string Result { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
