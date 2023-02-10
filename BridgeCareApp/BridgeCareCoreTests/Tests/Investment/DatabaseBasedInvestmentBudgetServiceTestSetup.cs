using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Reporting.Logging;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Services;
using BridgeCareCore.Services.DefaultData;

namespace BridgeCareCoreTests.Tests
{
    public static class DatabaseBasedInvestmentBudgetServiceTestSetup
    {
        public static InvestmentBudgetsService SetupDatabaseBasedService(UnitOfDataPersistenceWork unitOfWork)
        {
            AttributeTestSetup.CreateAttributes(unitOfWork);
            NetworkTestSetup.CreateNetwork(unitOfWork);
            var hubService = HubServiceMocks.Default();
            var logger = new LogNLog();
            var service = new InvestmentBudgetsService(
                unitOfWork,
                new ExpressionValidationService(unitOfWork, logger),
                hubService,
                new InvestmentDefaultDataService());
            return service;
        }
    }
}
