using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repsitories;
using Moq;

namespace BridgeCareCoreTests.Tests
{
    public static class BudgetRepositoryMocks
    {
        public static Mock<IBudgetRepository> New()
        {
            var mock = new Mock<IBudgetRepository>();
            return mock;
        }

        public static Mock<IBudgetRepository> WithLibraryAccess(Guid libraryId, LibraryAccessModel libraryAccess)
        {
            var mock = New();
            mock.Setup(m => m.GetLibraryAccess(libraryId)).Returns(libraryAccess);
            return mock;
        }
    }
}
