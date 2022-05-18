using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using DataAttribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public static class IAttributeRepositoryExtensions
    {
        public static void UpsertAttributes(this IAttributeRepository repository, List<AttributeDTO> dtos)
        {
            var dataMinerAttributes = new List<DataAttribute>();
            foreach (var dto in dtos)
            {
                var mappedDto = AttributeMapper.ToDomain(dto);
                if (mappedDto!=null)
                {
                    dataMinerAttributes.Add(mappedDto);
                }
            }
            repository.UpsertAttributes(dataMinerAttributes);
        }

        public static void UpsertAttributes(this IAttributeRepository repository, params AttributeDTO[] dtos)
        {
            repository.UpsertAttributes(dtos.ToList());
        }

        public static void UpsertAttributes(this IAttributeRepository repo, params DataAttribute[] attributes)
            => repo.UpsertAttributes(attributes.ToList());
    }
}
