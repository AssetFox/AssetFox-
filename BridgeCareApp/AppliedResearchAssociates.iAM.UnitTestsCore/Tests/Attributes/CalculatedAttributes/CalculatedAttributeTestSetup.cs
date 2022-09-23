using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes.CalculatedAttributes
{
    public static class CalculatedAttributeTestSetup
    {
        public static void CreateCalculatedAttributeLibrary(UnitOfDataPersistenceWork unitOfWork)
        {
            if (!unitOfWork.Context.CalculatedAttributeLibrary.Any(_ => _.IsDefault))
            {
                var dto = new CalculatedAttributeLibraryDTO
                {
                    IsDefault = true,
                    Id = Guid.NewGuid(),
                    Name = "Default Test Calculated Attribute Library",
                    CalculatedAttributes = { },
                };
                unitOfWork.CalculatedAttributeRepo.UpsertCalculatedAttributeLibrary(dto);
            }
        }
    }
}
