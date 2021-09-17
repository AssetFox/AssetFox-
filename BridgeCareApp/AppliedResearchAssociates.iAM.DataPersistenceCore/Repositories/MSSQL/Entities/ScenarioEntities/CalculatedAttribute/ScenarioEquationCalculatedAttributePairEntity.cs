﻿using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CalculatedAttribute
{
    public class ScenarioEquationCalculatedAttributePairEntity : BaseEquationJoinEntity
    {
        public Guid ScenarioCalculatedAttributePairId { get; set; }

        public virtual ScenarioCalculatedAttributeEquationCriteriaPairEntity ScenarioCalculatedAttributePair { get; set; }
    }
}
