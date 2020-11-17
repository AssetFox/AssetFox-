﻿using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CriterionLibraryTreatmentEntity
    {
        public Guid CriterionLibraryId { get; set; }

        public Guid TreatmentId { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }

        public virtual SelectableTreatmentEntity SelectableTreatment { get; set; }
    }
}
