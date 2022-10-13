using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class BudgetTestSetup
    {

        private static CriterionLibraryDTO CreateCriterionLibraryObject(string criteriaExpression = "", bool singleUse = false)
        {
            return new CriterionLibraryDTO()
            {
                MergedCriteriaExpression = criteriaExpression,
                IsSingleUse = singleUse
            };
        }

        private static BudgetDTO CreateBudgetObject(string budgetName)
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

        public static BudgetLibraryDTO CreateBudgetLibraryDto(string name, bool isShared = true)
        {
            //setup
            var budgetList = new List<BudgetDTO>();
            budgetList.Add(CreateBudgetObject("Budget 1"));

            //create budget library
            return new BudgetLibraryDTO()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Budgets = budgetList?.ToList(),
                IsShared = isShared
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

        public static BudgetLibraryDTO ModelForEntityInDb(IUnitOfWork unitOfWork, string budgetName, bool isShared)
        {
            var dto = CreateBudgetLibraryDto(budgetName, isShared);
            unitOfWork.BudgetRepo.UpsertBudgetLibrary(dto);
            return dto;
        }
    }
}
