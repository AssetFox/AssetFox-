using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataAccess;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;
using BridgeCareCore.Hubs;
using BridgeCareCore.Services.LegacySimulationSynchronization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MoreLinq;
using Attribute = AppliedResearchAssociates.iAM.Domains.Attribute;
using DA = AppliedResearchAssociates.iAM.DataAssignment;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestData
{
    public class SimulationAnalysisDataPersistenceTestHelper
    {
        private const int NetworkId = 13;
        private const int SimulationId = 1171;

        private readonly SqlConnection _sqlConnection;
        private readonly DataAccessor _dataAccessor;
        private readonly IAMContext _dbContext;

        public IConfiguration Config { get; set; }
        /*public IAttributeMetaDataRepository AttributeMetaDataRepo { get; set; }
        public INetworkRepository NetworkRepo { get; set; }
        public IAttributeValueHistoryRepository AttributeValueHistoryRepo { get; set; }
        public ISectionRepository SectionRepo { get; set; }
        public IFacilityRepository FacilityRepo { get; set; }
        public ISimulationAnalysisDetailRepository SimulationAnalysisDetailRepo { get; set; }
        public ISimulationRepository SimulationRepo { get; set; }
        public IBudgetAmountRepository BudgetAmountRepo { get; set; }
        public IBudgetRepository BudgetRepo { get; set; }
        public ICriterionLibraryRepository CriterionLibraryRepo { get; set; }
        public ICashFlowDistributionRuleRepository CashFlowDistributionRuleRepo { get; set; }
        public ICashFlowRuleRepository CashFlowRuleRepo { get; set; }
        public IInvestmentPlanRepository InvestmentPlanRepo { get; set; }
        public IEquationRepository EquationRepo { get; set; }
        public IAttributeRepository AttributeRepo { get; set; }
        public IPerformanceCurveRepository PerformanceCurveRepo { get; set; }
        public ITreatmentConsequenceRepository TreatmentConsequenceRepo { get; set; }
        public ITreatmentCostRepository TreatmentCostRepo { get; set; }
        public ITreatmentSchedulingRepository TreatmentSchedulingRepo { get; set; }
        public ITreatmentSupersessionRepository TreatmentSupersessionRepo { get; set; }
        public ISelectableTreatmentRepository SelectableTreatmentRepo { get; set; }
        public IBudgetPercentagePairRepository BudgetPercentagePairRepo { get; set; }
        public IBudgetPriorityRepository BudgetPriorityRepo { get; set; }
        public ITargetConditionGoalRepository TargetConditionGoalRepo { get; set; }
        public IDeficientConditionGoalRepository DeficientConditionGoalRepo { get; set; }
        public IBenefitRepository BenefitRepo { get; set; }
        public IRemainingLifeLimitRepository RemainingLifeLimitRepo { get; set; }
        public IAnalysisMethodRepository AnalysisMethodRepo { get; set; }
        public ISimulationOutputRepository SimulationOutputRepo { get; set; }
        public ICommittedProjectConsequenceRepository CommittedProjectConsequenceRepo { get; set; }
        public ICommittedProjectRepository CommittedProjectRepo { get; set; }*/
        public IHubContext<BridgeCareHub> HubContext { get; set; }
        public UnitOfWork UnitOfWork { get; set; }

        public Simulation StandAloneSimulation { get; set; }

        public SimulationAnalysisDataPersistenceTestHelper()
        {
            Config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testConnections.json")
                .Build();

            _sqlConnection = new SqlConnection(Config.GetConnectionString("BridgeCareLegacyConnex"));
            _sqlConnection.Open();
            _dataAccessor = new DataAccessor(_sqlConnection, null);
            _dbContext = new IAMContext(new DbContextOptionsBuilder<IAMContext>()
                .UseInMemoryDatabase(databaseName: "IAMv2")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);
            UnitOfWork = new UnitOfWork(Config, _dbContext);
        }

        public Simulation GetStandAloneSimulation() => _dataAccessor.GetStandAloneSimulation(NetworkId, SimulationId);

            /*public void InitializeNetworkRepos()
        {
            NetworkRepo = new NetworkRepository(_dbContext);
            AttributeValueHistoryRepo = new AttributeValueHistoryRepository(_dbContext);
            SectionRepo = new SectionRepository(AttributeValueHistoryRepo, _dbContext);
            FacilityRepo = new FacilityRepository(SectionRepo, _dbContext);
        }

        public void InitializeEquationAndCriterionRepos()
        {
            EquationRepo = new EquationRepository(_dbContext);
            CriterionLibraryRepo = new CriterionLibraryRepository(_dbContext);
        }

        public void InitializeAttributeRepos()
        {
            AttributeMetaDataRepo = new AttributeMetaDataRepository();
            AttributeRepo = new AttributeRepository(EquationRepo, CriterionLibraryRepo, _dbContext);
        }

        public void InitializeAnalysisMethodRepos()
        {
            BudgetPercentagePairRepo = new BudgetPercentagePairRepository(_dbContext);
            BudgetPriorityRepo = new BudgetPriorityRepository(CriterionLibraryRepo, BudgetPercentagePairRepo, _dbContext);
            TargetConditionGoalRepo = new TargetConditionGoalRepository(CriterionLibraryRepo, _dbContext);
            DeficientConditionGoalRepo = new DeficientConditionGoalRepository(CriterionLibraryRepo, _dbContext);
            BenefitRepo = new BenefitRepository(_dbContext);
            RemainingLifeLimitRepo = new RemainingLifeLimitRepository(CriterionLibraryRepo, _dbContext);
            AnalysisMethodRepo = new AnalysisMethodRepository(CriterionLibraryRepo,
                BudgetPriorityRepo,
                TargetConditionGoalRepo,
                DeficientConditionGoalRepo,
                BenefitRepo,
                RemainingLifeLimitRepo,
                _dbContext);
        }

        public void InitializeCommittedProjectRepos()
        {
            CommittedProjectConsequenceRepo = new CommittedProjectConsequenceRepository(_dbContext);
            CommittedProjectRepo = new CommittedProjectRepository(CommittedProjectConsequenceRepo, _dbContext);
        }

        public void InitializeInvestmentPlanRepos()
        {
            BudgetAmountRepo = new BudgetAmountRepository(_dbContext);
            BudgetRepo = new BudgetRepository(BudgetAmountRepo, _dbContext);
            CashFlowDistributionRuleRepo = new CashFlowDistributionRuleRepository(_dbContext);
            CashFlowRuleRepo = new CashFlowRuleRepository(CashFlowDistributionRuleRepo, CriterionLibraryRepo, _dbContext);
            InvestmentPlanRepo = new InvestmentPlanRepository(BudgetRepo, CriterionLibraryRepo, CashFlowRuleRepo, _dbContext);
        }

        public void InitializePerformanceCurveRepo() => PerformanceCurveRepo = new PerformanceCurveRepository(EquationRepo, CriterionLibraryRepo, _dbContext);

        public void InitializeSelectableTreatmentRepos()
        {
            TreatmentConsequenceRepo = new TreatmentConsequenceRepository(EquationRepo, CriterionLibraryRepo, _dbContext);
            TreatmentCostRepo = new TreatmentCostRepository(EquationRepo, CriterionLibraryRepo, _dbContext);
            TreatmentSchedulingRepo = new TreatmentSchedulingRepository(_dbContext);
            TreatmentSupersessionRepo = new TreatmentSupersessionRepository(CriterionLibraryRepo, _dbContext);
            SelectableTreatmentRepo = new SelectableTreatmentRepository(TreatmentConsequenceRepo,
                TreatmentCostRepo,
                CriterionLibraryRepo,
                TreatmentSchedulingRepo,
                TreatmentSupersessionRepo,
                _dbContext);
        }

        public void InitializeSimulationRepo()
        {
            SimulationAnalysisDetailRepo = new SimulationAnalysisDetailRepository(_dbContext);
            SimulationRepo = new SimulationRepository(SimulationAnalysisDetailRepo, Config, _dbContext);
        }

        public void InitializeSimulationOutputRepo() => SimulationOutputRepo = new SimulationOutputRepository(_dbContext);*/

        public void CreateNetwork()
        {
            UnitOfWork.NetworkRepo.CreateNetwork(StandAloneSimulation.Network);

            UnitOfWork.FacilityRepo.CreateFacilities(StandAloneSimulation.Network.Facilities.ToList(), StandAloneSimulation.Network.Id);
        }

        public void CreateAttributes() => UnitOfWork.AttributeRepo.UpsertAttributes(UnitOfWork.AttributeMetaDataRepo.GetAllAttributes().ToList());

        public void CreateAttributeCriteriaAndEquationJoins() => UnitOfWork.AttributeRepo.JoinAttributesWithEquationsAndCriteria(StandAloneSimulation.Network.Explorer);

        public void AddTreatmentSchedulings()
        {
            var year = StandAloneSimulation.InvestmentPlan.FirstYearOfAnalysisPeriod + 1;
            StandAloneSimulation.Treatments.ForEach(_ =>
            {
                var scheduling = _.Schedulings.GetAdd(new TreatmentScheduling());
                scheduling.OffsetToFutureYear = year;
                scheduling.Treatment = _;
                if (year <= StandAloneSimulation.InvestmentPlan.LastYearOfAnalysisPeriod)
                {
                    year++;
                }
            });
        }

        public async void SynchronizeLegacySimulation()
        {
            var legacySimulationSynchronizer = new LegacySimulationSynchronizer(HubContext, UnitOfWork);
            await legacySimulationSynchronizer.Synchronize(SimulationId);
        }

        public void AddTreatmentSupersessions()
        {
            var treatmentsList = StandAloneSimulation.Treatments.ToList();
            StandAloneSimulation.Treatments.ForEach((_, index) =>
            {
                var supersession = _.AddSupersession();
                supersession.Criterion.Expression = _.FeasibilityCriteria.FirstOrDefault()?.Expression;
                var nextIndex = index + 1;
                supersession.Treatment = nextIndex > StandAloneSimulation.Treatments.Count() - 1
                    ? treatmentsList[0]
                    : treatmentsList[nextIndex];
            });
        }

        public void AddCommittedProjects()
        {
            var selectableTreatment = StandAloneSimulation.Treatments.First();

            var budget = StandAloneSimulation.InvestmentPlan.Budgets
                .Single(_ => _.Name == selectableTreatment.Budgets.First().Name);

            var committedProject = StandAloneSimulation.CommittedProjects
                .GetAdd(new CommittedProject(StandAloneSimulation.Network.Sections.First(), StandAloneSimulation.InvestmentPlan.FirstYearOfAnalysisPeriod));
            committedProject.Name = $"Committed Project Test";
            committedProject.Budget = budget;
            selectableTreatment.Consequences.DistinctBy(_ => _.Attribute).ForEach(_ =>
            {
                var consequence = committedProject.Consequences.GetAdd(new TreatmentConsequence());
                consequence.Attribute = _.Attribute;
                consequence.Change.Expression = _.Change.Expression;
            });
            committedProject.ShadowForAnyTreatment = selectableTreatment.ShadowForAnyTreatment;
            committedProject.ShadowForSameTreatment = selectableTreatment.ShadowForSameTreatment;
            committedProject.Cost = Convert.ToDouble(budget.YearlyAmounts.First().Value);
        }

        public void SetupAll()
        {
            StandAloneSimulation = GetStandAloneSimulation();
            /*InitializeEquationAndCriterionRepos();
            InitializeAttributeRepos();*/
            CreateAttributes();
        }

        public void SetupForNetwork()
        {
            SetupAll();
            //InitializeNetworkRepos();
        }

        public void SetupForSimulation()
        {
            SetupAll();
            /*InitializeNetworkRepos();
            InitializeSimulationRepo();*/
            CreateNetwork();
        }

        public void SetupForAnalysisMethod()
        {
            SetupAll();
            /*InitializeNetworkRepos();
            InitializeSimulationRepo();
            InitializeInvestmentPlanRepos();
            InitializeAnalysisMethodRepos();*/
            CreateNetwork();
            UnitOfWork.SimulationRepo.CreateSimulation(StandAloneSimulation);
            UnitOfWork.InvestmentPlanRepo.CreateInvestmentPlan(StandAloneSimulation.InvestmentPlan, StandAloneSimulation.Id);
        }

        public void SetupForPerformanceCurves()
        {

            SetupAll();
            /*InitializeNetworkRepos();
            InitializeSimulationRepo();
            InitializePerformanceCurveRepo();*/
            CreateNetwork();
            UnitOfWork.SimulationRepo.CreateSimulation(StandAloneSimulation);
        }

        public void SetupForSimulationOutput()
        {
            SetupAll();
            /*InitializeNetworkRepos();
            InitializeSimulationRepo();
            InitializeSimulationOutputRepo();*/
            CreateNetwork();
            UnitOfWork.SimulationRepo.CreateSimulation(StandAloneSimulation);
        }

        public void SetupForInvestmentPlan()
        {
            SetupAll();
            /*InitializeNetworkRepos();
            InitializeSimulationRepo();
            InitializeInvestmentPlanRepos();*/
            CreateNetwork();
            UnitOfWork.SimulationRepo.CreateSimulation(StandAloneSimulation);
        }

        public void SetupForCommittedProjects()
        {
            SetupAll();
            /*InitializeNetworkRepos();
            InitializeSimulationRepo();
            InitializeInvestmentPlanRepos();
            InitializeCommittedProjectRepos();*/
            CreateNetwork();
            UnitOfWork.SimulationRepo.CreateSimulation(StandAloneSimulation);
            UnitOfWork.InvestmentPlanRepo.CreateInvestmentPlan(StandAloneSimulation.InvestmentPlan, StandAloneSimulation.Id);
            AddCommittedProjects();
        }

        public void SetupForSelectableTreatments()
        {
            SetupAll();
            /*InitializeNetworkRepos();
            InitializeSimulationRepo();
            InitializeInvestmentPlanRepos();
            InitializeSelectableTreatmentRepos();*/
            CreateNetwork();
            UnitOfWork.SimulationRepo.CreateSimulation(StandAloneSimulation);
            UnitOfWork.InvestmentPlanRepo.CreateInvestmentPlan(StandAloneSimulation.InvestmentPlan, StandAloneSimulation.Id);
            AddTreatmentSchedulings();
            AddTreatmentSupersessions();
        }

        public void SetupForFullSimulationAnalysisIntegration()
        {
            SetupAll();
            /*InitializeNetworkRepos();
            InitializeSimulationRepo();
            InitializeInvestmentPlanRepos();
            InitializeAnalysisMethodRepos();
            InitializePerformanceCurveRepo();
            InitializeCommittedProjectRepos();
            InitializeSelectableTreatmentRepos();
            InitializeSimulationOutputRepo();*/
        }

        public void SetupForLegacySimulationSynchronization()
        {
            StandAloneSimulation = GetStandAloneSimulation();
            /*InitializeEquationAndCriterionRepos();
            InitializeAttributeRepos();
            InitializeNetworkRepos();
            InitializeSimulationRepo();
            InitializeInvestmentPlanRepos();
            InitializeAnalysisMethodRepos();
            InitializePerformanceCurveRepo();
            InitializeSelectableTreatmentRepos();*/
        }

        public void CleanUp()
        {
            _sqlConnection.Close();
            _dbContext.Database.EnsureDeleted();
            UnitOfWork.Dispose();
        }
    }
}
