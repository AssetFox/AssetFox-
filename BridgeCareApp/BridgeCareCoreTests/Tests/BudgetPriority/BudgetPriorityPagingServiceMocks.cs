using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using Moq;

namespace BridgeCareCoreTests.Tests.BudgetPriority
{
    public static class BudgetPriorityPagingServiceMocks
    {
        public static Mock<IBudgetPriortyPagingService> DefaultMock()
            => new Mock<IBudgetPriortyPagingService>();
        public static void SetupGetSyncedLibraryDataset(this Mock<IBudgetPriortyPagingService> mock, LibraryUpsertPagingRequestModel<BudgetPriorityLibraryDTO, BudgetPriorityDTO> upsertRequest)
        {
            mock.Setup(r => r.GetSyncedLibraryDataset(upsertRequest)).Returns(new List<BudgetPriorityDTO>());
        }
    }
}
