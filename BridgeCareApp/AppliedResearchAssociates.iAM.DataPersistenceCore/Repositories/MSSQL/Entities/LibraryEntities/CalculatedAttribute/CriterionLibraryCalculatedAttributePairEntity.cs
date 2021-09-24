﻿using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CalculatedAttribute
{
    public class CriterionLibraryCalculatedAttributePairEntity : BaseCriterionLibraryJoinEntity
    {
        public Guid CalculatedAttributePairId { get; set; }

        public virtual CalculatedAttributeEquationCriteriaPairEntity CalculatedAttributePair { get; set; }
    }
}