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
using AppliedResearchAssociates.iAM.DTOs.Static;

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
            if (CriterionLibraryValidityChecker.IsValid(dto.AnalysisMethod.CriterionLibrary))
            {
                var analysisMethodCriterionLibraryEntity = CriterionMapper.ToEntity(dto.AnalysisMethod.CriterionLibrary);
                var analysisMethodJoin = new CriterionLibraryAnalysisMethodEntity
                {
                    AnalysisMethodId = dto.AnalysisMethod.Id,
                    CriterionLibrary = analysisMethodCriterionLibraryEntity,
                };
                analysisMethod.CriterionLibraryAnalysisMethodJoin = analysisMethodJoin;
            }
            var investmentPlan = InvestmentPlanMapper.ToEntityNullPropagating(dto.InvestmentPlan, dto.Id);
            var reportIndexEntities = new List<ReportIndexEntity>();
            foreach (var report in dto.ReportIndexes)
            {
                var reportEntity = report.ToEntity();
                reportIndexEntities.Add(reportEntity);
            }
            var scenarioBudgetsEntities = new List<ScenarioBudgetEntity>();
            foreach (var budget in dto.Budgets)
            {
                var scenarioBudgetEntity = budget.ToScenarioEntityWithBudgetAmounts(dto.Id);
                scenarioBudgetsEntities.Add(scenarioBudgetEntity);
            }
            var scenarioCalculatedAttributeEntities = new List<ScenarioCalculatedAttributeEntity>();
            foreach (var calculatedAttribute in dto.CalculatedAttributes)
            {
                var attributeName = calculatedAttribute.Attribute;
                var attribute = attributes.FirstOrDefault(a => a.Name == attributeName);
                var scenarioCalculatedAttributeEntity = calculatedAttribute.ToScenarioEntity(dto.Id, attribute.Id);
                foreach (var equationCriterionPair in calculatedAttribute.Equations)
                {
                    var equationCriterionPairEntity = CalculatedAttributeEquationCriteriaPairMapper.ToScenarioEntity(equationCriterionPair, attribute.Id);
                    var equationEntity = EquationMapper.ToEntity(equationCriterionPair.Equation);
                    var equationJoin = new ScenarioEquationCalculatedAttributePairEntity
                    {
                        ScenarioCalculatedAttributePairId = equationCriterionPairEntity.Id,
                        Equation = equationEntity,
                    };
                    equationCriterionPairEntity.EquationCalculatedAttributeJoin = equationJoin;
                    scenarioCalculatedAttributeEntity.Equations.Add(equationCriterionPairEntity);
                }

                scenarioCalculatedAttributeEntities.Add(scenarioCalculatedAttributeEntity);
            }
            var scenarioSelectableTreatmentEntities = new List<ScenarioSelectableTreatmentEntity>();
            foreach (var treatment in dto.Treatments)
            {
                var budgetJoins = new List<ScenarioSelectableTreatmentScenarioBudgetEntity>();
                foreach (var treatmentBudget in treatment.Budgets)
                {
                    var budgetDto = dto.Budgets.FirstOrDefault(b => b.Name == treatmentBudget.Name);
                    var budgetJoin = new ScenarioSelectableTreatmentScenarioBudgetEntity
                    {
                        ScenarioSelectableTreatmentId = dto.Id,
                        ScenarioBudgetId = budgetDto.Id,

                    };
                    budgetJoins.Add(budgetJoin);
                }
                var scenarioSelectableTreatmentEntity = treatment.ToScenarioEntity(dto.Id);
                scenarioSelectableTreatmentEntity.ScenarioSelectableTreatmentScenarioBudgetJoins = budgetJoins;
                scenarioSelectableTreatmentEntities.Add(scenarioSelectableTreatmentEntity);
            }
            var scenarioTargetConditionGoalEntities = new List<ScenarioTargetConditionGoalEntity>();
            foreach (var targetConditionGoal in dto.TargetConditionGoals)
            {
                var attributeName = targetConditionGoal.Attribute;
                var attribute = attributes.FirstOrDefault(a => a.Name == attributeName);
                var scenarioTargetConditionGoalEntity = targetConditionGoal.ToScenarioEntity(dto.Id, attribute.Id);
                scenarioTargetConditionGoalEntities.Add(scenarioTargetConditionGoalEntity);
            }
            var scenarioDeficientConditionGoals = new List<ScenarioDeficientConditionGoalEntity>();
            foreach (var scenarioDeficientConditionGoal in dto.DeficientConditionGoals)
            {
                var attributeName = scenarioDeficientConditionGoal.Attribute;
                var attribute = attributes.FirstOrDefault(a => a.Name == attributeName);
                var deficientConditionGoal = scenarioDeficientConditionGoal.ToScenarioEntity(dto.Id, attribute.Id);
                scenarioDeficientConditionGoals.Add(deficientConditionGoal);
            }
            var scenarioRemainingLifeLimitEntities = new List<ScenarioRemainingLifeLimitEntity>();
            foreach (var remainingLifeLimit in dto.RemainingLifeLimits)
            {
                var attributeName = remainingLifeLimit.Attribute;
                var attribute = attributes.FirstOrDefault(a => a.Name == attributeName);
                var remainingLifeLimitEntity = remainingLifeLimit.ToScenarioEntityWithCriterionLibraryJoin(dto.Id, attribute.Id);
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
                SelectableTreatments = scenarioSelectableTreatmentEntities,
            };

            return entity;
        }

    }
}
