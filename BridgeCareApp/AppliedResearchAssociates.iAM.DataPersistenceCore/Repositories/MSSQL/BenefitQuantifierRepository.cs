using System;
using System.Data;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class BenefitQuantifierRepository : IBenefitQuantifierRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public BenefitQuantifierRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public BenefitQuantifierDTO GetBenefitQuantifier(Guid networkId)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}.");
            }

            if (!_unitOfWork.Context.BenefitQuantifier.Any(_ => _.NetworkId == networkId))
            {
                return new BenefitQuantifierDTO
                {
                    NetworkId = networkId, Equation = new EquationDTO {Id = Guid.NewGuid()}
                };
            }

            return _unitOfWork.Context.BenefitQuantifier.Single(_ => _.NetworkId == networkId).ToDto();
        }

        public void UpsertBenefitQuantifier(BenefitQuantifierDTO dto)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == dto.NetworkId))
            {
                throw new RowNotInTableException($"No network found having id {dto.NetworkId}.");
            }

            var equationEntity = dto.Equation.ToEntity();

            _unitOfWork.Context.Upsert(equationEntity, equationEntity.Id, _unitOfWork.UserEntity?.Id);

            var benefitQuantifierEntity = dto.ToEntity();

            _unitOfWork.Context.Upsert(benefitQuantifierEntity, _ => _.NetworkId == dto.NetworkId,
                _unitOfWork.UserEntity?.Id);
        }

        public void DeleteBenefitQuantifier(Guid networkId)
        {
            _unitOfWork.Context.DeleteEntity<EquationEntity>(_ => _.BenefitQuantifier.NetworkId == networkId);

            _unitOfWork.Context.DeleteEntity<BenefitQuantifierEntity>(_ => _.NetworkId == networkId);
        }
    }
}
