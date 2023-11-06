using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.SelectableTreatment;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class TreatmentLibraryTestSetup
    {
        public static TreatmentLibraryDTO ModelForEntityInDb(IUnitOfWork unitOfWork, Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var dto = TreatmentLibraryDtos.Empty(resolveId);
            unitOfWork.SelectableTreatmentRepo.UpsertTreatmentLibrary(dto);
            return dto;
        }
    }
}
