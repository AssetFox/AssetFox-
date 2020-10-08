using System.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AttributeDatumRepository<T> : MSSQLRepository<AttributeDatum<T>>
    {
        public AttributeDatumRepository(IAMContext context) : base(context) { }

        public override async void AddAll(IEnumerable<AttributeDatum<T>> domains, params object[] args)
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

            await context.AttributeData.AddRangeAsync(domains.Select(d =>
                d.ToEntity(maintainableAssetEntity.Id, maintainableAssetEntity.LocationId)));
        }
    }
}
