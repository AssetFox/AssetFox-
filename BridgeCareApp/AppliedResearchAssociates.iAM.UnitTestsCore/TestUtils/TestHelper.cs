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
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
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
        private static readonly Guid NetworkId = Guid.Parse("7f4ea3ba-6082-4e1e-91a4-b80578aeb0ed");

        public readonly string BaseUrl = "http://localhost:64469/api";

        public readonly IAMContext DbContext;

        public IConfiguration Config { get; }

        public UnitOfDataPersistenceWork UnitOfWork { get; }

        public Mock<IEsecSecurity> MockEsecSecurityAdmin { get; }
        public Mock<IEsecSecurity> MockEsecSecurityDBE { get; }
        public Mock<ITreatmentService> MockTreatmentService { get; }
        public ILog Logger { get; }

        public Mock<HubService> MockHubService { get; }

        public Mock<IHubContext<BridgeCareHub>> MockHubContext { get; }

        public Mock<IHttpContextAccessor> MockHttpContextAccessor { get; }


        public TestHelper()
        {
            Config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testConnections.json")
                .Build();
            MockTreatmentService = new Mock<ITreatmentService>();
            MockEsecSecurityAdmin = new Mock<IEsecSecurity>();
            MockEsecSecurityAdmin.Setup(_ => _.GetUserInformation(It.IsAny<HttpRequest>()))
                .Returns(new UserInfo
                {
                    Name = "pdsystbamsusr01",
                    HasAdminAccess = true,
                    Email = "pdstseseca5@pa.gov"
                });
            MockEsecSecurityDBE = new Mock<IEsecSecurity>();
            MockEsecSecurityDBE.Setup(_ => _.GetUserInformation(It.IsAny<HttpRequest>()))
                .Returns(new UserInfo
                {
                    Name = "b-bamsadmin",
                    HasAdminAccess = false,
                    Email = "jmalmberg@ara.com"
                });

            MockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            Logger = new LogNLog();

            MockHubContext = new Mock<IHubContext<BridgeCareHub>>();

            MockHubService = new Mock<HubService>(MockHubContext.Object);
            var connectionString = TestConnectionStrings.BridgeCare(Config);
            DbContext = new IAMContext(new DbContextOptionsBuilder<IAMContext>()
                .UseSqlServer(connectionString)
                .Options);

            UnitOfWork = new UnitOfDataPersistenceWork(Config, DbContext);

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
        private static bool HttpContextHasBeenSetup = false;

        public void SetupDefaultHttpContext()
        {
            if (!HttpContextHasBeenSetup)
            {
                lock (HttpContextSetupLock) // Necessary as long as there is a chance that some tests may run in paralell. Can we eliminate that possiblity?
                {
                    if (!HttpContextHasBeenSetup)
                    {
                        var context = new DefaultHttpContext();
                        AddAuthorizationHeader(context);
                        MockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
                    }
                }
            }
        }

        public void AddAuthorizationHeader(DefaultHttpContext context) =>
            context.Request.Headers.Add("Authorization", "Bearer abc123");

        public NetworkEntity TestNetwork { get; } = new NetworkEntity
        {
            Id = NetworkId,
            Name = "Test Network"
        };

        public SimulationEntity TestSimulation(Guid? id = null, string name = null, Guid? owner = null)
        {
            var resolveName = name ?? RandomStrings.Length11();
            var resolveId = id ?? Guid.NewGuid();
            var users = new List<SimulationUserEntity>();
            var returnValue = new SimulationEntity
            {
                Id = resolveId,
                NetworkId = NetworkId,
                Name = resolveName,
                NumberOfYearsOfTreatmentOutlook = 2,
                SimulationUserJoins = users
            };
            if (owner != null)
                users.Add(new SimulationUserEntity() { IsOwner = true, UserId = owner.Value, SimulationId = resolveId });
            return returnValue;
        }


        private static bool AttributesHaveBeenCreated = false;
        private static readonly object AttributeLock = new object();

        public void CreateAttributes()
        {
            if (!AttributesHaveBeenCreated)
            {
                lock (AttributeLock)  // Necessary as long as there is a chance that some tests may run in paralell. Can we eliminate that possiblity?
                {
                    if (!AttributesHaveBeenCreated)
                    {
                        SQLDataSourceDTO dataSourceToApply = null;
                        if (!UnitOfWork.DataSourceRepo.GetDataSources().Any(_ => _.Type == "SQL"))
                        {
                            dataSourceToApply = new SQLDataSourceDTO
                            {
                                Id = Guid.NewGuid(),
                                Name = "Test SQL DataSource",
                                ConnectionString = Config.GetConnectionString("BridgeCareConnex")
                            };
                            UnitOfWork.DataSourceRepo.UpsertDatasource(dataSourceToApply);
                        }
                        else
                        {
                            dataSourceToApply = (SQLDataSourceDTO)UnitOfWork.DataSourceRepo.GetDataSources().First(_ => _.Type == "SQL");
                        }
                        var attributesToInsert = AttributeDtoLists.AttributeSetupDtos();
                        foreach (var attribute in attributesToInsert)
                        {
                            attribute.DataSource = dataSourceToApply;
                        }
                        UnitOfWork.AttributeRepo.UpsertAttributes(attributesToInsert);
                        AttributesHaveBeenCreated = true;
                    }
                }
            }
        }

        public virtual void CreateSingletons()
        {
            CreateAttributes();
            CreateNetwork();
            SetupDefaultHttpContext();
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

        public virtual SimulationEntity CreateSimulation(Guid? id = null, string name = null, Guid? owner = null)
        {
            var entity = TestSimulation(id, name, owner);
            UnitOfWork.Context.AddEntity(entity);
            return entity;
        }

        public virtual void CreateCalculatedAttributeLibrary()
        {
            if (!UnitOfWork.Context.CalculatedAttributeLibrary.Any(_ => _.IsDefault))
            {
                var dto = new CalculatedAttributeLibraryDTO
                {
                    IsDefault = true,
                    Id = Guid.NewGuid(),
                    Name = "Default Test Calculated Attribute Library",
                    CalculatedAttributes = { },
                };
                UnitOfWork.CalculatedAttributeRepo.UpsertCalculatedAttributeLibrary(dto);
            }
        }
    }
}
