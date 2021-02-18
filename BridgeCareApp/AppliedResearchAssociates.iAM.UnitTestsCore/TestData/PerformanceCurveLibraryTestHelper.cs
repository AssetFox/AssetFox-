using System.IO;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestData
{
    public class PerformanceCurveLibraryTestHelper
    {
        private readonly IAMContext _dbContext;
        private readonly IConfiguration _config;

        public UnitOfDataPersistenceWork UnitOfDataPersistenceWork { get; set; }

        public PerformanceCurveLibraryTestHelper()
        {
            _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testConnections.json")
                .Build();
            _dbContext = new IAMContext(new DbContextOptionsBuilder<IAMContext>()
                .UseInMemoryDatabase(databaseName: "IAMv2")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);
            UnitOfDataPersistenceWork = new UnitOfDataPersistenceWork(_config, _dbContext);
        }
    }
}
