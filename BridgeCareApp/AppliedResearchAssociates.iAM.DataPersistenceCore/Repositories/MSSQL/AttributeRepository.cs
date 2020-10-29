using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
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

        public void UpsertAttributes(List<Attribute> attributes)
        {
            Context.BulkInsertOrUpdate(attributes.Select(_ => _.ToEntity()).ToList());

            Context.SaveChanges();
        }
    }
}
