using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using Microsoft.Extensions.DependencyModel;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes.CalculatedAttributes
{
    public static class CalculatedAttributeTestSetup
    {
        private static object DefaultCalculatedAttributeSetupLock = new object();
        private static bool DefaultCalculatedAttributeLibraryHasBeenCreated = false;

        public static void CreateDefaultCalculatedAttributeLibrary(UnitOfDataPersistenceWork unitOfWork)
        {
            if (!DefaultCalculatedAttributeLibraryHasBeenCreated)
            {
                lock (DefaultCalculatedAttributeSetupLock)
                {
                    if (!DefaultCalculatedAttributeLibraryHasBeenCreated)
                    {
                        var dto = new CalculatedAttributeLibraryDTO
                        {
                            IsDefault = true,
                            Id = Guid.NewGuid(),
                            Name = "Default Test Calculated Attribute Library",
                            CalculatedAttributes = { },
                        };
                        unitOfWork.CalculatedAttributeRepo.UpsertCalculatedAttributeLibrary(dto);
                        DefaultCalculatedAttributeLibraryHasBeenCreated = true;
                    }
                }
            }
        }
        public static CalculatedAttributeLibraryDTO TestCalculatedAttributeLibraryDto(Guid libraryId)
        {
            var randomName = RandomStrings.WithPrefix("CalcAttr");
            var calcAttr = new CalculatedAttributeLibraryDTO
            {
                Id = libraryId,
                Name = randomName,
                Description = "test library"
            };
            return calcAttr;
        }

        public static CalculatedAttributeLibraryDTO TestCalculatedAttributeLibraryInDb(IUnitOfWork unitOfWork, Guid libraryId)
        {
            var calcAttrLibrary = TestCalculatedAttributeLibraryDto(libraryId);
            unitOfWork.CalculatedAttributeRepo.UpsertCalculatedAttributeLibrary(calcAttrLibrary);
            var calcAttrsFromDb = unitOfWork.CalculatedAttributeRepo.GetCalculatedAttributeLibraryByID(libraryId);
            return calcAttrsFromDb;
        }

        public static CalculatedAttributeDTO TestCalculatedAttributeDto(Guid calcAttrId, string attributeName)
        {
            var calcAttr = new CalculatedAttributeDTO
            {
                Id = calcAttrId,
                Attribute = attributeName,
                CalculationTiming = 0,
                Equations = new List<CalculatedAttributeEquationCriteriaPairDTO>() { new CalculatedAttributeEquationCriteriaPairDTO()
                {
                    Id = Guid.NewGuid(),
                    Equation = new EquationDTO()
                    {
                        Id = Guid.NewGuid(),
                        Expression = $"[{attributeName}]"
                    }
                } }
            };
            return calcAttr;
        }

        public static CalculatedAttributeDTO TestCalculatedAttributeInLibraryInDb(IUnitOfWork unitOfWork, CalculatedAttributeLibraryDTO library, Guid calcAttrId, string attributeName)
        {
            var calcAttr = TestCalculatedAttributeDto(calcAttrId, attributeName);
            library.CalculatedAttributes.Add(calcAttr);
            var calcAttrs = new List<CalculatedAttributeDTO> { calcAttr };
            unitOfWork.CalculatedAttributeRepo.UpsertCalculatedAttributeLibrary(library);
            var calcAttrsFromDb = unitOfWork.CalculatedAttributeRepo.GetCalculatedAttributeLibraryByID(library.Id).CalculatedAttributes;
            var calcAttrToReturn = calcAttrsFromDb.Single(c => c.Id == calcAttrId);
            return calcAttrToReturn;
        }

        public static CalculatedAttributeDTO TestCalculatedAttributeInScenarioInDb(IUnitOfWork unitOfWork, Guid scenarioId, Guid calcAttrId, string attributeName)
        {
            var calcAttr = TestCalculatedAttributeDto(calcAttrId, attributeName);
            var calcAttrs = new List<CalculatedAttributeDTO> { calcAttr };
            unitOfWork.CalculatedAttributeRepo.UpsertScenarioCalculatedAttributesNonAtomic(calcAttrs, scenarioId);
            var calcAttrsFromDb = unitOfWork.CalculatedAttributeRepo.GetScenarioCalculatedAttributes(scenarioId);
            var calcAttrToReturn = calcAttrsFromDb.Single(c => c.Id == calcAttrId);
            return calcAttrToReturn;
        }
    }
}
