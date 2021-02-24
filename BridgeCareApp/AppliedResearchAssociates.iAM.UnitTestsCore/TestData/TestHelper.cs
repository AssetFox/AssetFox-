using System;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestData
{
    public abstract class TestHelper
    {
        public readonly IAMContext DbContext;
        public IConfiguration Config { get; set; }
        public UnitOfDataPersistenceWork UnitOfDataPersistenceWork { get; set; }

        protected TestHelper()
        {
            Config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testConnections.json")
                .Build();

            DbContext = new IAMContext(new DbContextOptionsBuilder<IAMContext>()
                .UseInMemoryDatabase(databaseName: "IAMv2")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);
            UnitOfDataPersistenceWork = new UnitOfDataPersistenceWork(Config, DbContext);

            CreateAttributes();
        }

        public NetworkEntity NetworkEntity { get; } = new NetworkEntity
        {
            Id = Guid.Parse("7f4ea3ba-6082-4e1e-91a4-b80578aeb0ed"),
            Name = "Test Network"
        };

        public SimulationEntity SimulationEntity { get; } = new SimulationEntity
        {
            Id = Guid.Parse("416ad546-0796-4889-9db4-9c11bbd6c50d"),
            NetworkId = Guid.Parse("7f4ea3ba-6082-4e1e-91a4-b80578aeb0ed"),
            Name = "Test Simulation",
            NumberOfYearsOfTreatmentOutlook = 2
        };

        public virtual void CreateAttributes() =>
            UnitOfDataPersistenceWork.AttributeRepo
                .UpsertAttributes(UnitOfDataPersistenceWork.AttributeMetaDataRepo.GetAllAttributes().ToList());

        public virtual void CreateNetwork()
        {
            UnitOfDataPersistenceWork.Context.Network.Add(NetworkEntity);
            UnitOfDataPersistenceWork.Context.SaveChanges();
        }

        public virtual void CreateSimulation()
        {
            UnitOfDataPersistenceWork.Context.Simulation.Add(SimulationEntity);
            UnitOfDataPersistenceWork.Context.SaveChanges();
        }

        public virtual void CleanUp()
        {
            DbContext.Database.EnsureDeleted();
            UnitOfDataPersistenceWork.Dispose();
        }
    }
}
