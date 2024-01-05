using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;

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

        public static CalculatedAttributeLibraryDTO TestCalculatedAttributeLibraryInDb(IUnitOfWork unitOfWork, Guid libraryId)
        {
            var calcAttrLibrary = CalculatedAttributeLibraryDtos.Dto(libraryId);
            unitOfWork.CalculatedAttributeRepo.UpsertCalculatedAttributeLibrary(calcAttrLibrary);
            var calcAttrsFromDb = unitOfWork.CalculatedAttributeRepo.GetCalculatedAttributeLibraryByID(libraryId);
            return calcAttrsFromDb;
        }

        public static CalculatedAttributeDTO TestCalculatedAttributeDto(Guid calcAttrId, string attributeName)
        {
            var pair = new CalculatedAttributeEquationCriteriaPairDTO()
            {
                Id = Guid.NewGuid(),
                Equation = new EquationDTO()
                {
                    Id = Guid.NewGuid(),
                    Expression = $"[{attributeName}]"
                }
            };
            var calcAttr = new CalculatedAttributeDTO
            {
                Id = calcAttrId,
                Attribute = attributeName,
                CalculationTiming = 0,
                Equations = new List<CalculatedAttributeEquationCriteriaPairDTO>() { pair }
            };
            return calcAttr;
        }

        public static CalculatedAttributeDTO TestCalculatedAttributeDtoWithEquationCriterionLibrary(Guid calcAttrId, string attributeName, string mergeCriterionExpression = "mergeCriteriaExpression")
        {
            var criterionLibrary = CriterionLibraryDtos.Dto(null, mergeCriterionExpression);
            var pair = new CalculatedAttributeEquationCriteriaPairDTO()
            {
                Id = Guid.NewGuid(),
                Equation = new EquationDTO()
                {
                    Id = Guid.NewGuid(),
                    Expression = $"[{attributeName}]"
                },
                CriteriaLibrary = criterionLibrary
            };
            var calcAttr = new CalculatedAttributeDTO
            {
                Id = calcAttrId,
                Attribute = attributeName,
                CalculationTiming = 0,
                Equations = new List<CalculatedAttributeEquationCriteriaPairDTO>() { pair }
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
