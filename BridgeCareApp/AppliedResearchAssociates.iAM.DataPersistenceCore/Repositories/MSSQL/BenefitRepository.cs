using System;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class BenefitRepository : MSSQLRepository, IBenefitRepository
    {
        public BenefitRepository(IAMContext context) : base(context) { }

        public void CreateBenefit(Benefit benefit, Guid analysisMethodId)
        {
            if (!Context.AnalysisMethod.Any(_ => _.Id == analysisMethodId))
            {
                throw new RowNotInTableException($"No analysis method found having id {analysisMethodId}");
            }

            AttributeEntity attributeEntity = null;
            if (benefit.Attribute != null)
            {
                if (!Context.Attribute.Any(_ => _.Name == benefit.Attribute.Name))
                {
                    throw new RowNotInTableException($"No attribute found having name {benefit.Attribute.Name}.");
                }

                attributeEntity = Context.Attribute.Single(_ => _.Name == benefit.Attribute.Name);
            }

            Context.Benefit.Add(benefit.ToEntity(analysisMethodId, attributeEntity?.Id));
            Context.SaveChanges();
        }
    }
}
