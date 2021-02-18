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
        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public BenefitRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork) => _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

        public void CreateBenefit(Benefit benefit, Guid analysisMethodId)
        {
            if (!_unitOfDataPersistenceWork.Context.AnalysisMethod.Any(_ => _.Id == analysisMethodId))
            {
                throw new RowNotInTableException($"No analysis method found having id {analysisMethodId}");
            }

            AttributeEntity attributeEntity = null;
            if (benefit.Attribute != null)
            {
                if (!_unitOfDataPersistenceWork.Context.Attribute.Any(_ => _.Name == benefit.Attribute.Name))
                {
                    throw new RowNotInTableException($"No attribute found having name {benefit.Attribute.Name}.");
                }

                attributeEntity = _unitOfDataPersistenceWork.Context.Attribute.Single(_ => _.Name == benefit.Attribute.Name);
            }

            _unitOfDataPersistenceWork.Context.Benefit.Add(benefit.ToEntity(analysisMethodId, attributeEntity?.Id));
            _unitOfDataPersistenceWork.Context.SaveChanges();
        }
    }
}
