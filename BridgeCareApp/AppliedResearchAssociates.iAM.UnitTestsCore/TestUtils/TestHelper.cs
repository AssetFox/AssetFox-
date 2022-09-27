using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataUnitTests;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public static class TestHelper
    {
        static TestHelper()
        {
            var config = TestConfiguration.Get();
            var connectionString = TestConnectionStrings.BridgeCare(config);
            var options = new DbContextOptionsBuilder<IAMContext>()
                .UseSqlServer(connectionString)
                .Options;
            var dbContext = new IAMContext(options);
            UnitOfWork = new UnitOfDataPersistenceWork(config, dbContext);
            DatabaseResetter.ResetDatabase(UnitOfWork);
        }

        public static IAMContext DbContext => UnitOfWork.Context;

        public static readonly UnitOfDataPersistenceWork UnitOfWork;
    }
}
