using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataUnitTests;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Reporting.Logging;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Services;
using BridgeCareCore.StartupExtension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public static class TestServices
    {
        public static IServiceCollection AddBridgecareCoreServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSimulationData();
            serviceCollection.AddDefaultData();
            return serviceCollection;
        }

        private static IServiceProvider CreateServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            var testHelper = TestHelper.Instance;
            var config = TestConfiguration.Get();
            serviceCollection.AddSingleton(config);
            serviceCollection.AddSingleton(testHelper.DbContext);
            var hubService = HubServiceMocks.Default();
            serviceCollection.AddSingleton(hubService);
            serviceCollection.AddSingleton<IHubService>(hubService);
            var log = new LogNLog();
            serviceCollection.AddSingleton(log);
            serviceCollection.AddBridgecareCoreServices();
            var provider = serviceCollection.BuildServiceProvider();
            return provider;
        }

        public static Lazy<IServiceProvider> LazyServiceProvider
            => new Lazy<IServiceProvider>(() => CreateServiceProvider());

        public static IServiceProvider ServiceProvider
            => LazyServiceProvider.Value;

        public static T GetService<T>()
        {
            var provider = ServiceProvider;
            var service = provider.GetService<T>();
            return service;
        }

        public static ILog Logger()
        {
            var logger = new LogNLog();
            return logger;
        }

        public static ExpressionValidationService ExpressionValidation(UnitOfDataPersistenceWork unitOfWork)
        {
            var log = Logger();
            var service = new ExpressionValidationService(unitOfWork, log);
            return service;
        }

        public static IPerformanceCurvesService PerformanceCurves(UnitOfDataPersistenceWork unitOfWork, IHubService hubService)
        {
            var logger = new LogNLog();
            var expressionValidation = ExpressionValidation(unitOfWork);
            var service = new PerformanceCurvesService(unitOfWork, hubService, expressionValidation);
            return service;
        }
    }
}
