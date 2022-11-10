using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve
{
    public class PerformanceCurveLibraryUserEntity
    {
        public Guid PerformanceCurveLibraryId { get; set; }
        public Guid UserId { get; set; }
        public int AccessLevel { get; set; }

        public virtual PerformanceCurveLibraryEntity PerformanceCurveLibrary { get; set; }
        public virtual UserEntity User { get; set; }
    }
}
