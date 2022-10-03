using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class PerformanceCurveLibraryTestSetup
    {

        public static PerformanceCurveLibraryDTO TestPerformanceCurveLibrary(Guid id)
        {
            var dto = new PerformanceCurveLibraryDTO
            {
                Id = id,
                Name = "Test Name"
            };
            return dto;
        }

        public static PerformanceCurveLibraryDTO TestPerformanceCurveLibraryInDb(IUnitOfWork unitOfWork, Guid id)
        {
            var dto = TestPerformanceCurveLibrary(id);
            unitOfWork.PerformanceCurveRepo.UpsertPerformanceCurveLibrary(dto);
            return dto;
        }
    }
}
