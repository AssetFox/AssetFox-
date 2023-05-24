using System;
using System.Data.SqlClient;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork
{
    public class UnitOfDataPersistenceWork : IDisposable, IUnitOfWork
    {
        public UnitOfDataPersistenceWork(IConfiguration config, IAMContext context)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            Context = context ?? throw new ArgumentNullException(nameof(context));

            EncryptionKey = Config.GetChildren().Count() > 0 && Config != null ? Config.GetSection("EncryptionKey").Value : String.Empty;
        }
                
        public string EncryptionKey { get; }

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
        private IBudgetPriorityRepository _budgetPriorityRepo;
        private IBudgetRepository _budgetRepo;
        private ICashFlowDistributionRuleRepository _cashFlowDistributionRuleRepo;
        private ICashFlowRuleRepository _cashFlowRuleRepo;
        private ICommittedProjectConsequenceRepository _committedProjectConsequenceRepo;
        private ICommittedProjectRepository _committedProjectRepo;
        private ICriterionLibraryRepository _criterionLibraryRepo;
        private IDeficientConditionGoalRepository _deficientConditionGoalRepo;
        private IExcelRawDataRepository _excelWorksheetRepo;
        private IInvestmentPlanRepository _investmentPlanRepo;
        private IMaintainableAssetRepository _maintainableAssetRepo;
        private INetworkRepository _networkRepo;
        private IPerformanceCurveRepository _performanceCurveRepo;
        private ICalculatedAttributesRepository _calculatedAttributesRepo;
        private IRemainingLifeLimitRepository _remainingLifeLimitRepo;
        private ISelectableTreatmentRepository _selectableTreatmentRepo;
        private ISimulationAnalysisDetailRepository _simulationAnalysisDetailRepo;
        private ISimulationLogRepository _simulationLogRepo;
        private ISimulationOutputRepository _simulationOutputRepo;
        private ISimulationRepository _simulationRepo;
        private ITargetConditionGoalRepository _targetConditionGoalRepo;
        private ITreatmentConsequenceRepository _treatmentConsequenceRepo;
        private ITreatmentCostRepository _treatmentCostRepo;
        private ITreatmentSchedulingRepository _treatmentSchedulingRepo;
        private ITreatmentSupersessionRepository _treatmentSupersessionRepo;
        private IUserRepository _userRepo;
        private ISiteRepository _SiteRepo;
        private IAdminDataRepository _adminDataRepo;
        private ISimulationReportDetailRepository _simulationReportDetailRepo;
        private IBenefitQuantifierRepository _benefitQuantifierRepo;
        private IUserCriteriaRepository _userCriteriaRepo;
        private IReportIndexRepository _reportIndexRepo;
        private IAssetData _assetDataRepository;
        private IAnnouncementRepository _announcementRepo;
        private IDataSourceRepository _dataSourceRepo;
        private ITreatmentLibraryUserRepository _treatmentLibraryUserRepo; 

        public ITreatmentLibraryUserRepository TreatmentLibraryUserRepo => _treatmentLibraryUserRepo ??= new TreatmentLibraryUserRepository(this);
        public IAggregatedResultRepository AggregatedResultRepo => _aggregatedResultRepo ??= new AggregatedResultRepository(this);

        public IAnalysisMethodRepository AnalysisMethodRepo => _analysisMethodRepo ??= new AnalysisMethodRepository(this);

        public IAttributeDatumRepository AttributeDatumRepo => _attributeDatumRepo ??= new AttributeDatumRepository(this);

        public IAttributeRepository AttributeRepo => _attributeRepo ??= new AttributeRepository(this);
        public IAttributeMetaDataRepository AttributeMetaDataRepo => _attributeMetaDataRepo ?? new AttributeMetaDataRepository();
        public IAttributeValueHistoryRepository AttributeValueHistoryRepo => _attributeValueHistoryRepo ??= new AttributeValueHistoryRepository(this);

        public IBenefitRepository BenefitRepo => _benefitRepo ??= new BenefitRepository(this);

        public IBudgetAmountRepository BudgetAmountRepo => _budgetAmountRepo ??= new BudgetAmountRepository(this);

        public IBudgetPriorityRepository BudgetPriorityRepo => _budgetPriorityRepo ??= new BudgetPriorityRepository(this);

        public IBudgetRepository BudgetRepo => _budgetRepo ??= new BudgetRepository(this);

        public ICashFlowDistributionRuleRepository CashFlowDistributionRuleRepo => _cashFlowDistributionRuleRepo ??= new CashFlowDistributionRuleRepository(this);

        public ICashFlowRuleRepository CashFlowRuleRepo => _cashFlowRuleRepo ??= new CashFlowRuleRepository(this);

        public ICommittedProjectConsequenceRepository CommittedProjectConsequenceRepo => _committedProjectConsequenceRepo ??= new CommittedProjectConsequenceRepository(this);

        public ICommittedProjectRepository CommittedProjectRepo => _committedProjectRepo ??= new CommittedProjectRepository(this);

        public ICriterionLibraryRepository CriterionLibraryRepo => _criterionLibraryRepo ??= new CriterionLibraryRepository(this);

        public IDeficientConditionGoalRepository DeficientConditionGoalRepo => _deficientConditionGoalRepo ??= new DeficientConditionGoalRepository(this);

        public IExcelRawDataRepository ExcelWorksheetRepository => _excelWorksheetRepo ?? new ExcelRawDataRepository(this);

        public IInvestmentPlanRepository InvestmentPlanRepo => _investmentPlanRepo ??= new InvestmentPlanRepository(this);

        public IMaintainableAssetRepository MaintainableAssetRepo => _maintainableAssetRepo ??= new MaintainableAssetRepository(this);

        public virtual INetworkRepository NetworkRepo => _networkRepo ??= new NetworkRepository(this);

        public IPerformanceCurveRepository PerformanceCurveRepo => _performanceCurveRepo ??= new PerformanceCurveRepository(this);

        public ICalculatedAttributesRepository CalculatedAttributeRepo => _calculatedAttributesRepo ??= new CalculatedAttributeRepository(this);

        public IRemainingLifeLimitRepository RemainingLifeLimitRepo => _remainingLifeLimitRepo ??= new RemainingLifeLimitRepository(this);

        public ISelectableTreatmentRepository SelectableTreatmentRepo => _selectableTreatmentRepo ??= new SelectableTreatmentRepository(this);

        public ISimulationAnalysisDetailRepository SimulationAnalysisDetailRepo => _simulationAnalysisDetailRepo ??= new SimulationAnalysisDetailRepository(this);

        public ISimulationLogRepository SimulationLogRepo => _simulationLogRepo ??= new SimulationLogRepository(this);

        public ISimulationOutputRepository SimulationOutputRepo => _simulationOutputRepo ??= new SimulationOutputRepository(this);

        public ISimulationRepository SimulationRepo => _simulationRepo ??= new SimulationRepository(this);

        public ITargetConditionGoalRepository TargetConditionGoalRepo => _targetConditionGoalRepo ??= new TargetConditionGoalRepository(this);

        public ITreatmentConsequenceRepository TreatmentConsequenceRepo => _treatmentConsequenceRepo ??= new TreatmentConsequenceRepository(this);

        public ITreatmentCostRepository TreatmentCostRepo => _treatmentCostRepo ??= new TreatmentCostRepository(this);

        public ITreatmentSchedulingRepository TreatmentSchedulingRepo => _treatmentSchedulingRepo ??= new TreatmentSchedulingRepository(this);

        public ITreatmentSupersessionRepository TreatmentSupersessionRepo => _treatmentSupersessionRepo ??= new TreatmentSupersessionRepository(this);

        public IUserRepository UserRepo => _userRepo ??= new UserRepository(this);

        public ISiteRepository SiteRepo => _SiteRepo ??= new SiteRepository(this);

        public IAdminDataRepository AdminDataRepo => _adminDataRepo ??= new AdminDataRepository(this);

        public ISimulationReportDetailRepository SimulationReportDetailRepo => _simulationReportDetailRepo ??= new SimulationReportDetailRepository(this);

        public IBenefitQuantifierRepository BenefitQuantifierRepo => _benefitQuantifierRepo ??= new BenefitQuantifierRepository(this);

        public IUserCriteriaRepository UserCriteriaRepo => _userCriteriaRepo ??= new UserCriteriaRepository(this);

        public IReportIndexRepository ReportIndexRepository => _reportIndexRepo ??= new ReportIndexRepository(this);

        public IAssetData AssetDataRepository => _assetDataRepository ??= new MaintainableAssetDataRepository(this);

        public IAnnouncementRepository AnnouncementRepo => _announcementRepo ??= new AnnouncementRepository(this);

        public IDataSourceRepository DataSourceRepo => _dataSourceRepo ??= new DataSourceRepository(this);

        public UserDTO CurrentUser => UserEntity?.ToDto();

        public UserEntity UserEntity { get; private set; }

        public IDbContextTransaction DbContextTransaction { get; private set; }

        /// <summary><inheritdoc cref="IUnitOfWork.BeginTransaction"/></summary>
        public void BeginTransaction() => DbContextTransaction = Context.Database.BeginTransaction();

        public void SetUser(string username)
        {
            if (UserEntity?.Username != username)
            {
                var user = Context.User
                    .Include(_ => _.UserCriteriaFilterJoin)
                    .FirstOrDefault(_ => _.Username == username);
                UserEntity = user;
            }
        }

        private static object AddUserLock = new object();
        public void AddUser(string username, bool hasAdminClaim)
        {
            lock (AddUserLock)
            {
                if (!UserRepo.UserExists(username))
                {
                    BeginTransaction();
                    UserRepo.AddUser(username, hasAdminClaim);
                    Commit();
                }
            }
        }

        /// <summary><inheritdoc cref="IUnitOfWork.Commit"/></summary>
        public void Commit()
        {
            if (DbContextTransaction != null)
            {
                Context.SaveChanges();
                DbContextTransaction.Commit();
                DbContextTransaction.Dispose();
            }
        }

        /// <summary><inheritdoc cref="IUnitOfWork.Rollback"/></summary>
        public void Rollback()
        {
            if (DbContextTransaction != null)
            {
                DbContextTransaction.Rollback();
                DbContextTransaction.Dispose();
            }
        }

        // DISPOSE PROPERTIES & METHODS
        private bool _disposed;

        private void Dispose(bool disposing)
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
