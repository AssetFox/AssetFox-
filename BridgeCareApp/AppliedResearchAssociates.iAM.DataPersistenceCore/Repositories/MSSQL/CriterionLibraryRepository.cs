﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using MoreLinq.Extensions;


namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class CriterionLibraryRepository : MSSQLRepository, ICriterionLibraryRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private static readonly Dictionary<string, string> NameConventionPerEntityType = new Dictionary<string, string>
        {
            {"AnalysisMethodEntity", "Analysis Method"},
            {"BudgetEntity", "Budget"},
            {"BudgetPriorityEntity", "Budget Priority"},
            {"DeficientConditionGoalEntity", "Deficient Condition Goal"},
            {"PerformanceCurveEntity", "Performance Curve"},
            {"CashFlowRuleEntity", "Cash Flow Rule"},
            {"RemainingLifeLimitEntity", "Remaining Life Limit"},
            {"SelectableTreatmentEntity", "Feasibility"},
            {"TargetConditionGoalEntity", "Target Condition Goal"},
            {"TreatmentConsequenceEntity", "Treatment Consequence"},
            {"TreatmentCostEntity", "Treatment Cost"},
            {"TreatmentSupersessionEntity", "Treatment Supersession"}
        };
        public CriterionLibraryRepository(IAMContext context) : base(context) { }

        public void JoinEntitiesWithCriteria(Dictionary<string, List<Guid>> entityIdsPerExpression, string joinEntity, string prependName)
        {
            entityIdsPerExpression.Keys.ForEach(expression =>
            {
                var criterionLibraryEntity = Context.CriterionLibrary
                    .SingleOrDefault(_ => _.MergedCriteriaExpression == expression &&
                                          _.Name.Contains(NameConventionPerEntityType[joinEntity]));

                if (criterionLibraryEntity == null)
                {
                    var criterionLibraryNames = Context.CriterionLibrary
                        .Where(_ => _.Name.Contains(NameConventionPerEntityType[joinEntity]))
                        .Select(_ => _.Name).ToList();

                    var newCriterionLibraryName = $"{prependName} {NameConventionPerEntityType[joinEntity]} Criterion Library";
                    if (criterionLibraryNames.Contains(newCriterionLibraryName))
                    {
                        var version = 2;
                        while (criterionLibraryNames.Contains($"{newCriterionLibraryName} v{version}"))
                        {
                            version++;
                        }
                        newCriterionLibraryName = $"{newCriterionLibraryName} v{version}";
                    }

                    criterionLibraryEntity = new CriterionLibraryEntity
                    {
                        Id = Guid.NewGuid(), Name = newCriterionLibraryName, MergedCriteriaExpression = expression
                    };

                    Context.CriterionLibrary.Add(criterionLibraryEntity);
                }

                switch (joinEntity)
                {
                case "AnalysisMethodEntity":
                    CreateCriterionLibraryAnalysisMethodJoin(criterionLibraryEntity.Id, entityIdsPerExpression[expression].First());
                    break;
                case "BudgetEntity":
                    CreateCriterionLibraryBudgetJoins(criterionLibraryEntity.Id, entityIdsPerExpression[expression]);
                    break;
                case "BudgetPriorityEntity":
                    CreateCriterionLibraryBudgetPriorityJoins(criterionLibraryEntity.Id, entityIdsPerExpression[expression]);
                    break;
                case "CashFlowRuleEntity":
                    CreateCriterionLibraryCashFlowRuleJoins(criterionLibraryEntity.Id, entityIdsPerExpression[expression]);
                    break;
                case "DeficientConditionGoalEntity":
                    CreateCriterionLibraryDeficientConditionGoalJoins(criterionLibraryEntity.Id, entityIdsPerExpression[expression]);
                    break;
                case "PerformanceCurveEntity":
                    CreateCriterionLibraryPerformanceCurveJoins(criterionLibraryEntity.Id, entityIdsPerExpression[expression]);
                    break;
                case "RemainingLifeLimitEntity":
                    CreateCriterionLibraryRemainingLifeLimitJoins(criterionLibraryEntity.Id, entityIdsPerExpression[expression]);
                    break;
                case "TargetConditionGoalEntity":
                    CreateCriterionLibraryTargetConditionGoalJoins(criterionLibraryEntity.Id, entityIdsPerExpression[expression]);
                    break;
                case "TreatmentConsequenceEntity":
                    CreateCriterionLibraryTreatmentConsequenceJoins(criterionLibraryEntity.Id, entityIdsPerExpression[expression]);
                    break;
                case "TreatmentCostEntity":
                    CreateCriterionLibraryTreatmentCostJoins(criterionLibraryEntity.Id, entityIdsPerExpression[expression]);
                    break;
                case "TreatmentSupersessionEntity":
                    CreateCriterionLibraryTreatmentSupersessionJoins(criterionLibraryEntity.Id, entityIdsPerExpression[expression]);
                    break;
                default:
                    throw new InvalidOperationException("Unable to determine criterion library join entity type.");
                }
            });
            
            Context.SaveChanges();
        }

        public void CreateCriterionLibraries(List<CriterionLibraryEntity> criterionLibraryEntities)
        {
            if (IsRunningFromXUnit)
            {
                criterionLibraryEntities.ForEach(entity => Context.AddOrUpdate(entity, entity.Id));
            }
            else
            {
                Context.BulkInsertOrUpdate(criterionLibraryEntities);
            }

            Context.SaveChanges();
        }

        private void CreateCriterionLibraryAnalysisMethodJoin(Guid criterionLibraryId, Guid analysisMethodId)
        {
            var joinEntity = new CriterionLibraryAnalysisMethodEntity
            {
                CriterionLibraryId = criterionLibraryId, AnalysisMethodId = analysisMethodId
            };

            Context.CriterionLibraryAnalysisMethod.Add(joinEntity);
        }

        private void CreateCriterionLibraryBudgetJoins(Guid criterionLibraryId, List<Guid> budgetIds)
        {
            var joinEntities = budgetIds.Select(budgetId =>
                    new CriterionLibraryBudgetEntity
                    {
                        CriterionLibraryId = criterionLibraryId,
                        BudgetId = budgetId
                    })
                .ToList();

            if (IsRunningFromXUnit)
            {
                Context.CriterionLibraryBudget.AddRange(joinEntities);
            }
            else
            {
                Context.BulkInsert(joinEntities);
            }
        }

        private void CreateCriterionLibraryBudgetPriorityJoins(Guid criterionLibraryId, List<Guid> budgetPriorityIds)
        {
            var joinEntities = budgetPriorityIds.Select(budgetPriorityId =>
                    new CriterionLibraryBudgetPriorityEntity
                    {
                        CriterionLibraryId = criterionLibraryId,
                        BudgetPriorityId = budgetPriorityId
                    })
                .ToList();

            if (IsRunningFromXUnit)
            {
                Context.CriterionLibraryBudgetPriority.AddRange(joinEntities);
            }
            else
            {
                Context.BulkInsert(joinEntities);
            }
        }

        private void CreateCriterionLibraryCashFlowRuleJoins(Guid criterionLibraryId, List<Guid> cashFlowRuleIds)
        {
            var joinEntities = cashFlowRuleIds.Select(cashFlowRuleId =>
                    new CriterionLibraryCashFlowRuleEntity
                    {
                        CriterionLibraryId = criterionLibraryId,
                        CashFlowRuleId = cashFlowRuleId
                    })
                .ToList();

            if (IsRunningFromXUnit)
            {
                Context.CriterionLibraryCashFlowRule.AddRange(joinEntities);
            }
            else
            {
                Context.BulkInsert(joinEntities);
            }
        }

        private void CreateCriterionLibraryDeficientConditionGoalJoins(Guid criterionLibraryId, List<Guid> deficientConditionGoalIds)
        {
            var joinEntities = deficientConditionGoalIds.Select(deficientConditionGoalId =>
                    new CriterionLibraryDeficientConditionGoalEntity
                    {
                        CriterionLibraryId = criterionLibraryId,
                        DeficientConditionGoalId = deficientConditionGoalId
                    })
                .ToList();

            if (IsRunningFromXUnit)
            {
                Context.CriterionLibraryDeficientConditionGoal.AddRange(joinEntities);
            }
            else
            {
                Context.BulkInsert(joinEntities);
            }
        }

        private void CreateCriterionLibraryPerformanceCurveJoins(Guid criterionLibraryId, List<Guid> performanceCurveIds)
        {
            var joinEntities = performanceCurveIds.Select(performanceCurveId =>
                new CriterionLibraryPerformanceCurveEntity
                {
                    CriterionLibraryId = criterionLibraryId, PerformanceCurveId = performanceCurveId
                })
                .ToList();

            if (IsRunningFromXUnit)
            {
                Context.CriterionLibraryPerformanceCurve.AddRange(joinEntities);
            }
            else
            {
                Context.BulkInsert(joinEntities);
            }
        }

        private void CreateCriterionLibraryRemainingLifeLimitJoins(Guid criterionLibraryId, List<Guid> remainingLifeLimitIds)
        {
            var joinEntities = remainingLifeLimitIds.Select(remainingLifeLimitId =>
                    new CriterionLibraryRemainingLifeLimitEntity
                    {
                        CriterionLibraryId = criterionLibraryId,
                        RemainingLifeLimitId = remainingLifeLimitId
                    })
                .ToList();

            if (IsRunningFromXUnit)
            {
                Context.CriterionLibraryRemainingLifeLimit.AddRange(joinEntities);
            }
            else
            {
                Context.BulkInsert(joinEntities);
            }
        }

        private void CreateCriterionLibraryTargetConditionGoalJoins(Guid criterionLibraryId, List<Guid> targetConditionGoalIds)
        {
            var joinEntities = targetConditionGoalIds.Select(targetConditionGoalId =>
                    new CriterionLibraryTargetConditionGoalEntity
                    {
                        CriterionLibraryId = criterionLibraryId,
                        TargetConditionGoalId = targetConditionGoalId
                    })
                .ToList();

            if (IsRunningFromXUnit)
            {
                Context.CriterionLibraryTargetConditionGoal.AddRange(joinEntities);
            }
            else
            {
                Context.BulkInsert(joinEntities);
            }
        }

        private void CreateCriterionLibraryTreatmentConsequenceJoins(Guid criterionLibraryId, List<Guid> consequenceIds)
        {
            var joinEntities = consequenceIds.Select(consequenceId =>
                    new CriterionLibraryConditionalTreatmentConsequenceEntity
                    {
                        CriterionLibraryId = criterionLibraryId,
                        ConditionalTreatmentConsequenceId = consequenceId
                    })
                .ToList();

            if (IsRunningFromXUnit)
            {
                Context.CriterionLibraryTreatmentConsequence.AddRange(joinEntities);
            }
            else
            {
                Context.BulkInsert(joinEntities);
            }
        }

        private void CreateCriterionLibraryTreatmentCostJoins(Guid criterionLibraryId, List<Guid> costIds)
        {
            var joinEntities = costIds.Select(costId =>
                    new CriterionLibraryTreatmentCostEntity
                    {
                        CriterionLibraryId = criterionLibraryId,
                        TreatmentCostId = costId
                    })
                .ToList();

            if (IsRunningFromXUnit)
            {
                Context.CriterionLibraryTreatmentCost.AddRange(joinEntities);
            }
            else
            {
                Context.BulkInsert(joinEntities);
            }
        }

        private void CreateCriterionLibraryTreatmentSupersessionJoins(Guid criterionLibraryId, List<Guid> supersessionIds)
        {
            var joinEntities = supersessionIds.Select(supersessionId =>
                    new CriterionLibraryTreatmentSupersessionEntity
                    {
                        CriterionLibraryId = criterionLibraryId,
                        TreatmentSupersessionId = supersessionId
                    })
                .ToList();

            if (IsRunningFromXUnit)
            {
                Context.CriterionLibraryTreatmentSupersession.AddRange(joinEntities);
            }
            else
            {
                Context.BulkInsert(joinEntities);
            }
        }

        public void JoinSelectableTreatmentEntitiesWithCriteria(
            Dictionary<Guid, List<string>> expressionsPerSelectableTreatmentEntityId, string simulationName)
        {
            var criterionLibraryEntities = new List<CriterionLibraryEntity>();
            var criterionLibraryIdsPerSelectableTreatmentId = new Dictionary<Guid, List<Guid>>();

            expressionsPerSelectableTreatmentEntityId.Keys.ForEach(entityId =>
            {
                var criterionLibraryEntityIds = new List<Guid>();

                expressionsPerSelectableTreatmentEntityId[entityId].ForEach(expression =>
                {
                    var criterionLibraryEntity = Context.CriterionLibrary
                        .SingleOrDefault(_ => _.MergedCriteriaExpression == expression &&
                                              _.Name.Contains(NameConventionPerEntityType["SelectableTreatmentEntity"]));

                    if (criterionLibraryEntity == null)
                    {
                        var criterionLibraryNames = Context.CriterionLibrary
                            .Where(_ => _.Name.Contains(NameConventionPerEntityType["SelectableTreatmentEntity"]))
                            .Select(_ => _.Name).ToList();

                        var newCriterionLibraryName = $"{simulationName} Simulation {NameConventionPerEntityType["SelectableTreatmentEntity"]} Criterion Library";
                        if (criterionLibraryNames.Contains(newCriterionLibraryName))
                        {
                            var version = 2;
                            while (criterionLibraryNames.Contains($"{newCriterionLibraryName} v{version}"))
                            {
                                version++;
                            }
                            newCriterionLibraryName = $"{newCriterionLibraryName} v{version}";
                        }

                        criterionLibraryEntity = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            Name = newCriterionLibraryName,
                            MergedCriteriaExpression = expression
                        };

                        criterionLibraryEntities.Add(criterionLibraryEntity);
                    }

                    criterionLibraryEntityIds.Add(criterionLibraryEntity.Id);
                });

                criterionLibraryIdsPerSelectableTreatmentId.Add(entityId, criterionLibraryEntityIds);
            });

            if (criterionLibraryEntities.Any())
            {
                if (IsRunningFromXUnit)
                {
                    Context.CriterionLibrary.AddRange(criterionLibraryEntities);
                }
                else
                {
                    Context.BulkInsert(criterionLibraryEntities);
                }
            }

            CreateCriterionLibrarySelectableTreatmentJoins(criterionLibraryIdsPerSelectableTreatmentId);

            Context.SaveChanges();
        }

        private void CreateCriterionLibrarySelectableTreatmentJoins(Dictionary<Guid, List<Guid>> criterionLibraryIdsPerSelectableTreatmentIds)
        {
            var joinEntities = criterionLibraryIdsPerSelectableTreatmentIds
                .SelectMany(_ => _.Value
                    .Select(criterionLibraryId => new CriterionLibrarySelectableTreatmentEntity
                    {
                        CriterionLibraryId = criterionLibraryId,
                        SelectableTreatmentId = _.Key
                    })
                )
                .ToList();

            if (IsRunningFromXUnit)
            {
                Context.CriterionLibrarySelectableTreatment.AddRange(joinEntities);
            }
            else
            {
                Context.BulkInsert(joinEntities);
            }
        }
    }
}
