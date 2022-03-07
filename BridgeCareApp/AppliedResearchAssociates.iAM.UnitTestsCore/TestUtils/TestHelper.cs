using System;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Hubs;
using BridgeCareCore.Logging;
using BridgeCareCore.Models;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Services;
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
        private static readonly Guid SimulationId = Guid.Parse("416ad546-0796-4889-9db4-9c11bbd6c50d");
        private static readonly Guid CriterionLibraryId = Guid.Parse("47380dd4-8df8-46e2-9195-b7f786a4258a");
        private static readonly Guid UserId = Guid.Parse("1bcee741-02a5-4375-ac61-2323d45752b4");

        public readonly string BaseUrl = "http://localhost:64469/api";

        public readonly IAMContext DbContext;

        public IConfiguration Config { get; }

        public UnitOfDataPersistenceWork UnitOfWork { get; }

        public Mock<IEsecSecurity> MockEsecSecurityAuthorized { get; }
        public Mock<IEsecSecurity> MockEsecSecurityNotAuthorized { get; }

        public ILog Logger { get; }

        public Mock<HubService> MockHubService { get; }

        public Mock<IHubContext<BridgeCareHub>> MockHubContext { get; }

        public Mock<IHttpContextAccessor> MockHttpContextAccessor { get; }


        protected TestHelper()
        {
            Config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testConnections.json")
                .Build();

            MockEsecSecurityAuthorized = new Mock<IEsecSecurity>();
            MockEsecSecurityAuthorized.Setup(_ => _.GetUserInformation(It.IsAny<HttpRequest>()))
                .Returns(new UserInfo
                {
                    Name = "pdsystbamsusr01",
                    Role = "PD-BAMS-Administrator",
                    Email = "pdstseseca5@pa.gov"
                });
            MockEsecSecurityNotAuthorized = new Mock<IEsecSecurity>();
            MockEsecSecurityNotAuthorized.Setup(_ => _.GetUserInformation(It.IsAny<HttpRequest>()))
                .Returns(new UserInfo
                {
                    Name = "b-bamsadmin",
                    Role = "PD-BAMS-DBEngineer",
                    Email = "jmalmberg@ara.com"
                });

            MockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            Logger = new LogNLog();

            MockHubContext = new Mock<IHubContext<BridgeCareHub>>();

            MockHubService = new Mock<HubService>(MockHubContext.Object);

            DbContext = new IAMContext(new DbContextOptionsBuilder<IAMContext>()
                .UseSqlServer(Config.GetConnectionString("BridgeCareConnex"))
                .Options);

            UnitOfWork = new UnitOfDataPersistenceWork(Config, DbContext);

            UnitOfWork.Context.Database.EnsureDeleted();
            UnitOfWork.Context.Database.EnsureCreated();
        }

        private static TestHelper instance = null;
        public static TestHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TestHelper();
                }
                return instance;
            }
        }

        public void SetupDefaultHttpContext()
        {
            var context = new DefaultHttpContext();
            AddAuthorizationHeader(context);
            MockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
        }

        public void AddAuthorizationHeader(DefaultHttpContext context) =>
            context.Request.Headers.Add("Authorization", "Bearer abc123");

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

        public UserEntity TestUser { get; } = new UserEntity
        {
            Id = UserId,
            Username = "pdsystbamsusr02",
            HasInventoryAccess = true
        };

        public virtual void CreateAttributes() =>
            UnitOfWork.AttributeRepo
                .UpsertAttributes(UnitOfWork.AttributeMetaDataRepo.GetAllAttributes().ToList());

        public virtual void CreateNetwork()
        {
            if (!UnitOfWork.Context.Network.Any(_ => _.Id == NetworkId))
            {
                UnitOfWork.Context.AddEntity(TestNetwork);
            }
        }

        public virtual void CreateSimulation()
        {
            if (!UnitOfWork.Context.Simulation.Any(_ => _.Id == SimulationId))
            {
                UnitOfWork.Context.AddEntity(TestSimulation);
            }
        }

        public virtual void CreateCalculatedAttributeLibrary()
        {
            if (!UnitOfWork.Context.CalculatedAttributeLibrary.Any(_ => _.IsDefault))
            {
                _ = UnitOfWork.Context.CalculatedAttributeLibrary.Add(new CalculatedAttributeLibraryEntity
                {
                    IsDefault = true,
                    Id = Guid.NewGuid(),
                    Name = "Default Test Calculated Attribute Library",
                    CalculatedAttributes = { },
                    CreatedDate = DateTime.Now
                });
            }
        }

        public virtual void CleanUp()
        {
            //UnitOfWork.Context.Database.EnsureDeleted();
            //UnitOfWork.Dispose();
        }
    }
}
