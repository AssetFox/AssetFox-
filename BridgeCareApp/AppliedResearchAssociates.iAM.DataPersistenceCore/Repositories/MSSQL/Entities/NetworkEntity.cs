using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class NetworkEntity : BaseEntity
    {
        public NetworkEntity()
        {
            Simulations = new HashSet<SimulationEntity>();

            // Looks weird because sections and maintainable assets are identical conceptually, but
            // I think these two collections exist because one is for entities that correspond to
            // non-analysis "maintainable assets" and the other is for entities that correspond to
            // analysis "maintainable assets", which used to be called "sections".
            MaintainableAssets = new HashSet<MaintainableAssetEntity>();
            Sections = new HashSet<SectionEntity>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual NetworkRollupDetailEntity NetworkRollupDetail { get; set; }

        public virtual BenefitQuantifierEntity BenefitQuantifier { get; set; }

        public virtual ICollection<MaintainableAssetEntity> MaintainableAssets { get; set; }

        public virtual ICollection<SimulationEntity> Simulations { get; set; }

        public virtual ICollection<SectionEntity> Sections { get; set; }
    }
}
