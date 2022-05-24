﻿using System;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
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
        private static readonly Guid UserId = Guid.Parse("1bcee741-02a5-4375-ac61-2323d45752b4");

        public readonly string BaseUrl = "http://localhost:64469/api";

        public readonly IAMContext DbContext;

        public IConfiguration Config { get; }

        public UnitOfDataPersistenceWork UnitOfWork { get; }

        public Mock<IEsecSecurity> MockEsecSecurityAuthorized { get; }
        public Mock<IEsecSecurity> MockEsecSecurityNotAuthorized { get; }
        public Mock<ITreatmentService> MockTreatmentService { get; }
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
            MockTreatmentService = new Mock<ITreatmentService>();
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
            var connectionString = Config.GetConnectionString("BridgeCareConnex");
            DbContext = new IAMContext(new DbContextOptionsBuilder<IAMContext>()
                .UseSqlServer(connectionString)
                .Options);

            UnitOfWork = new UnitOfDataPersistenceWork(Config, DbContext);

            UnitOfWork.Context.Database.EnsureDeleted();
            UnitOfWork.Context.Database.EnsureCreated();
        }
                       
        private static readonly Lazy<TestHelper> lazy = new Lazy<TestHelper>(new TestHelper());
        public static TestHelper Instance
        {
            get
            {
                return lazy.Value;
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

        public SimulationEntity TestSimulation(Guid? id = null, string name = null)
        {
            var resolveName = name ?? RandomStrings.Length11();
            var resolveId = id ?? Guid.NewGuid();
            var returnValue = new SimulationEntity
            {
                Id = resolveId,
                NetworkId = NetworkId,
                Name = resolveName,
                NumberOfYearsOfTreatmentOutlook = 2
            };
            return returnValue;
        }

        public CriterionLibraryEntity TestCriterionLibrary(Guid? id = null, string? name = null)
        {
            var resolvedId = id ?? Guid.NewGuid();
            var resolvedName = name ?? "Test Criterion " + RandomStrings.Length11();
            var returnValue = new CriterionLibraryEntity
            {
                Id = resolvedId,
                Name = resolvedName,
                MergedCriteriaExpression = "Test Expression"
            };
            return returnValue;
        }

        public UserEntity TestUser { get; } = new UserEntity
        {
            Id = UserId,
            Username = "pdsystbamsusr02",
            HasInventoryAccess = true
        };

        public virtual void CreateAttributes()
        {
            var attributesToInsert = AttributeDtoLists.AttributeSetupDtos();
            UnitOfWork.AttributeRepo.UpsertAttributes(attributesToInsert);
        }

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
    }
}
