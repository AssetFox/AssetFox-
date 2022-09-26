using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataUnitTests;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Services;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Logging;
using BridgeCareCore.Models;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public class TestHelper
    {
        public static readonly Guid NetworkId = Guid.Parse("7f4ea3ba-6082-4e1e-91a4-b80578aeb0ed");

        public readonly IAMContext DbContext;

        public UnitOfDataPersistenceWork UnitOfWork { get; }

        public TestHelper()
        {
            var config = TestConfiguration.Get();
            var connectionString = TestConnectionStrings.BridgeCare(config);
            DbContext = new IAMContext(new DbContextOptionsBuilder<IAMContext>()
                .UseSqlServer(connectionString)
                .Options);

            UnitOfWork = new UnitOfDataPersistenceWork(config, DbContext);

            DatabaseResetter.ResetDatabase(UnitOfWork);
        }

        private static readonly Lazy<TestHelper> lazy = new Lazy<TestHelper>(new TestHelper());
        public static TestHelper Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private static readonly object HttpContextSetupLock = new object();

        public NetworkEntity TestNetwork { get; } = new NetworkEntity
        {
            Id = NetworkId,
            Name = "Test Network"
        };



        public void CreateSingletons()
        {
            AttributeTestSetup.CreateAttributes(UnitOfWork);
            CreateNetwork();
        }

        private static readonly object NetworkCreationLock = new object();

        public void CreateNetwork()
        {
            if (!UnitOfWork.Context.Network.Any(_ => _.Id == NetworkId))
            {
                lock (NetworkCreationLock)  // Necessary as long as there is a chance that some tests may run in paralell. Can we eliminate that possiblity?
                {
                    if (!UnitOfWork.Context.Network.Any(_ => _.Id == NetworkId))
                    {
                        UnitOfWork.Context.AddEntity(TestNetwork);
                    }
                }
            }
        }
    }
}
