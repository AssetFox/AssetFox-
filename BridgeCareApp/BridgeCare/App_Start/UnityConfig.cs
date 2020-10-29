using BridgeCare.DataAccessLayer;
using BridgeCare.DataAccessLayer.CriteriaDrivenBudgets;
using BridgeCare.Interfaces;
using BridgeCare.Interfaces.CriteriaDrivenBudgets;
using BridgeCare.Models;
using BridgeCare.Services;
using BridgeCare.Services.SummaryReport;
using System;
using BridgeCare.DataAccessLayer.Inventory;
using BridgeCare.DataAccessLayer.SummaryReport;
using Unity;
using Hangfire;
using Unity.Injection;
using Hangfire.Common;
using Hangfire.Client;
using Hangfire.States;
using BridgeCare.Interfaces.SummaryReport;

namespace BridgeCare
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your type's mappings here.
            // container.RegisterType<IProductRepository, ProductRepository>();
            container.RegisterType<INetworkRepository, NetworkRepository>();
            container.RegisterType<ISimulationRepository, SimulationRepository>();
            container.RegisterType<ISectionsRepository, SectionsRepository>();
            container.RegisterType<ISectionLocatorRepository, SectionLocatorRepository>();
            container.RegisterType<IDetailedReportRepository, DetailedReportRepository>();
            container.RegisterType<IAttributeRepository, AttributesRepository>();
            container.RegisterType<IPerformanceLibraryRepository, PerformanceLibraryRepository>();
            container.RegisterType<BridgeCareContext>();
            container.RegisterType<IBudgetReportRepository, BudgetReportRepository>();
            container.RegisterType<IValidationRepository, ValidationRepository>();
            container.RegisterType<IInventoryRepository, InventoryRepository>();
            container.RegisterType<ISimulationAnalysisRepository, SimulationAnalysisRepository>();
            container.RegisterType<IDeficientReportRepository, DeficientReportRepository>();
            container.RegisterType<IInvestmentLibraryRepository, InvestmentLibraryRepository>();
            container.RegisterType<ITreatmentLibraryRepository, TreatmentLibraryRepository>();
            container.RegisterType<CostDetails>();
            container.RegisterType<ITargetRepository, TargetRepository>();
            container.RegisterType<IReportCreator, ReportCreator>();
            container.RegisterType<FillDetailedSheet>();
            container.RegisterType<TargetsMetRepository>();
            container.RegisterType<TargetResultsRepository>();
            container.RegisterType<Target>();
            container.RegisterType<Deficient>();
            container.RegisterType<Detailed>();
            container.RegisterType<Budget>();
            container.RegisterType<CellAddress>();
            container.RegisterType<IPriorityRepository, PriorityRepository>();
            container.RegisterType<IDeficientRepository, DeficientRepository>();
            container.RegisterType<IRemainingLifeLimitRepository, RemainingLifeLimitRepository>();
            container.RegisterType<IRunRollupRepository, RunRollupRepository>();
            container.RegisterType<ICashFlowLibraryRepository, CashFlowLibraryRepository>();
            container.RegisterType<IUserCriteriaRepository, UserCriteriaRepository>();

            //Summary Report types
            container.RegisterType<ISummaryReportGenerator, SummaryReportGenerator>();
            container.RegisterType<IBridgeDataRepository, BridgeDataRepository>();
            container.RegisterType<SummaryReportBridgeData>();
            container.RegisterType<ICommonSummaryReportDataRepository, CommonSummaryReportDataRepository>();
            container.RegisterType<IBridgeWorkSummaryDataRepository, BridgeWorkSummaryDataRepository>();
            container.RegisterType<IInventoryItemDetailModelGenerator, InventoryItemDetailModelGenerator>();
            container.RegisterType<ICommittedProjects, CommittedProjects>();
            container.RegisterType<ICommittedRepository, CommittedRepository>();
            container.RegisterType<ICriteriaDrivenBudgetsRepository, CriteriaDrivenBudgetsRepository>();
            container.RegisterType<IWorkSummaryByBudgetRepository, WorkSummaryByBudgetRepository>();

            //Hangfire IoC configuration
            container.RegisterType<JobStorage>(new InjectionFactory(c => JobStorage.Current));
            container.RegisterType<IJobFilterProvider, JobFilterAttributeFilterProvider>(new InjectionConstructor(true));
            container.RegisterType<IBackgroundJobFactory, BackgroundJobFactory>();
            container.RegisterType<IRecurringJobManager, RecurringJobManager>();
            container.RegisterType<IBackgroundJobClient, BackgroundJobClient>();
            container.RegisterType<IBackgroundJobStateChanger, BackgroundJobStateChanger>();
        }
    }
}
