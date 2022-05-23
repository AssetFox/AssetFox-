using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAttributeRepository
    {
        void UpsertAttributes(List<Attribute> attributes);

        void JoinAttributesWithEquationsAndCriteria(Explorer explorer);

        Explorer GetExplorer();

        List<Guid> GetAttributeIdsInNetwork(Guid networkId);

        Task<List<AttributeDTO>> Attributes();

        Task<List<AttributeDTO>> CalculatedAttributes();
        AttributeDTO GetSingleById(Guid attributeId);
    }
}
