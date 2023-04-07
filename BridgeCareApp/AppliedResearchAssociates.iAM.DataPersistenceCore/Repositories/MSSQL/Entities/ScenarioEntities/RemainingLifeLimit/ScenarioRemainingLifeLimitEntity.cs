using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.RemainingLifeLimit
{
    public class ScenarioRemainingLifeLimitEntity : BaseRemainingLifeLimitEntity
    {
        public Guid SimulationId { get; set; }

        public double Value { get; set; }

        public Guid LibraryId { get; set; }

        public bool IsModified { get; set; }

        public virtual SimulationEntity Simulation { get; set; }

        public virtual CriterionLibraryScenarioRemainingLifeLimitEntity CriterionLibraryScenarioRemainingLifeLimitJoin { get; set; }
    }
}
