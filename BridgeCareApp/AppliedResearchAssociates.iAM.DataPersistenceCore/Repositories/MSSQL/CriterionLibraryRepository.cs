using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
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
            {"BudgetEntity", "Budget"},
            {"PerformanceCurveEntity", "Performance Curve"},
            {"CashFlowRuleEntity", "Cash Flow Rule"},
            {"SelectableTreatmentEntity", "Feasibility"},
            {"TreatmentConsequenceEntity", "Treatment Consequence"},
            {"TreatmentCostEntity", "Treatment Cost"},
            {"TreatmentSupersessionEntity", "Treatment Supersession"}
        };
        public CriterionLibraryRepository(IAMContext context) : base(context) { }

        public void JoinEntitiesWithCriteria(Dictionary<string, List<Guid>> entityIdsPerExpression, string joinEntity, string simulationName)
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

                    var newCriterionLibraryName = $"{simulationName} Simulation {NameConventionPerEntityType[joinEntity]} Criterion Library";
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
                case "BudgetEntity":
                    CreateCriterionLibraryBudgetJoins(criterionLibraryEntity.Id, entityIdsPerExpression[expression]);
                    break;
                case "CashFlowRuleEntity":
                    CreateCriterionLibraryCashFlowRuleJoins(criterionLibraryEntity.Id, entityIdsPerExpression[expression]);
                    break;
                case "PerformanceCurveEntity":
                    CreateCriterionLibraryPerformanceCurveJoins(criterionLibraryEntity.Id, entityIdsPerExpression[expression]);
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
                }
            });
            
            Context.SaveChanges();
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

        private void CreateCriterionLibraryTreatmentConsequenceJoins(Guid criterionLibraryId, List<Guid> consequenceIds)
        {
            var joinEntities = consequenceIds.Select(consequenceId =>
                    new CriterionLibraryTreatmentConsequenceEntity
                    {
                        CriterionLibraryId = criterionLibraryId,
                        TreatmentConsequenceId = consequenceId
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
