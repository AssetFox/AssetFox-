using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
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
            string networkKeyAttribute, BaseEntityProperties baseEntityProperties)
        {
            var analysisMethodDto = dto.AnalysisMethod;
            var attributeId = attributes.FirstOrDefault(_ => _.Name.Equals(analysisMethodDto.Attribute))?.Id;
            var analysisMethod = AnalysisMethodMapper.ToEntityWithBenefit(analysisMethodDto, dto.Id, attributes, attributeId, baseEntityProperties: baseEntityProperties);
            if (CriterionLibraryValidityChecker.IsValid(analysisMethodDto.CriterionLibrary))
            {
                var analysisMethodCriterionLibraryEntity = CriterionMapper.ToSingleUseEntity(analysisMethodDto.CriterionLibrary, baseEntityProperties);
                var analysisMethodJoin = new CriterionLibraryAnalysisMethodEntity
                {
                    AnalysisMethodId = analysisMethodDto.Id,
                    CriterionLibrary = analysisMethodCriterionLibraryEntity,
                };
                BaseEntityPropertySetter.SetBaseEntityProperties(analysisMethodJoin, baseEntityProperties);
                analysisMethod.CriterionLibraryAnalysisMethodJoin = analysisMethodJoin;
            }

            var investmentPlan = InvestmentPlanMapper.ToEntityNullPropagating(dto.InvestmentPlan, dto.Id, baseEntityProperties);
            var reportIndexEntities = new List<ReportIndexEntity>();
            foreach (var report in dto.ReportIndexes)
            {
                var reportEntity = report.ToEntity();
                reportIndexEntities.Add(reportEntity);
            }

            var scenarioBudgetsEntities = new List<ScenarioBudgetEntity>();
            foreach (var budget in dto.Budgets)
            {
                var scenarioBudgetEntity = budget.ToScenarioEntityWithBudgetAmounts(dto.Id, baseEntityProperties);
                scenarioBudgetsEntities.Add(scenarioBudgetEntity);
            }

            var scenarioCalculatedAttributeEntities = new List<ScenarioCalculatedAttributeEntity>();
            foreach (var calculatedAttribute in dto.CalculatedAttributes)
            {
                var attributeName = calculatedAttribute.Attribute;
                var attribute = attributes.FirstOrDefault(a => a.Name == attributeName);
                var scenarioCalculatedAttributeEntity = calculatedAttribute.ToScenarioEntity(dto.Id, attribute.Id, baseEntityProperties);
                foreach (var equationCriterionPair in calculatedAttribute.Equations)
                {
                    var equationCriterionPairEntity = CalculatedAttributeEquationCriteriaPairMapper.ToScenarioEntity(equationCriterionPair, attribute.Id, baseEntityProperties);
                    var equationEntity = EquationMapper.ToEntity(equationCriterionPair.Equation, baseEntityProperties);
                    var equationJoin = new ScenarioEquationCalculatedAttributePairEntity
                    {
                        ScenarioCalculatedAttributePairId = equationCriterionPairEntity.Id,
                        Equation = equationEntity,
                    };
                    equationCriterionPairEntity.EquationCalculatedAttributeJoin = equationJoin;
                    scenarioCalculatedAttributeEntity.Equations.Add(equationCriterionPairEntity);
                    var equationCriterionLibraryDto = equationCriterionPair.CriteriaLibrary;
                    if (CriterionLibraryValidityChecker.IsValid(equationCriterionLibraryDto))
                    {
                        var equationCriterionLibraryEntity = CriterionMapper.ToSingleUseEntity(equationCriterionLibraryDto, baseEntityProperties);                        
                        var criterionLibraryCalculatedAttributeJoin = new ScenarioCriterionLibraryCalculatedAttributePairEntity
                        {
                            ScenarioCalculatedAttributePairId = equationCriterionPairEntity.Id,
                            CriterionLibrary = equationCriterionLibraryEntity,

                        };
                        equationCriterionPairEntity.CriterionLibraryCalculatedAttributeJoin = criterionLibraryCalculatedAttributeJoin;
                    }
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
                    BaseEntityPropertySetter.SetBaseEntityProperties(budgetJoin, baseEntityProperties);
                    budgetJoins.Add(budgetJoin);
                }                
                var scenarioSelectableTreatmentEntity = treatment.ToScenarioEntityWithCriterionLibraryWithChildren(dto.Id, attributes, baseEntityProperties);
                scenarioSelectableTreatmentEntity.ScenarioSelectableTreatmentScenarioBudgetJoins = budgetJoins;
                scenarioSelectableTreatmentEntities.Add(scenarioSelectableTreatmentEntity);
            }           

            var scenarioTargetConditionGoalEntities = new List<ScenarioTargetConditionGoalEntity>();
            foreach (var targetConditionGoal in dto.TargetConditionGoals)
            {
                var attributeName = targetConditionGoal.Attribute;
                var attribute = attributes.FirstOrDefault(a => a.Name == attributeName);
                var scenarioTargetConditionGoalEntity = targetConditionGoal.ToScenarioEntityWithCriterionLibraryJoin(dto.Id, attribute.Id, baseEntityProperties);
                scenarioTargetConditionGoalEntities.Add(scenarioTargetConditionGoalEntity);
            }

            var scenarioDeficientConditionGoals = new List<ScenarioDeficientConditionGoalEntity>();
            foreach (var scenarioDeficientConditionGoal in dto.DeficientConditionGoals)
            {
                var attributeName = scenarioDeficientConditionGoal.Attribute;
                var attribute = attributes.FirstOrDefault(a => a.Name == attributeName);
                var deficientConditionGoal = scenarioDeficientConditionGoal.ToScenarioEntityWithCriterionLibraryJoin(dto.Id, attribute.Id, baseEntityProperties);
                scenarioDeficientConditionGoals.Add(deficientConditionGoal);
            }

            var scenarioRemainingLifeLimitEntities = new List<ScenarioRemainingLifeLimitEntity>();
            foreach (var remainingLifeLimit in dto.RemainingLifeLimits)
            {
                var attributeName = remainingLifeLimit.Attribute;
                var attribute = attributes.FirstOrDefault(a => a.Name == attributeName);
                var remainingLifeLimitEntity = remainingLifeLimit.ToScenarioEntityWithCriterionLibraryJoin(dto.Id, attribute.Id, baseEntityProperties);
                scenarioRemainingLifeLimitEntities.Add(remainingLifeLimitEntity);
            }

            var scenarioBudgetPriorityEntities = new List<ScenarioBudgetPriorityEntity>();
            foreach (var budgetPriority in dto.BudgetPriorities)
            {
                var budgetPriorityEntity = budgetPriority.ToScenarioEntityWithCriterionLibraryJoin(dto.Id, baseEntityProperties);
                scenarioBudgetPriorityEntities.Add(budgetPriorityEntity);

            }

            var scenarioCashFlowRuleEntities = new List<ScenarioCashFlowRuleEntity>();
            foreach (var cashFlowRule in dto.CashFlowRules)
            {
                var cashFlowRuleEntity = cashFlowRule.ToScenarioEntityWithCriterionLibraryJoin(dto.Id, baseEntityProperties);
                foreach (var distributionRule in cashFlowRule.CashFlowDistributionRules)
                {
                    var distributionRuleEntity = CashFlowDistributionRuleMapper.ToScenarioEntity(distributionRule, cashFlowRule.Id, baseEntityProperties);
                    cashFlowRuleEntity.ScenarioCashFlowDistributionRules.Add(distributionRuleEntity);
                }
                scenarioCashFlowRuleEntities.Add(cashFlowRuleEntity);
            }

            var committedProjectEntities = new List<CommittedProjectEntity>();
            foreach (var committedProject in dto.CommittedProjects)
            {
                var committedProjectEntity = committedProject.ToEntity(attributes, networkKeyAttribute, baseEntityProperties);
                committedProjectEntities.Add(committedProjectEntity);
            }

            var scenarioPerformanceCurves = new List<ScenarioPerformanceCurveEntity>();
            foreach (var performanceCurve in dto.PerformanceCurves)
            {
                var attributeName = performanceCurve.Attribute;
                var attribute = attributes.FirstOrDefault(a => a.Name == attributeName);
                var performanceCurveEntity = performanceCurve.ToScenarioEntityWithCriterionLibraryJoinAndEquationJoin(dto.Id, attribute.Id, baseEntityProperties);
                scenarioPerformanceCurves.Add(performanceCurveEntity);
            }

            var userJoins = new List<SimulationUserEntity>();
            foreach (var user in dto.Users)
            {
                var userJoin = user.ToEntity(dto.Id, baseEntityProperties);
                userJoins.Add(userJoin);
            }

            var simulationOutputJsonEntities = new List<SimulationOutputJsonEntity>();
            foreach(var simulationOutputJson in dto.SimulationOutputJsons)
            {
                var simulationOutputJsonEntity = simulationOutputJson.ToEntity(dto.Id);
                simulationOutputJsonEntities.Add(simulationOutputJsonEntity);
            }

            var simulationAnalysisDetail = new SimulationAnalysisDetailEntity();
            if (dto.SimulationAnalysisDetail != null)
            {
                simulationAnalysisDetail = dto.SimulationAnalysisDetail?.ToEntity();
                simulationAnalysisDetail.SimulationId = dto.Id;
            }

            var entity = new SimulationEntity
            {
                Name = dto.Name,
                Id = dto.Id,
                NetworkId = dto.NetworkId,
                NumberOfYearsOfTreatmentOutlook = 100,
                AnalysisMethod = analysisMethod,
                InvestmentPlan = investmentPlan,
                SimulationReports = reportIndexEntities,
                Budgets = scenarioBudgetsEntities,
                CalculatedAttributes = scenarioCalculatedAttributeEntities,
                ScenarioTargetConditionalGoals = scenarioTargetConditionGoalEntities,
                ScenarioDeficientConditionGoals = scenarioDeficientConditionGoals,
                PerformanceCurves = scenarioPerformanceCurves,
                RemainingLifeLimits = scenarioRemainingLifeLimitEntities,
                BudgetPriorities = scenarioBudgetPriorityEntities,
                CashFlowRules = scenarioCashFlowRuleEntities,
                CommittedProjects = committedProjectEntities,
                SelectableTreatments = scenarioSelectableTreatmentEntities,
                SimulationUserJoins = userJoins,
                SimulationOutputJsons = simulationOutputJsonEntities,
                SimulationAnalysisDetail = simulationAnalysisDetail
            };
            BaseEntityPropertySetter.SetBaseEntityProperties(entity, baseEntityProperties);
            return entity;
        }
    }
}
