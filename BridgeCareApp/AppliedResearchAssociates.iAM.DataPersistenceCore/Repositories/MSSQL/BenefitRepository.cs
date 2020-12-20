using System;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class BenefitRepository : IBenefitRepository
    {
        private readonly IAMContext _context;

        public BenefitRepository(IAMContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

        public void CreateBenefit(Benefit benefit, Guid analysisMethodId)
        {
            if (!_context.AnalysisMethod.Any(_ => _.Id == analysisMethodId))
            {
                throw new RowNotInTableException($"No analysis method found having id {analysisMethodId}");
            }

            AttributeEntity attributeEntity = null;
            if (benefit.Attribute != null)
            {
                if (!_context.Attribute.Any(_ => _.Name == benefit.Attribute.Name))
                {
                    throw new RowNotInTableException($"No attribute found having name {benefit.Attribute.Name}.");
                }

                attributeEntity = _context.Attribute.Single(_ => _.Name == benefit.Attribute.Name);
            }

            _context.Benefit.Add(benefit.ToEntity(analysisMethodId, attributeEntity?.Id));
        }
    }
}
