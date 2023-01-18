using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Enums;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class SimulationOutputEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid SimulationId { get; set; }

        public virtual SimulationEntity Simulation { get; set; }

        public double InitialConditionOfNetwork { get; set; }

        public virtual ICollection<AssetSummaryDetailEntity> InitialAssetSummaries { get; set; }

        public virtual ICollection<SimulationYearDetailEntity> Years { get; set; }

        public virtual ICollection<SimulationOutputJsonEntity> SimulationOutputJsons { get; set; }
    }
}
