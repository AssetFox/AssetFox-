using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.Extensions.Configuration;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork
{
    public interface IUnitOfWork
    {
        IConfiguration Config { get; }

        public string EncryptionKey { get; }

        ///<summary>Start a new transaction for the database repository</summary>
        ///<remarks>Only use this when the transaction MUST occur outside the data repository</remarks>
        void BeginTransaction();

        IAggregatedResultRepository AggregatedResultRepo { get; }

        IAnalysisMethodRepository AnalysisMethodRepo { get; }

        IAttributeDatumRepository AttributeDatumRepo { get; }

        IAttributeRepository AttributeRepo { get; }

        IAttributeValueHistoryRepository AttributeValueHistoryRepo { get; }

        IBenefitRepository BenefitRepo { get; }

        IBudgetAmountRepository BudgetAmountRepo { get; }

        IBudgetPriorityRepository BudgetPriorityRepo { get; }

        IBudgetRepository BudgetRepo { get; }

        ICashFlowDistributionRuleRepository CashFlowDistributionRuleRepo { get; }

        ICashFlowRuleRepository CashFlowRuleRepo { get; }

        ICommittedProjectConsequenceRepository CommittedProjectConsequenceRepo { get; }

        ICommittedProjectRepository CommittedProjectRepo { get; }

        //ICompleteSimulationRepository CompleteSimulationRepo { get; }

        ICriterionLibraryRepository CriterionLibraryRepo { get; }

        IDeficientConditionGoalRepository DeficientConditionGoalRepo { get; }

        IInvestmentPlanRepository InvestmentPlanRepo { get; }

        IMaintainableAssetRepository MaintainableAssetRepo { get; }

        INetworkRepository NetworkRepo { get; }

        IPerformanceCurveRepository PerformanceCurveRepo { get; }

        ICalculatedAttributesRepository CalculatedAttributeRepo { get; }

        IRemainingLifeLimitRepository RemainingLifeLimitRepo { get; }

        ISelectableTreatmentRepository SelectableTreatmentRepo { get; }

        ITreatmentLibraryUserRepository TreatmentLibraryUserRepo { get; }

        ISimulationAnalysisDetailRepository SimulationAnalysisDetailRepo { get; }

        ISimulationLogRepository SimulationLogRepo { get; }

        ISimulationOutputRepository SimulationOutputRepo { get; }

        ISimulationRepository SimulationRepo { get; }

        IAdminSettingsRepository AdminSettingsRepo { get; }

        ITargetConditionGoalRepository TargetConditionGoalRepo { get; }

        ITreatmentConsequenceRepository TreatmentConsequenceRepo { get; }

        ITreatmentCostRepository TreatmentCostRepo { get; }

        ITreatmentSchedulingRepository TreatmentSchedulingRepo { get; }

        ITreatmentSupersessionRepository TreatmentSupersessionRepo { get; }

        IUserRepository UserRepo { get; }

        ISimulationReportDetailRepository SimulationReportDetailRepo { get; }

        IBenefitQuantifierRepository BenefitQuantifierRepo { get; }

        IUserCriteriaRepository UserCriteriaRepo { get; }

        IReportIndexRepository ReportIndexRepository { get; }

        IAssetData AssetDataRepository { get; }

        IAnnouncementRepository AnnouncementRepo { get; }

        IDataSourceRepository DataSourceRepo { get; }

        UserDTO CurrentUser { get; }
        IAttributeMetaDataRepository AttributeMetaDataRepo { get; }
        
        IExcelRawDataRepository ExcelWorksheetRepository { get; }

        void SetUser(string username);

        void AddUser(string username, bool hasAdminClaim);

        ///<summary>Commit the transaction for the database repository</summary>
        ///<remarks>Only use this when the transaction MUST occur outside the data repository</remarks>
        void Commit();

        ///<summary>Roll back the transaction for the database repository</summary>
        ///<remarks>Only use this when the transaction MUST occur outside the data repository</remarks>
        void Rollback();
    }
}
