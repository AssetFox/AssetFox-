using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataAccess;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.Domains;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MoreLinq;
using DA = AppliedResearchAssociates.iAM.DataAssignment;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestData
{
    public class SimulationAnalysisDataPersistenceTestHelper
    {
        private const int NetworkId = 13;
        private const int SimulationId = 1171;
        private const string SqlConnectionString = "data source=RMD-PPATORN2-LT\\SQLSERVER2014;initial catalog=DbBackup;persist security info=True;user id=sa;password=20Pikachu^;MultipleActiveResultSets=True;App=EntityFramework";

        private readonly SqlConnection _sqlConnection;
        private readonly DataAccessor _dataAccessor;
        private readonly IAMContext _dbContext;

        public ISimulationRepository SimulationRepo { get; set; }
        public IBudgetAmountRepository BudgetAmountRepo { get; set; }
        public IBudgetRepository BudgetRepo { get; set; }
        public ICriterionLibraryRepository CriterionLibraryRepo { get; set; }
        public ICashFlowDistributionRuleRepository CashFlowDistributionRuleRepo { get; set; }
        public ICashFlowRuleRepository CashFlowRuleRepo { get; set; }
        public IInvestmentPlanRepository InvestmentPlanRepo { get; set; }
        public IEquationRepository EquationRepo { get; set; }
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
        public IAnalysisMethodRepository AnalysisMethodRepo { get; set; }

        public SimulationAnalysisDataPersistenceTestHelper()
        {
            _sqlConnection = new SqlConnection(SqlConnectionString);
            _sqlConnection.Open();
            _dataAccessor = new DataAccessor(_sqlConnection, null);
            _dbContext = new IAMContext(new DbContextOptionsBuilder<IAMContext>()
                .UseInMemoryDatabase(databaseName: "IAMv2")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);
        }

        public void CleanUp()
        {
            _sqlConnection.Close();
            _dbContext.Database.EnsureDeleted();
        }

        public Simulation GetStandAloneSimulation() => _dataAccessor.GetStandAloneSimulation(NetworkId, SimulationId);

        public void CreateNetwork(string networkName)
        {
            var network = new DA.Networking.Network(new List<DA.Networking.MaintainableAsset>(), Guid.NewGuid(), networkName);

            var networkRepo = new NetworkRepository(_dbContext);

            networkRepo.CreateNetwork(network);
        }

        public void CreateAttributes(List<string> attributeNames)
        {
            var attributeMetaDataRepo = new AttributeMetaDataRepository();
            var attributes = attributeMetaDataRepo.GetAllAttributes();

            var attributeRepo = new AttributeRepository(_dbContext);
            attributeRepo.UpsertAttributes(attributes.Where(_ => attributeNames.Contains(_.Name)).ToList());
        }

        public void TestCreateSimulationEntitySetup() => SimulationRepo = new SimulationRepository(_dbContext);

        public void TestCreateInvestmentPlanEntitySetup(Simulation simulation)
        {
            CreateNetwork(simulation.Network.Name);
            SimulationRepo = new SimulationRepository(_dbContext);
            SimulationRepo.CreateSimulation(simulation);
            BudgetAmountRepo = new BudgetAmountRepository(_dbContext);
            BudgetRepo = new BudgetRepository(BudgetAmountRepo, _dbContext);
            CriterionLibraryRepo = new CriterionLibraryRepository(_dbContext);
            CashFlowDistributionRuleRepo = new CashFlowDistributionRuleRepository(_dbContext);
            CashFlowRuleRepo = new CashFlowRuleRepository(CashFlowDistributionRuleRepo, CriterionLibraryRepo, _dbContext);
            InvestmentPlanRepo = new InvestmentPlanRepository(BudgetRepo, CriterionLibraryRepo, CashFlowRuleRepo, _dbContext);
        }

        public void TestCreatePerformanceCurveEntitiesSetup(Simulation simulation)
        {
            CreateNetwork(simulation.Network.Name);
            SimulationRepo = new SimulationRepository(_dbContext);
            SimulationRepo.CreateSimulation(simulation);
            var attributeNames = simulation.PerformanceCurves.Select(_ => _.Attribute.Name).Distinct().ToList();
            CreateAttributes(attributeNames);
            EquationRepo = new EquationRepository(_dbContext);
            CriterionLibraryRepo = new CriterionLibraryRepository(_dbContext);
            PerformanceCurveRepo = new PerformanceCurveRepository(EquationRepo, CriterionLibraryRepo, _dbContext);
        }

        public void TestCreateSelectableTreatmentEntitiesSetup(Simulation simulation)
        {
            TestCreateInvestmentPlanEntitySetup(simulation);
            InvestmentPlanRepo.CreateInvestmentPlan(simulation.InvestmentPlan, simulation.Name);
            var attributeNames = simulation.Treatments
                .SelectMany(_ => _.Consequences.Select(__ => __.Attribute.Name).Distinct().ToList())
                .ToList();
            CreateAttributes(attributeNames);
            EquationRepo = new EquationRepository(_dbContext);
            TreatmentConsequenceRepo = new TreatmentConsequenceRepository(EquationRepo, CriterionLibraryRepo, _dbContext);
            TreatmentCostRepo = new TreatmentCostRepository(EquationRepo, CriterionLibraryRepo, _dbContext);
            TreatmentSchedulingRepo = new TreatmentSchedulingRepository(_dbContext);
            TreatmentSupersessionRepo = new TreatmentSupersessionRepository(CriterionLibraryRepo, _dbContext);
            SelectableTreatmentRepo = new SelectableTreatmentRepository(InvestmentPlanRepo,
                TreatmentConsequenceRepo,
                TreatmentCostRepo,
                CriterionLibraryRepo,
                TreatmentSchedulingRepo,
                TreatmentSupersessionRepo,
                _dbContext);
            // add treatment schedulings since there aren't any for our test simulation treatments
            var year = simulation.InvestmentPlan.FirstYearOfAnalysisPeriod + 1;
            simulation.Treatments.ForEach(_ =>
            {
                var scheduling = _.Schedulings.GetAdd(new TreatmentScheduling());
                scheduling.OffsetToFutureYear = year;
                scheduling.Treatment = _;
                if (year <= simulation.InvestmentPlan.LastYearOfAnalysisPeriod)
                {
                    year++;
                }
            });
            // add treatment supersessions since there aren't any for our test simulation treatments
            var treatmentsList = simulation.Treatments.ToList();
            simulation.Treatments.ForEach((_, index) =>
            {
                var supersession = _.AddSupersession();
                supersession.Criterion.Expression = _.FeasibilityCriteria.FirstOrDefault().Expression;
                var nextIndex = index + 1;
                supersession.Treatment = nextIndex > simulation.Treatments.Count() - 1
                    ? treatmentsList[0]
                    : treatmentsList[nextIndex];
            });
        }

        public void TestCreateAnalysisMethodEntitySetup(Simulation simulation)
        {
            TestCreateInvestmentPlanEntitySetup(simulation);
            InvestmentPlanRepo.CreateInvestmentPlan(simulation.InvestmentPlan, simulation.Name);
            var attributeNames = simulation.AnalysisMethod.TargetConditionGoals
                .Select(_ => _.Attribute.Name).ToList();
            attributeNames.Add(simulation.AnalysisMethod.Weighting.Name);
            CreateAttributes(attributeNames.Distinct().ToList());
            BudgetPercentagePairRepo = new BudgetPercentagePairRepository(_dbContext);
            BudgetPriorityRepo = new BudgetPriorityRepository(CriterionLibraryRepo, BudgetPercentagePairRepo, _dbContext);
            TargetConditionGoalRepo = new TargetConditionGoalRepository(CriterionLibraryRepo, _dbContext);
            DeficientConditionGoalRepo = new DeficientConditionGoalRepository(CriterionLibraryRepo, _dbContext);
            AnalysisMethodRepo = new AnalysisMethodRepository(CriterionLibraryRepo,
                BudgetPriorityRepo,
                InvestmentPlanRepo,
                TargetConditionGoalRepo,
                DeficientConditionGoalRepo,
                _dbContext);
        }
    }
}
