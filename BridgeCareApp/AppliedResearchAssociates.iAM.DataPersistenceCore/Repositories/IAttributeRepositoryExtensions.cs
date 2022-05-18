using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using DataMinerAttribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public static class IAttributeRepositoryExtensions
    {
        public static void UpsertAttributes(this IAttributeRepository repository, List<AttributeDTO> dtos)
        {
            var dataMinerAttributes = AttributeMapper.ToDomainListButDiscardBad(dtos);
            repository.UpsertAttributes(dataMinerAttributes);
        }

        public static void UpsertAttributes(this IAttributeRepository repository, params AttributeDTO[] dtos)
        {
            repository.UpsertAttributes(dtos.ToList());
        }

        public static void UpsertAttributes(this IAttributeRepository repo, params DataMinerAttribute[] attributes)
            => repo.UpsertAttributes(attributes.ToList());
    }
}
