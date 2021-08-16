using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.RemainingLifeLimit
{
    public class CriterionLibraryScenarioRemainingLifeLimitEntity : BaseEntity
    {
        public Guid CriterionLibraryId { get; set; }

        public Guid ScenarioRemainingLifeLimitId { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }

        public virtual ScenarioRemainingLifeLimitEntity ScenarioRemainingLifeLimit { get; set; }
    }
}
