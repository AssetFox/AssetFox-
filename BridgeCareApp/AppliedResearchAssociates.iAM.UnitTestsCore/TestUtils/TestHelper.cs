using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataUnitTests;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public class TestHelper
    {
        private readonly IAMContext IamContext;

        static TestHelper()
        {
            lazy = new Lazy<TestHelper>(new TestHelper());
        }

        private UnitOfDataPersistenceWork UnitOfDataPersistenceWork { get; }

        private TestHelper()
        {
            var config = TestConfiguration.Get();
            var connectionString = TestConnectionStrings.BridgeCare(config);
            IamContext = new IAMContext(new DbContextOptionsBuilder<IAMContext>()
                .UseSqlServer(connectionString)
                .Options);

            UnitOfDataPersistenceWork = new UnitOfDataPersistenceWork(config, IamContext);

            DatabaseResetter.ResetDatabase(UnitOfDataPersistenceWork);
        }

        private static readonly Lazy<TestHelper> lazy;
        private static TestHelper Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public static IAMContext DbContext
        {
            get
            {
                var dbContext = Instance.IamContext;
                return dbContext;
            }
        }

        public static UnitOfDataPersistenceWork UnitOfWork
        {
            get
            {
                var unitOfWork = Instance.UnitOfDataPersistenceWork;
                return unitOfWork;
            }
        }
    }
}
