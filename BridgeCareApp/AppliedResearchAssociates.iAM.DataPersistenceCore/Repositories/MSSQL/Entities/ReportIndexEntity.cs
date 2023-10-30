using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    /// <summary>
    /// Domain object representing record in data persistence for report data that should be persisted
    /// </summary>
    public class ReportIndexEntity : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid? SimulationID { get; set; }
        public Guid? NetworkID { get; set; }
        public string ReportTypeName { get; set; }
        public string Result { get; set; }
        public DateTime? ExpirationDate { get; set; }

        public virtual SimulationEntity Simulation { get; set; }
    }
}
