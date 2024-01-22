using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;

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

        private static PerformanceCurveDTO CreatePerformanceCurveDto(string performanceCurveName)
        {
            return new PerformanceCurveDTO()
            {
                Id = Guid.NewGuid(),
                Name = performanceCurveName,
                Attribute = ""
            };
        }
        public static PerformanceCurveLibraryDTO CreatePerformanceCurveLibraryDto(string name)
        {
            //setup
            var performanceCurveList = new List<PerformanceCurveDTO>();
            performanceCurveList.Add(CreatePerformanceCurveDto("Performance Curve 1"));

            //create budget library
            return new PerformanceCurveLibraryDTO()
            {
                Id = Guid.NewGuid(),
                Name = name,
                PerformanceCurves = performanceCurveList?.ToList()
            };
        }
        public static PerformanceCurveLibraryDTO ModelForEntityInDb(IUnitOfWork unitOfWork, string performanceCurveLibraryName = null)
        {
            var resolvePerformanceCurveLibraryName = performanceCurveLibraryName ?? RandomStrings.WithPrefix("PerformanceCurveLibrary");
            var dto = CreatePerformanceCurveLibraryDto(resolvePerformanceCurveLibraryName);
            unitOfWork.PerformanceCurveRepo.UpsertPerformanceCurveLibrary(dto);
            var dtoAfter = unitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(dto.Id);
            return dtoAfter;
        }
    }
}
