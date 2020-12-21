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
        private readonly UnitOfWork.UnitOfWork _unitOfWork;

        public BenefitRepository(UnitOfWork.UnitOfWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateBenefit(Benefit benefit, Guid analysisMethodId)
        {
            if (!_unitOfWork.Context.AnalysisMethod.Any(_ => _.Id == analysisMethodId))
            {
                throw new RowNotInTableException($"No analysis method found having id {analysisMethodId}");
            }

            AttributeEntity attributeEntity = null;
            if (benefit.Attribute != null)
            {
                if (!_unitOfWork.Context.Attribute.Any(_ => _.Name == benefit.Attribute.Name))
                {
                    throw new RowNotInTableException($"No attribute found having name {benefit.Attribute.Name}.");
                }

                attributeEntity = _unitOfWork.Context.Attribute.Single(_ => _.Name == benefit.Attribute.Name);
            }

            _unitOfWork.Context.Benefit.Add(benefit.ToEntity(analysisMethodId, attributeEntity?.Id));
            _unitOfWork.Context.SaveChanges();
        }
    }
}
