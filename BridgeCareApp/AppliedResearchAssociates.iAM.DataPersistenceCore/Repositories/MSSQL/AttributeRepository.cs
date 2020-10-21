using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AttributeRepository : MSSQLRepository, IAttributeRepository
    {
        private readonly IAttributeMetaDataRepository _attributeMetaDataRepo;

        public AttributeRepository(IAMContext context, IAttributeMetaDataRepository attributeMetaDataRepo) :
            base(context) =>
            _attributeMetaDataRepo =
                attributeMetaDataRepo ?? throw new ArgumentNullException(nameof(attributeMetaDataRepo));

        public Dictionary<Guid, Attribute> AttributeDictionary { get; set; }

        //public Dictionary<Guid, Attribute> GetAttributeDictionary(string filePath)
        //{
        //    if (AttributeDictionary == null || !AttributeDictionary.Any())
        //    {
        //        AttributeDictionary = new Dictionary<Guid, Attribute>();

        //        var attributeMetaData = _attributeMetaDataRepo.All(filePath);
        //        if (attributeMetaData.Any())
        //        {
        //            foreach (var attributeMetaDatum in attributeMetaData)
        //            {
        //                if (!AttributeDictionary.ContainsKey(attributeMetaDatum.Id))
        //                {
        //                    AttributeDictionary.Add(attributeMetaDatum.Id, AttributeFactory.Create(attributeMetaDatum));
        //                }
        //            }
        //        }
        //    }

        //    return AttributeDictionary;
        //}

        public void CreateMissingAttributes(List<Attribute> attributes)
        {
            var existingAttributeIds = Context.Attributes.Select(_ => _.Id).ToList();
            if (attributes.Any(_ => !existingAttributeIds.Contains(_.Id)))
            {
                Context.Attributes.AddRange(attributes.Where(_ => !existingAttributeIds.Contains(_.Id))
                    .Select(_ => _.ToEntity()));
                Context.SaveChanges();
            }
        }

        public IEnumerable<Attribute> GetAttributesFromNetwork(Guid networkId)
        {
            if (!Context.Networks.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"Could not find network with id {networkId}");
            }

            var maintainableAssets = Context.MaintainableAssets
                .Include(_ => _.AttributeData)
                .ThenInclude(_ => _.Attribute)
                .Where(_ => _.NetworkId == networkId);

            if (!maintainableAssets.Any())
            {
                return new List<Attribute>();
            }

            return maintainableAssets.SelectMany(_ =>
                _.AttributeData.Select(__ => __.Attribute).DistinctBy(__ => __.Id).Select(__ => __.ToDomain()));
        }
    }
}
