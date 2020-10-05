using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AttributeDatumRepository<T> : MSSQLRepository<AttributeDatum<T>, AttributeDatumEntity>, IAttributeDatumDataRepository
    {
        public AttributeDatumRepository(IAMContext context) : base(context)
        {
        }

#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
        public void AddAttributeDatum<T>(AttributeDatum<T> domain, string uniqueIdentifier)
        {
            var segmentEntity = context.Segments
                .First(e => e.UniqueIdentifier == uniqueIdentifier);

            context.AttributeData.Add(domain.ToEntity(segmentEntity.Id, segmentEntity.LocationId));
        }
#pragma warning restore CS0693 // Type parameter has the same name as the type parameter from outer type
    }
}
