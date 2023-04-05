using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.BudgetPriority
{
    public static class BudgetPriorityLibraryTestSetup
    {
        private static BudgetPriorityDTO CreateBudgetPriorityDto()
        {
            return new BudgetPriorityDTO()
            {
                Id = Guid.NewGuid(),
                PriorityLevel= 1,
                Year = 2023,
            };
        }
        public static BudgetPriorityLibraryDTO CreateBudgetPriorityLibraryDto(string name)
        {
            //setup
            var dto = BudgetPriorityDtos.New(null, 1, 2023);
            var budgetPriorityList = new List<BudgetPriorityDTO> { dto };

            //create budget priority library
            return new BudgetPriorityLibraryDTO()
            {
                Id = Guid.NewGuid(),
                Name = name,
                BudgetPriorities = budgetPriorityList?.ToList(),
            };
        }
        public static BudgetPriorityLibraryDTO ModelForEntityInDb(IUnitOfWork unitOfWork, string budgetPriorityLibraryName = null)
        {
            var resolveBudgetPriorityLibraryName = budgetPriorityLibraryName ?? RandomStrings.WithPrefix("BudgetPriorityLibrary");
            var dto = CreateBudgetPriorityLibraryDto(resolveBudgetPriorityLibraryName);
            unitOfWork.BudgetPriorityRepo.UpsertBudgetPriorityLibrary(dto);
            var dtoAfter = unitOfWork.BudgetPriorityRepo.GetBudgetPriorityLibraries().FirstOrDefault(x => x.Id == dto.Id);
            return dtoAfter;
        }
    }
}
