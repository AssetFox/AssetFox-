using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using EFCore.BulkExtensions;
using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AttributeRepository : MSSQLRepository, IAttributeRepository
    {
        public static readonly bool IsRunningFromNUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("nunit.framework"));

        private readonly IMaintainableAssetRepository _maintainableAssetRepo;

        public AttributeRepository(IMaintainableAssetRepository maintainableAssetRepo,
            IAMContext context) :
            base(context) =>
            _maintainableAssetRepo =
                maintainableAssetRepo ?? throw new ArgumentNullException(nameof(maintainableAssetRepo));

        public Dictionary<Guid, DataMinerAttribute> AttributeDictionary { get; set; }

        public void UpsertAttributes(List<DataMinerAttribute> attributes)
        {
            if (IsRunningFromNUnit)
            {
                Context.Attribute.AddRange(attributes.Select(_ => _.ToEntity()).ToList());
            }
            else
            {
                Context.BulkInsertOrUpdate(attributes.Select(_ => _.ToEntity()).ToList());
            }

            Context.SaveChanges();
        }
    }
}
