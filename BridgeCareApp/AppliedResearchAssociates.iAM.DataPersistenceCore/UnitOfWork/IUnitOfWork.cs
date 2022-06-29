﻿using System;
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

        IAMContext Context { get; } // This needs to go ASAP

        void BeginTransaction(); // This needs to go ASAP

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

        ICriterionLibraryRepository CriterionLibraryRepo { get; }

        IDeficientConditionGoalRepository DeficientConditionGoalRepo { get; }

        IInvestmentPlanRepository InvestmentPlanRepo { get; }

        IMaintainableAssetRepository MaintainableAssetRepo { get; }

        INetworkRepository NetworkRepo { get; }

        IPerformanceCurveRepository PerformanceCurveRepo { get; }

        ICalculatedAttributesRepository CalculatedAttributeRepo { get; }

        IRemainingLifeLimitRepository RemainingLifeLimitRepo { get; }

        ISelectableTreatmentRepository SelectableTreatmentRepo { get; }

        ISimulationAnalysisDetailRepository SimulationAnalysisDetailRepo { get; }

        ISimulationLogRepository SimulationLogRepo { get; }

        ISimulationOutputRepository SimulationOutputRepo { get; }

        ISimulationRepository SimulationRepo { get; }

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

        void SetUser(string username);

        void AddUser(string username, string role);

        void Commit();

        void Rollback();
    }
}
