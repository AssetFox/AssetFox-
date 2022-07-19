using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AppliedResearchAssociates.iAM.DataMinerUnitTests
{
    public static class UnitOfWorkSetup
    {
        public static UnitOfDataPersistenceWork New(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("BridgeCareConnex");
            var options = new DbContextOptionsBuilder<IAMContext>()
                .UseSqlServer(connectionString)
                .Options;
            var dbContext = new IAMContext(options);
            var unitOfWork = new UnitOfDataPersistenceWork(configuration, dbContext);
            return unitOfWork;
        }
    }
}
