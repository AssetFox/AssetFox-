using System;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class BenefitRepository : IBenefitRepository
    {
        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public BenefitRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateBenefit(Benefit benefit, Guid analysisMethodId)
        {
            if (!_unitOfWork.Context.AnalysisMethod.Any(_ => _.Id == analysisMethodId))
            {
                throw new RowNotInTableException($"No analysis method found having id {analysisMethodId}");
            }

            if (benefit.Attribute == null || string.IsNullOrEmpty(benefit.Attribute.Name))
            {

                throw new InvalidOperationException("Analysis method benefit must have an attribute.");
            }

            if (!_unitOfWork.Context.Attribute.Any(_ => _.Name == benefit.Attribute.Name))
            {
                throw new RowNotInTableException($"No attribute found having name {benefit.Attribute.Name}.");
            }

            var attributeEntity = _unitOfWork.Context.Attribute.Single(_ => _.Name == benefit.Attribute.Name);

            _unitOfWork.Context.AddEntity(benefit.ToEntity(analysisMethodId, attributeEntity.Id),
                _unitOfWork.UserEntity?.Id);
        }

        public void UpsertBenefit(BenefitDTO dto, Guid analysisMethodId)
        {
            if (!_unitOfWork.Context.AnalysisMethod.Any(_ => _.Id == analysisMethodId))
            {
                throw new RowNotInTableException($"No simulation analysis method found having id {analysisMethodId}.");
            }

            if (string.IsNullOrEmpty(dto.Attribute))
            {
                throw new InvalidOperationException("Analysis method benefit must have an attribute.");
            }

            if (!_unitOfWork.Context.Attribute.Any(_ => _.Name == dto.Attribute))
            {
                throw new RowNotInTableException($"No attribute found having name {dto.Attribute}.");
            }

            var attributeEntity = _unitOfWork.Context.Attribute.Single(_ => _.Name == dto.Attribute);

            var benefitEntity = dto.ToEntity(analysisMethodId, attributeEntity.Id);

            _unitOfWork.Context.Upsert(benefitEntity, _ => _.Id == dto.Id, _unitOfWork.UserEntity?.Id);
        }
    }
}
