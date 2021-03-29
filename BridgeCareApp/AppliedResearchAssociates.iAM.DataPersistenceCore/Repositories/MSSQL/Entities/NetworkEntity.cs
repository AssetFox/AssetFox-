using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class NetworkEntity : BaseEntity
    {
        public NetworkEntity()
        {
            MaintainableAssets = new HashSet<MaintainableAssetEntity>();
            Simulations = new HashSet<SimulationEntity>();
            Facilities = new HashSet<FacilityEntity>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual NetworkRollupDetailEntity NetworkRollupDetail { get; set; }

        public virtual BenefitQuantifierEntity BenefitQuantifier { get; set; }

        public virtual ICollection<MaintainableAssetEntity> MaintainableAssets { get; set; }

        public virtual ICollection<SimulationEntity> Simulations { get; set; }

        public virtual ICollection<FacilityEntity> Facilities { get; set; }
    }
}
