using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    public class UnitOfDataPersistenceWork : IDisposable
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        public UnitOfDataPersistenceWork(IConfiguration config, IAMContext context)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));

            Context = context ?? throw new ArgumentNullException(nameof(context));
            if (!IsRunningFromXUnit)
            {
                Context.Database.SetCommandTimeout(1800);
            }

            Connection = new Microsoft.Data.SqlClient.SqlConnection(Config.GetConnectionString("BridgeCareConnex"));
            LegacyConnection = new SqlConnection(Config.GetConnectionString("BridgeCareLegacyConnex"));
        }

        public IConfiguration Config { get; }

        public IAMContext Context { get; }

        public Microsoft.Data.SqlClient.SqlConnection Connection { get; }

        public SqlConnection LegacyConnection { get; }

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
        private IUserRepository _userRepo;
        private ISimulationReportDetailRepository _simulationReportDetailRepo;
        private IUserCriteriaRepository _userCriteriaRepo;
        private IReportIndexRepository _reportIndexRepo;
        private IAssetData _assetDataRepository;

        public IAggregatedResultRepository AggregatedResultRepo => _aggregatedResultRepo ??= new AggregatedResultRepository(this);

        public IAnalysisMethodRepository AnalysisMethodRepo => _analysisMethodRepo ??= new AnalysisMethodRepository(this);

        public IAttributeDatumRepository AttributeDatumRepo => _attributeDatumRepo ??= new AttributeDatumRepository(this);

        public IAttributeMetaDataRepository AttributeMetaDataRepo => _attributeMetaDataRepo ??= new AttributeMetaDataRepository();

        public IAttributeRepository AttributeRepo => _attributeRepo ??= new AttributeRepository(this);

        public IAttributeValueHistoryRepository AttributeValueHistoryRepo => _attributeValueHistoryRepo ??= new AttributeValueHistoryRepository(this);

        public IBenefitRepository BenefitRepo => _benefitRepo ??= new BenefitRepository(this);

        public IBudgetAmountRepository BudgetAmountRepo => _budgetAmountRepo ??= new BudgetAmountRepository(this);

        public IBudgetPercentagePairRepository BudgetPercentagePairRepo => _budgetPercentagePairRepo ??= new BudgetPercentagePairRepository(this);

        public IBudgetPriorityRepository BudgetPriorityRepo => _budgetPriorityRepo ??= new BudgetPriorityRepository(this);

        public IBudgetRepository BudgetRepo => _budgetRepo ??= new BudgetRepository(this);

        public ICashFlowDistributionRuleRepository CashFlowDistributionRuleRepo => _cashFlowDistributionRuleRepo ??= new CashFlowDistributionRuleRepository(this);

        public ICashFlowRuleRepository CashFlowRuleRepo => _cashFlowRuleRepo ??= new CashFlowRuleRepository(this);

        public ICommittedProjectConsequenceRepository CommittedProjectConsequenceRepo => _committedProjectConsequenceRepo ??= new CommittedProjectConsequenceRepository(this);

        public ICommittedProjectRepository CommittedProjectRepo => _committedProjectRepo ??= new CommittedProjectRepository(this);

        public ICriterionLibraryRepository CriterionLibraryRepo => _criterionLibraryRepo ??= new CriterionLibraryRepository(this);

        public IDeficientConditionGoalRepository DeficientConditionGoalRepo => _deficientConditionGoalRepo ??= new DeficientConditionGoalRepository(this);

        public IEquationRepository EquationRepo => _equationRepo ??= new EquationRepository(this);

        public IFacilityRepository FacilityRepo => _facilityRepo ??= new FacilityRepository(this);

        public IInvestmentPlanRepository InvestmentPlanRepo => _investmentPlanRepo ??= new InvestmentPlanRepository(this);

        public IMaintainableAssetRepository MaintainableAssetRepo => _maintainableAssetRepo ??= new MaintainableAssetRepository(this);

        public virtual INetworkRepository NetworkRepo => _networkRepo ??= new NetworkRepository(this);

        public IPerformanceCurveRepository PerformanceCurveRepo => _performanceCurveRepo ??= new PerformanceCurveRepository(this);

        public IRemainingLifeLimitRepository RemainingLifeLimitRepo => _remainingLifeLimitRepo ??= new RemainingLifeLimitRepository(this);

        public ISectionRepository SectionRepo => _sectionRepo ??= new SectionRepository(this);

        public ISelectableTreatmentRepository SelectableTreatmentRepo => _selectableTreatmentRepo ??= new SelectableTreatmentRepository(this);

        public ISimulationAnalysisDetailRepository SimulationAnalysisDetailRepo => _simulationAnalysisDetailRepo ??= new SimulationAnalysisDetailRepository(this);

        public ISimulationOutputRepository SimulationOutputRepo => _simulationOutputRepo ??= new SimulationOutputRepository(this);

        public ISimulationRepository SimulationRepo => _simulationRepo ??= new SimulationRepository(this);

        public ITargetConditionGoalRepository TargetConditionGoalRepo => _targetConditionGoalRepo ??= new TargetConditionGoalRepository(this);

        public ITreatmentConsequenceRepository TreatmentConsequenceRepo => _treatmentConsequenceRepo ??= new TreatmentConsequenceRepository(this);

        public ITreatmentCostRepository TreatmentCostRepo => _treatmentCostRepo ??= new TreatmentCostRepository(this);

        public ITreatmentSchedulingRepository TreatmentSchedulingRepo => _treatmentSchedulingRepo ??= new TreatmentSchedulingRepository(this);

        public ITreatmentSupersessionRepository TreatmentSupersessionRepo => _treatmentSupersessionRepo ??= new TreatmentSupersessionRepository(this);

        public IUserRepository UserRepo => _userRepo ??= new UserRepository(this);

        public ISimulationReportDetailRepository SimulationReportDetailRepo => _simulationReportDetailRepo ??= new SimulationReportDetailRepository(this);

        public IUserCriteriaRepository UserCriteriaRepo => _userCriteriaRepo ??= new UserCriteriaRepository(this);

        public IReportIndexRepository ReportIndexRepository => _reportIndexRepo ??= new ReportIndexRepository(this);

        public IAssetData AssetDataRepository => _assetDataRepository ??= new PennDOTAssetDataRepository(new List<string>() { "BRKey", "BMSID" }, this);

        public IDbContextTransaction DbContextTransaction
        {
            get => _dbContextTransaction;
            private set => _dbContextTransaction = value;
        }

        public void BeginTransaction() => DbContextTransaction = Context.Database.BeginTransaction();

        public void Commit()
        {
            if (DbContextTransaction != null)
            {
                DbContextTransaction.Commit();
                DbContextTransaction.Dispose();
            }
        }

        public void Rollback()
        {
            if (DbContextTransaction != null)
            {
                DbContextTransaction.Rollback();
                DbContextTransaction.Dispose();
            }
        }

        // DISPOSE PROPERTIES & METHODS
        private bool _disposed = false;

        private IDbContextTransaction _dbContextTransaction;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
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
