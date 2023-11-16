using System;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class BenefitRepository : IBenefitRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public BenefitRepository(UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void UpsertBenefit(BenefitDTO dto, Guid analysisMethodId)
        {
            if (!_unitOfWork.Context.AnalysisMethod.Any(_ => _.Id == analysisMethodId))
            {
                throw new RowNotInTableException("The specified analysis method was not found.");
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
