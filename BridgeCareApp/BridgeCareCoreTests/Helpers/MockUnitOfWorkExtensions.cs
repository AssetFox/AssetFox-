using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Moq;

namespace BridgeCareCoreTests.Helpers
{
    public static class MockUnitOfWorkExtensions
    {
        public static void SetupBudgetRepo(this Mock<IUnitOfWork> mockUnitOfWork, IBudgetRepository budgetRepository)
        {
            mockUnitOfWork.Setup(u => u.BudgetRepo).Returns(budgetRepository);
        }

        public static void SetupBudgetRepo(this Mock<IUnitOfWork> mockUnitOfWork, Mock<IBudgetRepository> mockBudgetRepository)
        {
            mockUnitOfWork.SetupBudgetRepo(mockBudgetRepository.Object);
        }

        public static void SetupInvestmentPlanRepo(this Mock<IUnitOfWork> mockunitOfWork, Mock<IInvestmentPlanRepository> mockInvestmentPlanRepository)
        {
            mockunitOfWork.Setup(u => u.InvestmentPlanRepo).Returns(mockInvestmentPlanRepository.Object);
        }

        public static void SetupTreatmentRepo(this Mock<IUnitOfWork> mockUnitOfWork, Mock<ITreatmentLibraryUserRepository> mockTreatmentRepository)
        {
            mockUnitOfWork.Setup(u => u.TreatmentLibraryUserRepo).Returns(mockTreatmentRepository.Object);
        }
    }
}
