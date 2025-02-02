﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.Data.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using DataAttribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public static class AttributeRepositoryExtensions
    {
        public static void UpsertAttributes(this IAttributeRepository repository, List<AttributeDTO> dtos)
        {
            var dataAttributes = MapToDataAttributes(repository, dtos);
            repository.UpsertAttributes(dataAttributes);
        }

        public static void UpsertAttributes(this IAttributeRepository repository, params AttributeDTO[] dtos)
        {
            repository.UpsertAttributes(dtos.ToList());
        }

        public static void UpsertAttributes(this IAttributeRepository repo, params DataAttribute[] attributes)
            => repo.UpsertAttributesNonAtomic(attributes.ToList());

        // Helper method to map DTOs to DataAttribute objects
        private static List<DataAttribute> MapToDataAttributes(IAttributeRepository repository, List<AttributeDTO> dtos)
        {
            var dataAttributes = new List<DataAttribute>();
            foreach (var dto in dtos)
            {
                var mappedDto = AttributeDtoDomainMapper.ToDomain(dto, repository.GetEncryptionKey());
                var valid = AttributeValidityChecker.IsValid(mappedDto);
                if (!valid)
                {
                    throw new InvalidAttributeException($"Invalid attribute {mappedDto.Name} with aggregation rule {mappedDto.AggregationRuleType}");
                }
                if (mappedDto != null)
                {
                    dataAttributes.Add(mappedDto);
                }
                else
                {
                    throw new AttributeMappingFailureException($"Invalid attribute {dto.Name}");
                }
            }
            return dataAttributes;
        }
    }
}
