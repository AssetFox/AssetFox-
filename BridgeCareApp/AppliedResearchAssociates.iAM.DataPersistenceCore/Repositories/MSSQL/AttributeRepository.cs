using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using Microsoft.EntityFrameworkCore;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AttributeRepository : MSSQLRepository, IAttributeRepository
    {
        public AttributeRepository(IAMContext context) : base(context) { }

        public void CreateAttributes(IEnumerable<Attribute> attributes)
        {
            Context.Attributes.AddRange(attributes.Select(_ => _.ToEntity()));
            Context.SaveChanges();
        }

        public IEnumerable<Attribute> GetAttributesFromNetwork(Guid networkId)
        {
            if (!Context.Networks.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"Could not find network with id {networkId}");
            }

            var network = Context.Networks.Include(_ => _.MaintainableAssets)
                .ThenInclude(_ => _.AttributeData)
                .ThenInclude(_ => _.Attribute)
                .Single(_ => _.Id == networkId);

            if (network == null)
            {
                return new List<Attribute>();
            }

            return network.MaintainableAssets.SelectMany(_ =>
            {
                var attributes = _.AttributeData.Select(__ => __.Attribute).Distinct();
                return attributes.Select(__ => __.ToDomain());
            });
        }
    }
}
