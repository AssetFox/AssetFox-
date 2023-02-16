using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses.PerformanceCurve;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class PerformanceCurveLibraryTestSetup
    {

        public static PerformanceCurveLibraryDTO TestPerformanceCurveLibraryInDb(IUnitOfWork unitOfWork, Guid id)
        {
            var dto = PerformanceCurveLibraryDtos.Empty(id);
            unitOfWork.PerformanceCurveRepo.UpsertPerformanceCurveLibrary(dto);
            return dto;
        }
    }
}
