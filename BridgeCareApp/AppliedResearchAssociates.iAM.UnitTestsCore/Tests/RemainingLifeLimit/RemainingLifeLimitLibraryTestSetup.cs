using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.RemainingLifeLimit
{
    public static class RemainingLifeLimitLibraryTestSetup
    {
        private static RemainingLifeLimitDTO CreateRemainingLifeLimitDto(string remainingLifeLimitName)
        {
            return new RemainingLifeLimitDTO()
            {
                Id = Guid.NewGuid(),
                Value = 0,
                Attribute = ""
            };
        }
        public static RemainingLifeLimitLibraryDTO CreateRemainingLifeLimitLibraryDto(string name)
        {
            //setup
            var remainingLifeLimitList = new List<RemainingLifeLimitDTO>();
            remainingLifeLimitList.Add(CreateRemainingLifeLimitDto("Remaining Life Limit 1"));

            //create RLL library
            return new RemainingLifeLimitLibraryDTO()
            {
                Id = Guid.NewGuid(),
                Name = name,
            };
        }
        public static RemainingLifeLimitLibraryDTO ModelForEntityInDb(IUnitOfWork unitOfWork, string remainingLifeLimitLibraryName = null)
        {
            var resolveRemainingLifeLimitLibraryName = remainingLifeLimitLibraryName ?? RandomStrings.WithPrefix("RemainingLifeLimitLibrary");
            var dto = CreateRemainingLifeLimitLibraryDto(resolveRemainingLifeLimitLibraryName);
            unitOfWork.RemainingLifeLimitRepo.UpsertRemainingLifeLimitLibrary(dto);
            var dtoAfter = unitOfWork.RemainingLifeLimitRepo.GetAllRemainingLifeLimitLibrariesNoChildren().FirstOrDefault(x => x.Id == dto.Id);
            return dtoAfter;
        }
    }
}
