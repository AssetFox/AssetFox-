using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class SimulationLogDTO
    {
        public Guid Id { get; set; }
        public Guid SimulationId { get; set; }
        public int Status { get; set; }
        public int Subject { get; set; }
        public string Message { get; set; }
        /// <summary>TimeStamp is isgnored when saving to the database,
        /// as it has its own timestamping. But when loading from the database,
        /// the TimeStamp is filled with the CreatedDate property of the entity.</summary>
        public DateTime TimeStamp { get; set; }
    }
}
