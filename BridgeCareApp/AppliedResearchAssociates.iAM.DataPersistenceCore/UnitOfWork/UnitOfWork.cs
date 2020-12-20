using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using AttributeDatumRepository = AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.AttributeDatumRepository;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork
{
    public class UnitOfWork : IDisposable
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        /*private readonly IConfiguration _config;
        private readonly IAMContext _context;*/

        public UnitOfWork(IConfiguration config, IAMContext context)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));

            Context = context ?? throw new ArgumentNullException(nameof(context));
            if (!IsRunningFromXUnit)
            {
                Context.Database.SetCommandTimeout(1800);
            }
        }

        public IConfiguration Config { get; }
        public IAMContext Context { get; }

        // REPOSITORIES
        private IAggregatedResultRepository _aggregatedResultRepo;
        private IAnalysisMethodRepository _analysisMethodRepo;
        private IAttributeDatumRepository _attributeDatumRepo;
        private IAttributeMetaDataRepository _attributeMetaDataRepo;
        private IAttributeRepository _attributeRepo;
        private IAttributeValueHistoryRepository _attributeValueHistoryRepo;
        private IBenefitRepository _benefitRepo;
        private IBudgetAmountRepository _budgetAmountRepo;
        private IBudgetPercentagePairRepository _budgetPercentagePairRepo;
        private IBudgetPriorityRepository _budgetPriorityRepo;
        private IBudgetRepository _budgetRepo;
        private ICashFlowDistributionRuleRepository _cashFlowDistributionRuleRepo;
        private ICashFlowRuleRepository _cashFlowRuleRepo;
        private ICommittedProjectConsequenceRepository _committedProjectConsequenceRepo;
        private ICommittedProjectRepository _committedProjectRepo;
        private ICriterionLibraryRepository _criterionLibraryRepo;
        private IDeficientConditionGoalRepository _deficientConditionGoalRepo;
        private IEquationRepository _equationRepo;
        private IFacilityRepository _facilityRepo;
        private IInvestmentPlanRepository _investmentPlanRepo;
        private IMaintainableAssetRepository _maintainableAssetRepo;
        private INetworkRepository _networkRepo;
        private IPerformanceCurveRepository _performanceCurveRepo;
        private IRemainingLifeLimitRepository _remainingLifeLimitRepo;
        private ISectionRepository _sectionRepo;
        private ISelectableTreatmentRepository _selectableTreatmentRepo;
        private ISimulationAnalysisDetailRepository _simulationAnalysisDetailRepo;
        private ISimulationOutputRepository _simulationOutputRepo;
        private ISimulationRepository _simulationRepo;
        private ITargetConditionGoalRepository _targetConditionGoalRepo;
        private ITreatmentConsequenceRepository _treatmentConsequenceRepo;
        private ITreatmentCostRepository _treatmentCostRepo;
        private ITreatmentSchedulingRepository _treatmentSchedulingRepo;
        private ITreatmentSupersessionRepository _treatmentSupersessionRepo;
        private IDbContextTransaction _dbContextTransaction;

        public IAggregatedResultRepository AggregatedResultRepo => _aggregatedResultRepo ??= new AggregatedResultRepository(_context);

        public IAnalysisMethodRepository AnalysisMethodRepo =>
            _analysisMethodRepo ??= new AnalysisMethodRepository(CriterionLibraryRepo,
                BudgetPriorityRepo, TargetConditionGoalRepo, DeficientConditionGoalRepo, BenefitRepo,
                RemainingLifeLimitRepo, _context);

        public IAttributeDatumRepository AttributeDatumRepo => _attributeDatumRepo ??= new AttributeDatumRepository(AttributeMetaDataRepo, AttributeRepo, _context);

        public IAttributeMetaDataRepository AttributeMetaDataRepo => _attributeMetaDataRepo ??= new AttributeMetaDataRepository();

        public IAttributeRepository AttributeRepo => _attributeRepo ??= new AttributeRepository(EquationRepo, CriterionLibraryRepo, _context);

        public IAttributeValueHistoryRepository AttributeValueHistoryRepo => _attributeValueHistoryRepo ??= new AttributeValueHistoryRepository(_config, _context);

        public IBenefitRepository BenefitRepo => _benefitRepo ??= new BenefitRepository(_context);

        public IBudgetAmountRepository BudgetAmountRepo => _budgetAmountRepo ??= new BudgetAmountRepository(_context);

        public IBudgetPercentagePairRepository BudgetPercentagePairRepo => _budgetPercentagePairRepo ??= new BudgetPercentagePairRepository(_context);

        public IBudgetPriorityRepository BudgetPriorityRepo => _budgetPriorityRepo ??= new BudgetPriorityRepository(CriterionLibraryRepo, BudgetPercentagePairRepo, _context);

        public IBudgetRepository BudgetRepo => _budgetRepo ??= new BudgetRepository(BudgetAmountRepo, _context);

        public ICashFlowDistributionRuleRepository CashFlowDistributionRuleRepo => _cashFlowDistributionRuleRepo ??= new CashFlowDistributionRuleRepository(_context);

        public ICashFlowRuleRepository CashFlowRuleRepo => _cashFlowRuleRepo ??= new CashFlowRuleRepository(CashFlowDistributionRuleRepo, CriterionLibraryRepo, _context);

        public ICommittedProjectConsequenceRepository CommittedProjectConsequenceRepo => _committedProjectConsequenceRepo ??= new CommittedProjectConsequenceRepository(_context);

        public ICommittedProjectRepository CommittedProjectRepo => _committedProjectRepo ??= new CommittedProjectRepository(CommittedProjectConsequenceRepo, _context);

        public ICriterionLibraryRepository CriterionLibraryRepo => _criterionLibraryRepo ??= new CriterionLibraryRepository(_context);

        public IDeficientConditionGoalRepository DeficientConditionGoalRepo => _deficientConditionGoalRepo ??= new DeficientConditionGoalRepository(CriterionLibraryRepo, _context);

        public IEquationRepository EquationRepo => _equationRepo ??= new EquationRepository(_context);

        public IFacilityRepository FacilityRepo => _facilityRepo ??= new FacilityRepository(SectionRepo, _context);

        public IInvestmentPlanRepository InvestmentPlanRepo => _investmentPlanRepo ??= new InvestmentPlanRepository(BudgetRepo, CriterionLibraryRepo, CashFlowRuleRepo, _context);

        public IMaintainableAssetRepository MaintainableAssetRepo => _maintainableAssetRepo ??= new MaintainableAssetRepository(_context);

        public INetworkRepository NetworkRepo => _networkRepo ??= new NetworkRepository(_context);

        public IPerformanceCurveRepository PerformanceCurveRepo => _performanceCurveRepo ??= new PerformanceCurveRepository(EquationRepo, CriterionLibraryRepo, _context);

        public IRemainingLifeLimitRepository RemainingLifeLimitRepo => _remainingLifeLimitRepo ??= new RemainingLifeLimitRepository(CriterionLibraryRepo, _context);

        public ISectionRepository SectionRepo => _sectionRepo ??= new SectionRepository(AttributeValueHistoryRepo, _context);

        public ISelectableTreatmentRepository SelectableTreatmentRepo =>
            _selectableTreatmentRepo ??= new SelectableTreatmentRepository(TreatmentConsequenceRepo, TreatmentCostRepo, CriterionLibraryRepo,
                TreatmentSchedulingRepo, TreatmentSupersessionRepo, _context);

        public ISimulationAnalysisDetailRepository SimulationAnalysisDetailRepo => _simulationAnalysisDetailRepo ??= new SimulationAnalysisDetailRepository(_context);

        public ISimulationOutputRepository SimulationOutputRepo => _simulationOutputRepo ??= new SimulationOutputRepository(_context);

        public ISimulationRepository SimulationRepo => _simulationRepo ??= new SimulationRepository(SimulationAnalysisDetailRepo, _config, _context);

        public ITargetConditionGoalRepository TargetConditionGoalRepo => _targetConditionGoalRepo ??= new TargetConditionGoalRepository(CriterionLibraryRepo, _context);

        public ITreatmentConsequenceRepository TreatmentConsequenceRepo => _treatmentConsequenceRepo ??= new TreatmentConsequenceRepository(EquationRepo, CriterionLibraryRepo, _context);

        public ITreatmentCostRepository TreatmentCostRepo => _treatmentCostRepo ??= new TreatmentCostRepository(EquationRepo, CriterionLibraryRepo, _context);

        public ITreatmentSchedulingRepository TreatmentSchedulingRepo => _treatmentSchedulingRepo ??= new TreatmentSchedulingRepository(_context);

        public ITreatmentSupersessionRepository TreatmentSupersessionRepo => _treatmentSupersessionRepo ??= new TreatmentSupersessionRepository(CriterionLibraryRepo, _context);

        public IDbContextTransaction DbContextTransaction => _dbContextTransaction ??= _context.Database.BeginTransaction();

        public void Commit() => DbContextTransaction.Commit();

        public void Rollback() => DbContextTransaction.Rollback();

        // DISPOSE PROPERTIES & PROPERTIES
        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
