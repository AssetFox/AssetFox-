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
    public static class BudgetLibraryTestSetup
    {

        private static CriterionLibraryDTO CreateCriterionLibraryObject(string criteriaExpression = "", bool singleUse = false)
        {
            return new CriterionLibraryDTO()
            {
                MergedCriteriaExpression = criteriaExpression,
                IsSingleUse = singleUse
            };
        }

        private static BudgetDTO CreateBudgetDto(string budgetName)
        {
            //create budget amounts
            var budgetAmountList = new List<BudgetAmountDTO>();
            budgetAmountList.Add(CreateBudgetAmountObject(budgetName, 2010, 2010000));

            //create criterion library
            var criterionLibraryObject = CreateCriterionLibraryObject("0=0", true);

            return new BudgetDTO()
            {
                Id = Guid.NewGuid(),
                Name = budgetName,
                BudgetAmounts = budgetAmountList,
                CriterionLibrary = criterionLibraryObject
            };
        }

        public static BudgetLibraryDTO CreateBudgetLibraryDto(string name)
        {
            //setup
            var budgetList = new List<BudgetDTO>();
            budgetList.Add(CreateBudgetDto("Budget 1"));

            //create budget library
            return new BudgetLibraryDTO()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Budgets = budgetList?.ToList(),
            };
        }

        private static BudgetAmountDTO CreateBudgetAmountObject(string budgetName, int year, decimal value)
        {
            return new BudgetAmountDTO()
            {
                Id = Guid.NewGuid(),
                BudgetName = budgetName,
                Year = year,
                Value = value
            };
        }

        public static BudgetLibraryDTO ModelForEntityInDb(IUnitOfWork unitOfWork, string budgetLibraryName = null)
        {
            var resolveBudgetLibraryName = budgetLibraryName ?? RandomStrings.WithPrefix("BudgetLibrary");
            var dto = CreateBudgetLibraryDto(resolveBudgetLibraryName);
            unitOfWork.BudgetRepo.UpsertBudgetLibrary(dto);
            var dtoAfter = unitOfWork.BudgetRepo.GetBudgetLibrary(dto.Id);
            return dtoAfter;
        }
    }
}
