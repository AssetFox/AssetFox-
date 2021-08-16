using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.RemainingLifeLimit
{
    public class ScenarioRemainingLifeLimitEntity : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid SimulationId { get; set; }

        public Guid AttributeId { get; set; }

        public double Value { get; set; }

        public virtual AttributeEntity Attribute { get; set; }

        public virtual SimulationEntity Simulation { get; set; }

        public virtual CriterionLibraryScenarioRemainingLifeLimitEntity CriterionLibraryScenari0RemainingLifeLimitJoin { get; set; }
    }
}
