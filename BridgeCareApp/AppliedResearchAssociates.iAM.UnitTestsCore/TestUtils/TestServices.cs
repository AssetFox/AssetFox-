using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
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
            serviceCollection.AddSingleton(testHelper.Config);
            serviceCollection.AddSingleton(testHelper.DbContext);
            var hubService = HubServiceMocks.Default();
            serviceCollection.AddSingleton(hubService);
            serviceCollection.AddSingleton<IHubService>(hubService);
            var log = testHelper.Logger;
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

        public static IPerformanceCurvesService PerformanceCurves => GetService<IPerformanceCurvesService>();
    }
}
