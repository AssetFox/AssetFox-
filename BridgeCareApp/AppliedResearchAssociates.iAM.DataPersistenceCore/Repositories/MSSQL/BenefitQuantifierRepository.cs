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
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public BenefitQuantifierRepository(UnitOfDataPersistenceWork unitOfDataPersistenceWork) =>
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ??
                                         throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

        public BenefitQuantifierDTO GetBenefitQuantifier(Guid networkId)
        {
            if (!_unitOfDataPersistenceWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}.");
            }

            if (!_unitOfDataPersistenceWork.Context.BenefitQuantifier.Any(_ => _.NetworkId == networkId))
            {
                return new BenefitQuantifierDTO
                {
                    NetworkId = networkId, Equation = new EquationDTO {Id = Guid.NewGuid()}
                };
            }

            return _unitOfDataPersistenceWork.Context.BenefitQuantifier.Single(_ => _.NetworkId == networkId).ToDto();
        }

        public void UpsertBenefitQuantifier(BenefitQuantifierDTO dto, UserInfoDTO userInfo)
        {
            if (!_unitOfDataPersistenceWork.Context.Network.Any(_ => _.Id == dto.NetworkId))
            {
                throw new RowNotInTableException($"No network found having id {dto.NetworkId}.");
            }

            var userEntity = _unitOfDataPersistenceWork.Context.User.SingleOrDefault(_ => _.Username == userInfo.Sub);

            var equationEntity = dto.Equation.ToEntity();

            _unitOfDataPersistenceWork.Context.Upsert(equationEntity, equationEntity.Id, userEntity?.Id);

            var benefitQuantifierEntity = dto.ToEntity();

            _unitOfDataPersistenceWork.Context.Upsert(benefitQuantifierEntity, _ => _.NetworkId == dto.NetworkId,
                userEntity?.Id);
        }

        public void DeleteBenefitQuantifier(Guid networkId)
        {
            _unitOfDataPersistenceWork.Context.DeleteEntity<EquationEntity>(_ => _.BenefitQuantifier.NetworkId == networkId);

            _unitOfDataPersistenceWork.Context.DeleteEntity<BenefitQuantifierEntity>(_ => _.NetworkId == networkId);
        }
    }
}
