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
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        public AttributeRepository(IAMContext context) : base(context) { }

        public Dictionary<Guid, DataMinerAttribute> AttributeDictionary { get; set; }

        public void UpsertAttributes(List<DataMinerAttribute> attributes)
        {
            if (IsRunningFromXUnit)
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
