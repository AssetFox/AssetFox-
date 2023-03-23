using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CalculatedAttribute
{
    public class ScenarioCalculatedAttributeEntity : BaseCalculatedAttributeEntity
    {
        public ScenarioCalculatedAttributeEntity() => Equations = new HashSet<ScenarioCalculatedAttributeEquationCriteriaPairEntity>();

        public Guid SimulationId { get; set; }

        public Guid LibraryId { get; set; }

        public virtual SimulationEntity Simulation { get; set; }

        public ICollection<ScenarioCalculatedAttributeEquationCriteriaPairEntity> Equations { get; set; }
    }
}
