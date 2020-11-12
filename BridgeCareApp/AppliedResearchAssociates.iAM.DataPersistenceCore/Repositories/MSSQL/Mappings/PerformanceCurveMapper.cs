using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class PerformanceCurveMapper
    {
        public static PerformanceCurveEntity ToEntity(this PerformanceCurve domain, Guid performanceCurveLibraryId, Guid attributeId) =>
            new PerformanceCurveEntity
            {
                Id = Guid.NewGuid(),
                PerformanceCurveLibraryId = performanceCurveLibraryId,
                AttributeId = attributeId,
                Name = domain.Name,
                Shift = domain.Shift
            };

        public static PerformanceCurve ToDomain(this PerformanceCurveEntity domain)
        {
            /*return new PerformanceCurve(new Explorer())
            {
                Attribute = new NumberAttribute()
            };*/
            return null;
        }
    }
}
