using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Data.Attributes;
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

        List<AttributeDTO> GetAttributes();

        Task<List<RuleDefinition>> GetAggregationRules();

        Task<List<string>> GetAttributeDataTypes();
        Task<List<string>> GetAttributeDataSourceTypes();
        Task<List<AttributeDTO>> GetAttributesAsync();

        Task<List<AttributeDTO>> CalculatedAttributes();
        AttributeDTO GetSingleById(Guid attributeId);
        /// <summary>Case insensitive search. If no attribute with the given name is found,
        /// returns null without throwing. Also, this method is necessarily somewhat
        /// inefficient. To perform the case-insensitive comparison, it pulls everything into memory.</summary>
        AttributeDTO GetSingleByName(string attributeName);

        /// <summary>Case insensitive search. If no attribute with the a given name is found,
        /// simply does not return an attribute for that name. Also, this method is necessarily somewhat
        /// inefficient. To perform the case-insensitive comparison, it pulls everything into memory.</summary>
        List<AttributeDTO> GetAttributesWithNames(List<string> attributeNames);

        void DeleteAttributesShouldNeverBeNeededButSometimesIs(List<Guid> attributeIdsToDelete);

        string GetEncryptionKey();

        string GetAttributeName(Guid attributeId);
        List<AttributeDTO> GetAllAttributesAbbreviated();
    }
}
