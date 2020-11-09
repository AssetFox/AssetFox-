using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract
{
    public abstract class TreatmentEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int ShadowForAnyTreatment { get; set; }
        public int ShadowForSameTreatment { get; set; }
    }
}
