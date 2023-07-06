using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using Moq;

namespace BridgeCareCoreTests.Tests.BudgetPriority
{
    public static class BudgetPriorityRepositoryMocks
    {
        public static Mock<IBudgetPriorityRepository> DefaultMock(Mock<IUnitOfWork> mockUnitOfWork = null)
        {
            var mockBudgetPriorityRepository = new Mock<IBudgetPriorityRepository>();
            if (mockUnitOfWork != null)
            {
                mockUnitOfWork.Setup(u => u.BudgetPriorityRepo).Returns(mockBudgetPriorityRepository.Object);
            }
            return mockBudgetPriorityRepository;
        }
        public static void SetupGetLibraryAccess(this Mock<IBudgetPriorityRepository> mock, Guid libraryId, LibraryUserAccessModel accessModel)
        {
            mock.Setup(r => r.GetLibraryAccess(libraryId, It.IsAny<Guid>())).Returns(accessModel);
        }
    }
}
