using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using DataAttribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public static class IAttributeRepositoryExtensions
    {
        public static void UpsertAttributes(this IAttributeRepository repository, List<AttributeDTO> dtos)
        {
            var dataAttributes = new List<DataAttribute>();
            foreach (var dto in dtos)
            {
                var mappedDto = AttributeMapper.ToDomain(dto);
                var valid = AttributeValidityChecker.IsValid(mappedDto);
                if (!valid)
                {
                    throw new Exception($"Invalid attribute {mappedDto.Name} with aggregation rule {mappedDto.AggregationRuleType}");
                }
                if (mappedDto!=null)
                {
                    dataAttributes.Add(mappedDto);
                }
                else
                {
                    throw new Exception($"Invalid attribute {dto.Name}");
                }
            }
            repository.UpsertAttributes(dataAttributes);
        }

        public static void UpsertAttributes(this IAttributeRepository repository, params AttributeDTO[] dtos)
        {
            repository.UpsertAttributes(dtos.ToList());
        }

        public static void UpsertAttributes(this IAttributeRepository repo, params DataAttribute[] attributes)
            => repo.UpsertAttributes(attributes.ToList());
    }
}
