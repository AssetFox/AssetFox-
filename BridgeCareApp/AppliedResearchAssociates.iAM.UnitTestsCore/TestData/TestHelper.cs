﻿using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.UnitTestsCore.Mocks;
using BridgeCareCore;
using BridgeCareCore.Hubs;
using BridgeCareCore.Logging;
using BridgeCareCore.Models;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Engine;
using Moq;
using Moq.Contrib.HttpClient;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestData
{
    public class TestHelper
    {
        private static readonly Guid NetworkId = Guid.Parse("7f4ea3ba-6082-4e1e-91a4-b80578aeb0ed");
        private static readonly Guid SimulationId = Guid.Parse("416ad546-0796-4889-9db4-9c11bbd6c50d");
        private static readonly Guid CriterionLibraryId = Guid.Parse("47380dd4-8df8-46e2-9195-b7f786a4258a");

        public readonly string BaseUrl = "http://localhost:64469/api";

        public readonly IAMContext DbContext;

        public IConfiguration Config { get; }

        public UnitOfDataPersistenceWork UnitOfDataPersistenceWork { get; }

        public MockEsecSecurity MockEsecSecurity { get; }

        public ILog Logger { get; }

        public Mock<IHubContext<BridgeCareHub>> MockHubContext { get; set; }

        public TestHelper(string dbName = "IAMv2")
        {
            Config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testConnections.json")
                .AddJsonFile("testEsec.json")
                .Build();

            MockEsecSecurity = new MockEsecSecurity();

            Logger = new LogNLog();

            MockHubContext = new Mock<IHubContext<BridgeCareHub>>();

            DbContext = new IAMContext(new DbContextOptionsBuilder<IAMContext>()
                .UseInMemoryDatabase(dbName)
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

            UnitOfDataPersistenceWork = new UnitOfDataPersistenceWork(Config, DbContext);
        }

        public NetworkEntity TestNetwork { get; } = new NetworkEntity
        {
            Id = NetworkId,
            Name = "Test Network"
        };

        public SimulationEntity TestSimulation { get; } = new SimulationEntity
        {
            Id = SimulationId,
            NetworkId = NetworkId,
            Name = "Test Simulation",
            NumberOfYearsOfTreatmentOutlook = 2
        };

        public CriterionLibraryEntity TestCriterionLibrary { get; } = new CriterionLibraryEntity
        {
            Id = CriterionLibraryId,
            Name = "Test Criterion",
            MergedCriteriaExpression = "Test Expression"
        };

        public virtual void CreateAttributes()
        {
            if (!UnitOfDataPersistenceWork.Context.Attribute.Any())
            {
                UnitOfDataPersistenceWork.AttributeRepo
                    .UpsertAttributes(UnitOfDataPersistenceWork.AttributeMetaDataRepo.GetAllAttributes().ToList());
            }
        }

        public virtual void CreateNetwork()
        {
            if (!UnitOfDataPersistenceWork.Context.Network.Any(_ => _.Id == NetworkId))
            {
                UnitOfDataPersistenceWork.Context.Network.Add(TestNetwork);
                UnitOfDataPersistenceWork.Context.SaveChanges();
            }
        }

        public virtual void CreateSimulation()
        {
            if (!UnitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == SimulationId))
            {
                UnitOfDataPersistenceWork.Context.Simulation.Add(TestSimulation);
                UnitOfDataPersistenceWork.Context.SaveChanges();
            }
        }

        public virtual void CleanUp()
        {
            DbContext.Database.EnsureDeleted();
            UnitOfDataPersistenceWork.Dispose();
        }
    }
}
