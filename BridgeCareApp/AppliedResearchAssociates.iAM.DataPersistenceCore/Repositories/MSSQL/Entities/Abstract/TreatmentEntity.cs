using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract
{
    public abstract class TreatmentEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int ShadowForAnyTreatment { get; set; }

        public int ShadowForSameTreatment { get; set; }

    }
}
