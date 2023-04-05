using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;

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
                throw new RowNotInTableException("The specified network was not found.");
            }

            if (!_unitOfWork.Context.BenefitQuantifier.Any(_ => _.NetworkId == networkId))
            {
                return new BenefitQuantifierDTO
                {
                    NetworkId = networkId, Equation = new EquationDTO {Id = Guid.NewGuid()}
                };
            }

            return _unitOfWork.Context.BenefitQuantifier
                .Include(_ => _.Equation)
                .Single(_ => _.NetworkId == networkId).ToDto();
        }

        public void UpsertBenefitQuantifier(BenefitQuantifierDTO dto)
        {
            _unitOfWork.AsTransaction(() => _unitOfWork.BenefitQuantifierRepo.UpsertBenefitQuantifierNonAtomic(dto));
        }

        public void UpsertBenefitQuantifierNonAtomic(BenefitQuantifierDTO dto)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == dto.NetworkId))
            {
                throw new RowNotInTableException("The specified network was not found.");
            }

                var attributes = _unitOfWork.Context.Attribute.ToList();
                var stringAttributes = attributes.Where(_ => _.DataType == "STRING").ToList();
                var numberAttributes = attributes.Where(_ => _.DataType == "NUMBER").ToList();
                CheckEquationAttributes(stringAttributes, numberAttributes, dto.Equation.Expression);

                var equationEntity = dto.Equation.ToEntity();

                _unitOfWork.Context.Upsert(equationEntity, equationEntity.Id, _unitOfWork.UserEntity?.Id);

                var benefitQuantifierEntity = dto.ToEntity();

                _unitOfWork.Context.Upsert(benefitQuantifierEntity, _ => _.NetworkId == dto.NetworkId,
                    _unitOfWork.UserEntity?.Id);
        }

        private void CheckEquationAttributes(List<AttributeEntity> stringAttributes, List<AttributeEntity> numberAttributes, string target)
        {
            {
                if (stringAttributes.Any(_ => target.Contains(_.Name)))
                {
                    var stringAttributesInEquation = stringAttributes.Where(_ => target.Contains(_.Name)).ToList();
                    throw new InvalidOperationException(
                        $"Unsupported string attribute(s) found in benefit quantifier equation expression: {string.Join(", ", stringAttributesInEquation)}.");
                }

                target = target.Replace('[', '?');
                foreach (var allowedAttribute in numberAttributes.Where(allowedAttribute =>
                    target.IndexOf("?" + allowedAttribute.Name + "]", StringComparison.Ordinal) >= 0))
                {
                    target = target.Replace("?" + allowedAttribute.Name + "]", "[" + allowedAttribute.Name + "]");
                }

                if (target.Count(f => f == '?') <= 0)
                {
                    return;
                }

                var invalidAttributes = new List<string>();

                do
                {
                    var start = target.IndexOf('?');
                    var end = target.IndexOf(']');
                    var invalidAttribute = target.Substring(start + 1, end - 1);
                    invalidAttributes.Add(invalidAttribute);
                    var invalidAttributePosition = target.IndexOf($"?{invalidAttribute}]", StringComparison.Ordinal);
                    target = invalidAttributePosition + 1 <= target.Length
                        ? target.Substring(invalidAttributePosition + 1)
                        : "";
                } while (target.Contains("?"));

                throw new InvalidOperationException(
                    $"Unsupported attribute(s) found in benefit quantifier equation expression: {string.Join(", ", invalidAttributes)}");
            }
        }

        public void DeleteBenefitQuantifier(Guid networkId)
        {
            _unitOfWork.Context.DeleteEntity<BenefitQuantifierEntity>(_ => _.NetworkId == networkId);
        }
    }
}
