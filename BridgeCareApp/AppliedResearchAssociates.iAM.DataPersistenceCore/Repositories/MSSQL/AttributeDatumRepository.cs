using System.Linq;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AttributeDatumRepository<T> : MSSQLRepository<AttributeDatum<T>>, IAttributeDatumDataRepository
    {
        public AttributeDatumRepository(IAMContext context) : base(context) { }

        public void AddAttributeData<T>(IEnumerable<AttributeDatum<T>> domains, Guid segmentId)
        {
            if (!context.Segments.Any(e => e.Id == segmentId))
            {
                throw new RowNotInTableException($"No segment was found using given the id: {segmentId}");
            }

            var maintainableAssetEntity = context.MaintainableAssets
                .Single(e => e.Id == segmentId);

            context.AttributeData.AddRange(domains.Select(d => d.ToEntity(maintainableAssetEntity.Id, maintainableAssetEntity.LocationId)));
        }

#pragma warning restore CS0693 // Type parameter has the same name as the type parameter from outer type
    }
}
