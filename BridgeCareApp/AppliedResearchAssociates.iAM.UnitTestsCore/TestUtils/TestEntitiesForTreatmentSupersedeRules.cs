using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public static class TestEntitiesForTreatmentSupersedeRules
    {
        public static List<ScenarioBudgetEntity> ScenarioBudgetEntities => new List<ScenarioBudgetEntity>()
        {
            new ScenarioBudgetEntity()
            {
                Id = TestDataForTreatmentSupersedeRules.InterstateBudgetId,
                Name = TestDataForTreatmentSupersedeRules.InterstateBudgetName,
                ScenarioBudgetAmounts = new List<ScenarioBudgetAmountEntity>()
                {
                    new ScenarioBudgetAmountEntity() { Year = 2022, Value = 10000000 },
                    new ScenarioBudgetAmountEntity() { Year = 2023, Value = 10000000 }
                }
            },
            new ScenarioBudgetEntity()
            {
                Id = TestDataForTreatmentSupersedeRules.LocalBudgetId,
                Name = TestDataForTreatmentSupersedeRules.LocalBudgetName,
                ScenarioBudgetAmounts = new List<ScenarioBudgetAmountEntity>()
                {
                    new ScenarioBudgetAmountEntity() { Year = 2022, Value = 5000000 },
                    new ScenarioBudgetAmountEntity() { Year = 2023, Value = 3000000 }
                }
            }
        };

        public static List<AttributeEntity> AttribureEntities => new List<AttributeEntity>()
        {
            new AttributeEntity()
            {
               Id = Guid.Parse("c31ea5bb-3d48-45bb-a68f-01ee75f17f01"),
                Name = "BRKEY_",
                DataType = "NUMBER"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("cbdc2aac-f2b7-405e-8ff8-21f2785330c2"),
                Name = "BMSID",
                DataType = "STRING"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("67abf485-b3bc-4899-b492-f9165b571043"),
                Name = "DECK_SEEDED",
                DataType = "NUMBER"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("fb86603f-7bc5-4e29-b643-cd739ef065e4"),
                Name = "SUP_SEEDED",
                DataType = "NUMBER"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("cea45b74-f6c2-4e5c-8d2c-3102a85bf335"),
                Name = "SUB_SEEDED",
                DataType = "NUMBER"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("e276df35-a96b-4bdd-b57b-31236a0ddbc6"),
                Name = "CULV_SEEDED",
                DataType = "NUMBER"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("a0c921cc-c40a-41e4-a1d6-33f810397ab7"),
                Name = "DECK_DURATION_N",
                DataType = "NUMBER"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("8f81bdf8-f492-40d9-b790-f87ad7de26a8"),
                Name = "SUP_DURATION_N",
                DataType = "NUMBER"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("eb6d72ec-5801-4ec0-bd8b-c5641c291f59"),
                Name = "SUB_DURATION_N",
                DataType = "NUMBER"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("f7693cfe-8705-4f58-8d16-9be6e5d9a2a0"),
                Name = "CULV_DURATION_N",
                DataType = "NUMBER"
            }
        };

        private static Guid NewGuid => Guid.NewGuid();

        public static SimulationEntity GoodTestSimulation() =>
            new SimulationEntity()
            {
                Id = TestDataForTreatmentSupersedeRules.SimulationId,
                Name = "Test",
                InvestmentPlan = new InvestmentPlanEntity()
                {
                    Id = Guid.Parse("ad1e1f67-486f-409a-b532-b03d7eb4b1c7"),
                    SimulationId = TestDataForTreatmentSupersedeRules.SimulationId,
                    FirstYearOfAnalysisPeriod = 2022,
                    InflationRatePercentage = 3,
                    MinimumProjectCostLimit = 1000,
                    NumberOfYearsInAnalysisPeriod = 3
                },
                Budgets = ScenarioBudgetEntities,
                NetworkId = TestDataForTreatmentSupersedeRules.NetworkId,
                Network = new NetworkEntity()
                {
                    Id = TestDataForTreatmentSupersedeRules.NetworkId,
                    Name = TestDataForTreatmentSupersedeRules.NetworkName,
                    KeyAttributeId = Guid.Parse("c31ea5bb-3d48-45bb-a68f-01ee75f17f0c")
                },
                CashFlowRules = new List<DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CashFlow.ScenarioCashFlowRuleEntity>(),
                SelectableTreatments = ScenarioTreatments
            };

        public static List<ScenarioSelectableTreatmentEntity> ScenarioTreatments => new List<ScenarioSelectableTreatmentEntity> { ScenarioTreatmentWithSupersedeRules(), ScenarioPreventTreatment() };

        public static SelectableTreatmentEntity SelectablePreventTreatment()
        {
            return SelectableTreatment("Prevent Treatment", TestDataForTreatmentSupersedeRules.PreventTreatmentId);
        }

        private static ScenarioSelectableTreatmentEntity ScenarioPreventTreatment()
        {
            return ScenarioTreatment("Prevent Treatment", TestDataForTreatmentSupersedeRules.PreventTreatmentId);
        }

        private static ScenarioSelectableTreatmentEntity ScenarioTreatmentWithSupersedeRules()
        {
            var testTreatmentWithRulesId = TestDataForTreatmentSupersedeRules.TreatmentId;
            var scenarioSelectableTreatment = ScenarioTreatment("TestTreatmentWithRules", testTreatmentWithRulesId);
            var preventTreatment = ScenarioPreventTreatment();

            // Add supersede rules
            var criterionLibrary = CreateCriterionLibrary();
            var rule = new ScenarioTreatmentSupersedeRuleEntity
            {
                Id = NewGuid,
                PreventTreatmentId = preventTreatment.Id,
                TreatmentId = testTreatmentWithRulesId,
                CriterionLibraryScenarioTreatmentSupersedeRuleJoin = new CriterionLibraryScenarioTreatmentSupersedeRuleEntity
                {
                    CriterionLibrary = criterionLibrary,
                    CriterionLibraryId = criterionLibrary.Id
                }
            };
            scenarioSelectableTreatment.ScenarioTreatmentSupersedeRules.Add(rule);

            scenarioSelectableTreatment.ScenarioSelectableTreatmentScenarioBudgetJoins = new List<ScenarioSelectableTreatmentScenarioBudgetEntity> { new ScenarioSelectableTreatmentScenarioBudgetEntity { ScenarioBudget = new ScenarioBudgetEntity { Id = NewGuid, BudgetOrder = 1, Name = "Test Budget" } } };

            return scenarioSelectableTreatment;
        }

        private static ScenarioSelectableTreatmentEntity ScenarioTreatment(string name, Guid treatmentId)
        {
            var equation = new EquationEntity
            {
                Expression = "100",
            };
            var equationJoin = new ScenarioTreatmentCostEquationEntity
            {
                Equation = equation,
                ScenarioTreatmentCostId = TestDataForTreatmentSupersedeRules.CostId,
            };
            var scenarioTreatmentCost = new ScenarioTreatmentCostEntity
            {
                Id = TestDataForTreatmentSupersedeRules.CostId,
                ScenarioTreatmentCostEquationJoin = equationJoin,
                ScenarioSelectableTreatmentId = treatmentId,
            };
            var costs = new List<ScenarioTreatmentCostEntity> { scenarioTreatmentCost };            
            var treatmentJoin = new CriterionLibraryScenarioSelectableTreatmentEntity { CriterionLibrary = CreateCriterionLibrary() };
            var entity = new ScenarioSelectableTreatmentEntity
            {
                Id = treatmentId,                
                Description = "Test description",
                Name = name,
                SimulationId = TestDataForTreatmentSupersedeRules.SimulationId,
                ScenarioTreatmentCosts = costs,
                ScenarioTreatmentConsequences = new List<ScenarioConditionalTreatmentConsequenceEntity>(),
                ScenarioSelectableTreatmentScenarioBudgetJoins = new List<ScenarioSelectableTreatmentScenarioBudgetEntity>(),
                CriterionLibraryScenarioSelectableTreatmentJoin = treatmentJoin,
                ScenarioTreatmentSupersedeRules = new List<ScenarioTreatmentSupersedeRuleEntity>()
            };

            return entity;
        }

        public static ScenarioTreatmentSupersedeRuleEntity ScenarioTreatmentSupersedeRule(Guid treatmentId, ScenarioSelectableTreatmentEntity preventTreatment)
        {
            var criterionLibrary = CreateCriterionLibrary();
            var entity = new ScenarioTreatmentSupersedeRuleEntity
            {
                Id = NewGuid,
                PreventTreatmentId = preventTreatment.Id,
                TreatmentId = treatmentId,
                CriterionLibraryScenarioTreatmentSupersedeRuleJoin = new CriterionLibraryScenarioTreatmentSupersedeRuleEntity
                {
                    CriterionLibrary = criterionLibrary,
                    CriterionLibraryId = criterionLibrary.Id
                }
            };

            return entity;
        }

        public static TreatmentSupersedeRuleEntity TreatmentSupersedeRule(Guid treatmentId, SelectableTreatmentEntity preventTreatment)
        {
            var criterionLibrary = CreateCriterionLibrary();
            var entity = new TreatmentSupersedeRuleEntity
            {
                Id = NewGuid,
                PreventTreatmentId = preventTreatment.Id,
                TreatmentId = treatmentId,
                CriterionLibraryTreatmentSupersedeRuleJoin = new CriterionLibraryTreatmentSupersedeRuleEntity
                {
                    CriterionLibrary = criterionLibrary,
                    CriterionLibraryId = criterionLibrary.Id
                }
            };

            return entity;
        }

        private static CriterionLibraryEntity CreateCriterionLibrary()
        {            
            return new CriterionLibraryEntity { Id = NewGuid, MergedCriteriaExpression = "TestExpression" };
        }

        private static SelectableTreatmentEntity SelectableTreatment(string name, Guid treatmentId)
        {   
            var treatmentJoin = new CriterionLibrarySelectableTreatmentEntity { CriterionLibrary = CreateCriterionLibrary() };
            var entity = new SelectableTreatmentEntity
            {
                Id = treatmentId,
                Description = "Test description1",
                Name = name,
                TreatmentCosts = new List<TreatmentCostEntity>(),
                TreatmentConsequences = new List<ConditionalTreatmentConsequenceEntity>(),
                CriterionLibrarySelectableTreatmentJoin = treatmentJoin,
                TreatmentPerformanceFactors = new List<TreatmentPerformanceFactorEntity>(),
                TreatmentSupersedeRules = new List<TreatmentSupersedeRuleEntity>()
            };

            return entity;
        }
    }
}
