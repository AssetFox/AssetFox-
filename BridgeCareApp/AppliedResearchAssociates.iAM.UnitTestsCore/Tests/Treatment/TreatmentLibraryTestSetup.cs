using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Treatment
{
    public static class TreatmentLibraryTestSetup
    {
        private static TreatmentDTO CreateTreatmentDto(string treatmentName)
        {
            return new TreatmentDTO()
            {
                Id = Guid.NewGuid(),
                Name = treatmentName,
            };
        }
        public static TreatmentLibraryDTO CreateTreatmentLibraryDto(string name)
        {
            //setup
            var treatmentList = new List<TreatmentDTO>();
            treatmentList.Add(CreateTreatmentDto("Treatment 1"));

            //create treatment library
            return new TreatmentLibraryDTO()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Treatments = treatmentList?.ToList(),
            };
        }
        public static TreatmentLibraryDTO ModelForEntityInDb(IUnitOfWork unitOfWork, string treatmentLibraryName = null)
        {
            var resolveTreatmentLibraryName = treatmentLibraryName ?? RandomStrings.WithPrefix("TreatmentLibrary");
            var dto = CreateTreatmentLibraryDto(resolveTreatmentLibraryName);
            unitOfWork.SelectableTreatmentRepo.UpsertTreatmentLibrary(dto);
            var dtoAfter = unitOfWork.SelectableTreatmentRepo.GetSingleTreatmentLibary(dto.Id);
            return dtoAfter;
        }
    }
}
