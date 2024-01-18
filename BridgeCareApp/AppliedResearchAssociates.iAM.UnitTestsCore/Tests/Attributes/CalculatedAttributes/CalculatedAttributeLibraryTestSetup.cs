using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes.CalculatedAttributes
{
    public static class CalculatedAttributeLibraryTestSetup
    {
        public static CalculatedAttributeLibraryDTO ModelForEntityInDb(IUnitOfWork unitOfWork)
        {
            var dto = CalculatedAttributeLibraryDtos.Dto();
            unitOfWork.CalculatedAttributeRepo.UpsertCalculatedAttributeLibrary(dto);
            var dtoAfter = unitOfWork.CalculatedAttributeRepo.GetCalculatedAttributeLibraries().FirstOrDefault(x => x.Id == dto.Id);
            return dtoAfter;
        }
    }
}
