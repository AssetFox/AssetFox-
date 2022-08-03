using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class PerformanceCurveLibraryTestSetup
    {

        public static PerformanceCurveLibraryEntity TestPerformanceCurveLibrary(Guid id)
        {
            var entity = new PerformanceCurveLibraryEntity
            {
                Id = id,
                Name = "Test Name"
            };
            return entity;
        }

        public static PerformanceCurveLibraryEntity TestPerformanceCurveLibraryInDb(IUnitOfWork unitOfWork, Guid id)
        {
            var entity = TestPerformanceCurveLibrary(id);
            unitOfWork.Context.Add(entity);
            unitOfWork.Context.SaveChanges();
            return entity;
        }
    }
}
