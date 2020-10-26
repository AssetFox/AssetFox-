using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using MoreLinq.Extensions;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AttributeRepository : MSSQLRepository, IAttributeRepository
    {
        private readonly IMaintainableAssetRepository _maintainableAssetRepo;

        public AttributeRepository(IMaintainableAssetRepository maintainableAssetRepo,
            IAMContext context) :
            base(context) =>
            _maintainableAssetRepo =
                maintainableAssetRepo ?? throw new ArgumentNullException(nameof(maintainableAssetRepo));

        public Dictionary<Guid, Attribute> AttributeDictionary { get; set; }

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

            var maintainableAssets =
                _maintainableAssetRepo.GetAllInNetworkWithAssignedDataAndLocations(networkId).ToList();

            if (!maintainableAssets.Any())
            {
                return new List<Attribute>();
            }

            return maintainableAssets.SelectMany(_ =>
                _.AssignedData.Select(__ => __.Attribute).DistinctBy(__ => __.Id));
        }
    }
}
