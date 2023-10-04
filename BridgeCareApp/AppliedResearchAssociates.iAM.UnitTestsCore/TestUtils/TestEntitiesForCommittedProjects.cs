using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public static class TestEntitiesForCommittedProjects
    {
        public static List<ScenarioBudgetEntity> ScenarioBudgetEntities => new List<ScenarioBudgetEntity>()
        {
            new ScenarioBudgetEntity()
            {
                Id = TestDataForCommittedProjects.InterstateBudgetId,
                Name = TestDataForCommittedProjects.InterstateBudgetName,
                ScenarioBudgetAmounts = new List<ScenarioBudgetAmountEntity>()
                {
                    new ScenarioBudgetAmountEntity() { Year = 2022, Value = 10000000 },
                    new ScenarioBudgetAmountEntity() { Year = 2023, Value = 10000000 }
                }
            },
            new ScenarioBudgetEntity()
            {
                Id = TestDataForCommittedProjects.LocalBudgetId,
                Name = TestDataForCommittedProjects.LocalBudgetName,
                ScenarioBudgetAmounts = new List<ScenarioBudgetAmountEntity>()
                {
                    new ScenarioBudgetAmountEntity() { Year = 2022, Value = 5000000 },
                    new ScenarioBudgetAmountEntity() { Year = 2023, Value = 3000000 }
                }
            }
        };

        private static SimulationEntity GoodTestSimulation() =>
            new SimulationEntity()
            {
                Id = TestDataForCommittedProjects.SimulationId,
                Name = "Test",
                InvestmentPlan = new InvestmentPlanEntity()
                {
                    Id = Guid.Parse("ad1e1f67-486f-409a-b532-b03d7eb4b1c7"),
                    SimulationId = TestDataForCommittedProjects.SimulationId,
                    FirstYearOfAnalysisPeriod = 2022,
                    InflationRatePercentage = 3,
                    MinimumProjectCostLimit = 1000,
                    NumberOfYearsInAnalysisPeriod = 3
                },
                Budgets = ScenarioBudgetEntities,
                NetworkId = TestDataForCommittedProjects.NetworkId,
                Network = new NetworkEntity()
                {
                    Id = TestDataForCommittedProjects.NetworkId,
                    Name = TestDataForCommittedProjects.NetworkName,
                    KeyAttributeId = Guid.Parse("c31ea5bb-3d48-45bb-a68f-01ee75f17f0c")
                },
                CashFlowRules = new List<DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CashFlow.ScenarioCashFlowRuleEntity>()
            };

        private static CommittedProjectEntity SomethingEntity(Guid id, Guid simulationId, int year) => new CommittedProjectEntity()
        {
            Id = id,
            Year = year,
            Name = "Something",
            ShadowForAnyTreatment = 1,
            ShadowForSameTreatment = 1,
            ProjectSource = "Committed",
            Cost = 10000,
            SimulationId = simulationId,
            ScenarioBudgetId = ScenarioBudgetEntities.Single(_ => _.Name == "Local").Id,
            ScenarioBudget = ScenarioBudgetEntities.Single(_ => _.Name == "Local"),
            CommittedProjectLocation = new CommittedProjectLocationEntity(Guid.NewGuid(), DataPersistenceCore.DataPersistenceConstants.SectionLocation, "1"),
        };

        private static CommittedProjectEntity SomethingFourYear2023() =>
            SomethingEntity(Guid.Parse("444e66df-4436-49b1-ae68-9f5c10656b1b"), TestDataForCommittedProjects.FourYearSimulationId, 2023);

        private static CommittedProjectEntity SomethingFourYear2025() =>
            SomethingEntity(Guid.Parse("4e9e66df-4436-49b1-ae68-9f5c10656b1b"), TestDataForCommittedProjects.FourYearSimulationId, 2025);

        public static List<CommittedProjectEntity> CommittedProjectEntities => new List<CommittedProjectEntity>()
        {
            SomethingEntity(Guid.Parse("2e9e66df-4436-49b1-ae68-9f5c10656b1b"), TestDataForCommittedProjects.SimulationId, 2022),
            SomethingFourYear2025(),
            SimpleEntity(),
        };

        private static CommittedProjectEntity SimpleEntity() => new CommittedProjectEntity()
        {
            Id = Guid.Parse("091001e2-c1f0-4af6-90e7-e998bbea5d00"),
            Year = 2023,
            Name = "Simple",
            ShadowForAnyTreatment = 1,
            ShadowForSameTreatment = 3,
            Cost = 200000,
            SimulationId = TestDataForCommittedProjects.SimulationId,
            ScenarioBudgetId = ScenarioBudgetEntities.Single(_ => _.Name == "Interstate").Id,
            ScenarioBudget = ScenarioBudgetEntities.Single(_ => _.Name == "Interstate"),
            CommittedProjectLocation = new CommittedProjectLocationEntity(Guid.NewGuid(), DataPersistenceCore.DataPersistenceConstants.SectionLocation, "2"),
        };
        public static List<CommittedProjectEntity> CommittedProjectsWithoutBudgets()
        {
            var newProjects = CommittedProjectEntities;
            newProjects.ForEach(entity =>
            {
                entity.ScenarioBudget = null;
                entity.ScenarioBudgetId = null;
            });
            return newProjects;
        }


        public static List<MaintainableAssetEntity> MaintainableAssetEntities => new List<MaintainableAssetEntity>()
        {
            new MaintainableAssetEntity() {
                Id = Guid.Parse("f286b7cf-445d-4291-9167-0f225b170cae"),
                NetworkId = TestDataForCommittedProjects.NetworkId,
                MaintainableAssetLocation = new MaintainableAssetLocationEntity(Guid.NewGuid(), DataPersistenceCore.DataPersistenceConstants.SectionLocation ,"1"),
                SpatialWeighting = "[DECK_AREA]",
                AggregatedResults = new List<AggregatedResultEntity>()
            },
            new MaintainableAssetEntity() {
                Id = Guid.Parse("46f5da89-5e65-4b8a-9b36-03d9af0302f7"),
                NetworkId = TestDataForCommittedProjects.NetworkId,
                MaintainableAssetLocation = new MaintainableAssetLocationEntity(Guid.NewGuid(), DataPersistenceCore.DataPersistenceConstants.SectionLocation ,"2"),
                SpatialWeighting = "[DECK_AREA]",
                AggregatedResults = new List<AggregatedResultEntity>()
            },
            new MaintainableAssetEntity() {
                Id = Guid.Parse("cf28e62e-0a02-4195-8d28-5cdb9646dd58"),
                NetworkId = TestDataForCommittedProjects.NetworkId,
                MaintainableAssetLocation = new MaintainableAssetLocationEntity(Guid.NewGuid(), DataPersistenceCore.DataPersistenceConstants.SectionLocation ,"3"),
                SpatialWeighting = "[DECK_AREA]",
                AggregatedResults = new List<AggregatedResultEntity>()
            },
            new MaintainableAssetEntity() {
                Id = Guid.Parse("75b07f98-e168-438f-84b6-fcc57b3e3d8f"),
                NetworkId = TestDataForCommittedProjects.NetworkId,
                MaintainableAssetLocation = new MaintainableAssetLocationEntity(Guid.NewGuid(), DataPersistenceCore.DataPersistenceConstants.SectionLocation ,"4"),
                SpatialWeighting = "[DECK_AREA]",
                AggregatedResults = new List<AggregatedResultEntity>()
            },
            new MaintainableAssetEntity() {
                Id = Guid.Parse("dd10baa8-142d-41ec-a8f6-5410d8d1a141"),
                NetworkId = TestDataForCommittedProjects.NetworkId,
                MaintainableAssetLocation = new MaintainableAssetLocationEntity(Guid.NewGuid(), DataPersistenceCore.DataPersistenceConstants.SectionLocation ,"5"),
                SpatialWeighting = "[DECK_AREA]",
                AggregatedResults = new List<AggregatedResultEntity>()
            }
        };


        public static List<AttributeEntity> AttribureEntities => new List<AttributeEntity>()
        {
            new AttributeEntity()
            {
                Id = Guid.Parse("c31ea5bb-3d48-45bb-a68f-01ee75f17f0c"),
                Name = "BRKEY_",
                DataType = "NUMBER"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("cbdc2aac-f2b7-405e-8ff8-21f2785330c1"),
                Name = "BMSID",
                DataType = "STRING"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("67abf485-b3bc-4899-b492-f9165b571040"),
                Name = "DECK_SEEDED",
                DataType = "NUMBER"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("fb86603f-7bc5-4e29-b643-cd739ef065e3"),
                Name = "SUP_SEEDED",
                DataType = "NUMBER"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("cea45b74-f6c2-4e5c-8d2c-3102a85bf339"),
                Name = "SUB_SEEDED",
                DataType = "NUMBER"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("e276df35-a96b-4bdd-b57b-31236a0ddbc9"),
                Name = "CULV_SEEDED",
                DataType = "NUMBER"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("a0c921cc-c40a-41e4-a1d6-33f810397abe"),
                Name = "DECK_DURATION_N",
                DataType = "NUMBER"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("8f81bdf8-f492-40d9-b790-f87ad7de26a5"),
                Name = "SUP_DURATION_N",
                DataType = "NUMBER"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("eb6d72ec-5801-4ec0-bd8b-c5641c291f58"),
                Name = "SUB_DURATION_N",
                DataType = "NUMBER"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("f7693cfe-8705-4f58-8d16-9be6e5d9a2af"),
                Name = "CULV_DURATION_N",
                DataType = "NUMBER"
            }
        };
        private static SimulationEntity FourYearTestSimulation()
        {
            var entity = new SimulationEntity
            {
                Id = TestDataForCommittedProjects.FourYearSimulationId,
                Name = "FourYearTest",
                InvestmentPlan = new InvestmentPlanEntity()
                {
                    Id = Guid.Parse("4d1e1f67-486f-409a-b532-b03d7eb4b1c7"),
                    SimulationId = TestDataForCommittedProjects.FourYearSimulationId,
                    FirstYearOfAnalysisPeriod = 2022,
                    InflationRatePercentage = 3,
                    MinimumProjectCostLimit = 1000,
                    NumberOfYearsInAnalysisPeriod = 4
                },
                Budgets = ScenarioBudgetEntities,
                NetworkId = TestDataForCommittedProjects.NetworkId,
                Network = new NetworkEntity()
                {
                    Id = TestDataForCommittedProjects.NetworkId,
                    Name = TestDataForCommittedProjects.NetworkName,
                    MaintainableAssets = MaintainableAssetEntities
                },
                CashFlowRules = new List<DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CashFlow.ScenarioCashFlowRuleEntity>(),
                CommittedProjects = new List<CommittedProjectEntity>
                {
                    SomethingFourYear2023(),
                    SomethingFourYear2025(),
                }
            };
            entity.InvestmentPlan.Simulation = entity;
            return entity;
        }

        public static List<SimulationEntity> Simulations => new List<SimulationEntity>()
        {
            GoodTestSimulation(),

            FourYearTestSimulation(),

            new SimulationEntity()
            {
                Id = TestDataForCommittedProjects.NoCommitSimulationId,
                Name = "No Commit",
                InvestmentPlan = new InvestmentPlanEntity()
                {
                    Id = Guid.Parse("dab41545-f70b-4747-9112-6790599ff583"),
                    SimulationId = TestDataForCommittedProjects.NoCommitSimulationId,
                    FirstYearOfAnalysisPeriod = 2022,
                    InflationRatePercentage = 3,
                    MinimumProjectCostLimit = 1000,
                    NumberOfYearsInAnalysisPeriod = 3
                },
                Budgets = ScenarioBudgetEntities,
                NetworkId = TestDataForCommittedProjects.NetworkId,
                Network = new NetworkEntity()
                {
                    Id = TestDataForCommittedProjects.NetworkId,
                    Name = TestDataForCommittedProjects.NetworkName,
                    KeyAttributeId = Guid.Parse("c31ea5bb-3d48-45bb-a68f-01ee75f17f0c")
                },
                CashFlowRules = new List<DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CashFlow.ScenarioCashFlowRuleEntity>()
            }
        };
        public static List<InvestmentPlanEntity> InvestmentPlanEntities()
        {
            var result = new List<InvestmentPlanEntity>();
            foreach (var simulation in Simulations)
            {
                var simIP = simulation.InvestmentPlan;
                // The simulation reference must be added here - this avoids the circular reference
                simIP.Simulation = simulation;
                result.Add(simIP);
            }

            return result;
        }

        public static List<ScenarioSelectableTreatmentEntity> FourYearScenarioNoTreatmentEntities()
        {
            var entity = FourYearScenarioNoTreatment();
            var list = new List<ScenarioSelectableTreatmentEntity> { entity };
            return list;
        }

        public static ScenarioSelectableTreatmentEntity FourYearScenarioNoTreatment()
        {


            var equation = new EquationEntity
            {
                Expression = "100",
            };
            var equationJoin = new ScenarioTreatmentCostEquationEntity
            {
                Equation = equation,
                ScenarioTreatmentCostId = TestDataForCommittedProjects.NoTreatmentId,
            };
            var scenarioTreatmentCost = new ScenarioTreatmentCostEntity
            {
                Id = TestDataForCommittedProjects.CostId,
                ScenarioTreatmentCostEquationJoin = equationJoin,
                ScenarioSelectableTreatmentId = TestDataForCommittedProjects.NoTreatmentId,
            };
            var costs = new List<ScenarioTreatmentCostEntity> { scenarioTreatmentCost };
            var consequences = new List<ScenarioConditionalTreatmentConsequenceEntity>();
            var budgets = new List<ScenarioSelectableTreatmentScenarioBudgetEntity>();
            var treatmentJoin = new CriterionLibraryScenarioSelectableTreatmentEntity
            {

            };
            var entity = new ScenarioSelectableTreatmentEntity
            {
                Id = TestDataForCommittedProjects.NoTreatmentId,
                ScenarioTreatmentCosts = costs,
                Description = "No Treatment",
                Name = "No Treatment",
                SimulationId = TestDataForCommittedProjects.FourYearSimulationId,
                ScenarioTreatmentConsequences = consequences,
                ScenarioSelectableTreatmentScenarioBudgetJoins = budgets,
                CriterionLibraryScenarioSelectableTreatmentJoin = null,
            };
            return entity;
        }

    }
}
