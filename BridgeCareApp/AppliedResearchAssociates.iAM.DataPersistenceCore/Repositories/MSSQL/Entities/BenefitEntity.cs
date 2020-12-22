using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class BenefitEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid AnalysisMethodId { get; set; }

        public Guid? AttributeId { get; set; }

        public double Limit { get; set; }

        public virtual AnalysisMethodEntity AnalysisMethod { get; set; }

        public virtual AttributeEntity Attribute { get; set; }
    }
}
