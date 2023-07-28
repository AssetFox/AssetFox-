using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.TargetConditionGoal;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.BudgetPriority;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CashFlow;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.TargetConditionGoal;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class CompleteSimulationMapper
    {
        public static SimulationEntity ToNewEntity(
            this CompleteSimulationDTO dto,
            List<AttributeEntity> attributes,
            string networkKeyAttribute)
        {
            var analysisMethod = AnalysisMethodMapper.ToEntity(dto.AnalysisMethod, dto.Id);
            var investmentPlan = InvestmentPlanMapper.ToEntity(dto.InvestmentPlan, dto.Id);
            var reportIndexEntities = new List<ReportIndexEntity>();
            foreach (var report in dto.ReportIndexes)
            {
                var reportEntity = report.ToEntity();
                reportIndexEntities.Add(reportEntity);
            }
            var scenarioBudgetsEntities = new List<ScenarioBudgetEntity>();
            foreach (var budget in dto.Budgets)
            {
                var scenarioBudgetEntity = budget.ToScenarioEntity(dto.Id);
                scenarioBudgetsEntities.Add(scenarioBudgetEntity);
            }
            var scenarioCalculatedAttributeEntities = new List<ScenarioCalculatedAttributeEntity>();
            foreach (var calculatedAttribute in dto.CalculatedAttributes)
            {
                var scenarioCalculatedAtrributeEntity = calculatedAttribute.ToScenarioEntity(dto.Id, dto.Id);
                scenarioCalculatedAttributeEntities.Add(scenarioCalculatedAtrributeEntity);
            }            
            var scenarioSelectableTreatmentEntities = new List<ScenarioSelectableTreatmentEntity>();
            foreach (var treatment in dto.Treatments)
            {
                var scenarioSelectableTreatmentEntity = treatment.ToScenarioEntity(dto.Id);
                scenarioSelectableTreatmentEntities.Add(scenarioSelectableTreatmentEntity);
            }
            var scenarioTargetConditionGoalEntities = new List<ScenarioTargetConditionGoalEntity>();
            foreach (var targetConditionGoal in dto.TargetConditionGoals)
            {
                var scenarioTargetConditionGoalEntity = targetConditionGoal.ToScenarioEntity(dto.Id, dto.Id);
                scenarioTargetConditionGoalEntities.Add(scenarioTargetConditionGoalEntity);
            }
            var scenarioDeficientConditionGoals = new List<ScenarioDeficientConditionGoalEntity>();
            foreach (var scenarioDeficientConditionGoal in dto.DeficientConditionGoals)
            {
                var deficientConditionGoal = scenarioDeficientConditionGoal.ToScenarioEntity(dto.Id, dto.Id);
                scenarioDeficientConditionGoals.Add(deficientConditionGoal);
            }
            var scenarioRemainingLifeLimitEntities = new List<ScenarioRemainingLifeLimitEntity>();
            foreach (var remainingLifeLimit in dto.RemainingLifeLimits)
            {
                var remainingLifeLimitEntity = remainingLifeLimit.ToScenarioEntity(dto.Id, dto.Id);
                scenarioRemainingLifeLimitEntities.Add(remainingLifeLimitEntity);
            }
            var scenarioBudgetPriorityEntities = new List<ScenarioBudgetPriorityEntity>();
            foreach (var budgetPriority in dto.BudgetPriorities)
            {
                var budgetPriorityEntity = budgetPriority.ToScenarioEntity(dto.Id);
                scenarioBudgetPriorityEntities.Add(budgetPriorityEntity);
            }
            var scenarioCashFlowRuleEntities = new List<ScenarioCashFlowRuleEntity>();
            foreach (var cashFlowRule in dto.CashFlowRules)
            {
                var cashFlowRuleEntity = cashFlowRule.ToScenarioEntity(dto.Id);
                scenarioCashFlowRuleEntities.Add(cashFlowRuleEntity);
            }
            var committedProjectEntities = new List<CommittedProjectEntity>();

            foreach (var committedProject in dto.CommittedProjects)
            {
                var committedProjectEntity = committedProject.ToEntity(attributes, networkKeyAttribute);
                committedProjectEntities.Add(committedProjectEntity);
            }

            var entity = new SimulationEntity
            {
                Name = dto.Name,
                Id = dto.Id,
                NetworkId = dto.NetworkId,
                AnalysisMethod = analysisMethod,
                InvestmentPlan = investmentPlan,
                SimulationReports = reportIndexEntities,
                Budgets = scenarioBudgetsEntities,
                CalculatedAttributes = scenarioCalculatedAttributeEntities,
                ScenarioTargetConditionalGoals = scenarioTargetConditionGoalEntities,
                ScenarioDeficientConditionGoals = scenarioDeficientConditionGoals,
                RemainingLifeLimits = scenarioRemainingLifeLimitEntities,
                BudgetPriorities = scenarioBudgetPriorityEntities,
                CashFlowRules = scenarioCashFlowRuleEntities,
                CommittedProjects = committedProjectEntities,
            };

            return entity;
        }

    }
}
