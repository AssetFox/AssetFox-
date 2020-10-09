using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.MSSQL.Mappings;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AttributeDatumRepository<T> : MSSQLRepository<AttributeDatum<T>>
    {
        public AttributeDatumRepository(IAMContext context) : base(context) { }

        public override async void AddAll(IEnumerable<AttributeDatum<T>> domainAttributeData, params object[] args)
        {
            if (!args.Any())
            {
                throw new NullReferenceException("No arguments found for attribute data query");
            }

            var maintainableAssetId = (Guid)args[0];

            if (!context.MaintainableAssets.Any(e => e.Id == maintainableAssetId))
            {
                throw new RowNotInTableException($"No maintainable asset was found using given the id: {maintainableAssetId}");
            }

            var maintainableAssetEntity = context.MaintainableAssets
                .Single(e => e.Id == maintainableAssetId);

            await context.AttributeData.AddRangeAsync(domainAttributeData.Select(d =>
                d.ToEntity(maintainableAssetEntity.Id, maintainableAssetEntity.LocationId)));
        }
    }
}
