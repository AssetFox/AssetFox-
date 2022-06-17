using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public class TestDataForCommittedProjects
    {
        private static Guid _networkId = Guid.Parse("7940d27c-c9b1-4ef2-b5e7-2f1d8240a064");

        //public static List<string> KeyProperties => new List<string> { "ID", "BRKEY_", "BMSID" };
        public static Dictionary<string,List<KeySegmentDatum>> KeyProperties()
        {
            var result = new Dictionary<string,List<KeySegmentDatum>>();
            var dummyAttribute = new AttributeDTO() { Id = Guid.Parse("25fba698-d19e-46bf-a1a9-1b61e9560165"), Name = "ID" };
            result.Add("ID", DummyKeySegmentDatum(dummyAttribute));
            result.Add("BRKEY_", DummyKeySegmentDatum(Attributes.Single(_ => _.Name == "BRKEY_")));
            result.Add("BMSID", DummyKeySegmentDatum(Attributes.Single(_ => _.Name == "BMSID")));
            return result;
        }

        public static List<SimulationEntity> Simulations => new List<SimulationEntity>()
        {
            // Must be complete, but only need ID and associated libraries
            new SimulationEntity()
            {
                Id = Guid.Parse("dcdacfde-02da-4109-b8aa-add932756dee"),
                Name = "Test",
                InvestmentPlan = new InvestmentPlanEntity() { Id = Guid.Parse("ad1e1f67-486f-409a-b532-b03d7eb4b1c7"), FirstYearOfAnalysisPeriod = 2022 },
                Budgets = ScenarioBudgetEntities,
                NetworkId = _networkId
            },
            new SimulationEntity()
            {
                Id = Guid.Parse("dae1c62c-adba-4510-bfe5-61260c49ec99"),
                Name = "Test 2",
                InvestmentPlan = new InvestmentPlanEntity() { Id = Guid.Parse("dab41545-f70b-4747-9112-6790599ff583"), FirstYearOfAnalysisPeriod = 2022 },
                Budgets = ScenarioBudgetEntities,
                NetworkId = _networkId
            },
        };

        public static List<AttributeDTO> Attributes => new List<AttributeDTO>()
        {
            // Must include name and ID
            new AttributeDTO()
            {
                Id = Guid.Parse("c31ea5bb-3d48-45bb-a68f-01ee75f17f0c"),
                Name = "BRKEY_"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("cbdc2aac-f2b7-405e-8ff8-21f2785330c1"),
                Name = "BMSID"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("67abf485-b3bc-4899-b492-f9165b571040"),
                Name = "DECK_SEEDED"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("fb86603f-7bc5-4e29-b643-cd739ef065e3"),
                Name = "SUP_SEEDED"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("cea45b74-f6c2-4e5c-8d2c-3102a85bf339"),
                Name = "SUB_SEEDED"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("e276df35-a96b-4bdd-b57b-31236a0ddbc9"),
                Name = "CULV_SEEDED"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("a0c921cc-c40a-41e4-a1d6-33f810397abe"),
                Name = "DECK_DURATION_N"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("8f81bdf8-f492-40d9-b790-f87ad7de26a5"),
                Name = "SUP_DURATION_N"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("eb6d72ec-5801-4ec0-bd8b-c5641c291f58"),
                Name = "SUB_DURATION_N"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("f7693cfe-8705-4f58-8d16-9be6e5d9a2af"),
                Name = "CULV_DURATION_N"
            }
        };

        public static List<AttributeEntity> AttribureEntities => new List<AttributeEntity>()
        {
            new AttributeEntity()
            {
                Id = Guid.Parse("c31ea5bb-3d48-45bb-a68f-01ee75f17f0c"),
                Name = "BRKEY_"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("cbdc2aac-f2b7-405e-8ff8-21f2785330c1"),
                Name = "BMSID"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("67abf485-b3bc-4899-b492-f9165b571040"),
                Name = "DECK_SEEDED"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("fb86603f-7bc5-4e29-b643-cd739ef065e3"),
                Name = "SUP_SEEDED"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("cea45b74-f6c2-4e5c-8d2c-3102a85bf339"),
                Name = "SUB_SEEDED"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("e276df35-a96b-4bdd-b57b-31236a0ddbc9"),
                Name = "CULV_SEEDED"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("a0c921cc-c40a-41e4-a1d6-33f810397abe"),
                Name = "DECK_DURATION_N"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("8f81bdf8-f492-40d9-b790-f87ad7de26a5"),
                Name = "SUP_DURATION_N"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("eb6d72ec-5801-4ec0-bd8b-c5641c291f58"),
                Name = "SUB_DURATION_N"
            },
            new AttributeEntity()
            {
                Id = Guid.Parse("f7693cfe-8705-4f58-8d16-9be6e5d9a2af"),
                Name = "CULV_DURATION_N"
            }
        };

        public static List<MaintainableAsset> MaintainableAssets => CompleteMaintainableAssets;

        public static List<MaintainableAssetEntity> MaintainableAssetEntities => new List<MaintainableAssetEntity>()
        {
            new MaintainableAssetEntity() {
                Id = Guid.Parse("f286b7cf-445d-4291-9167-0f225b170cae"),
                NetworkId = _networkId,
                MaintainableAssetLocation = new MaintainableAssetLocationEntity(Guid.NewGuid(), DataPersistenceCore.DataPersistenceConstants.SectionLocation ,"1"),
                SpatialWeighting = "[DECK_AREA]"
            },
            new MaintainableAssetEntity() {
                Id = Guid.Parse("46f5da89-5e65-4b8a-9b36-03d9af0302f7"),
                NetworkId = _networkId,
                MaintainableAssetLocation = new MaintainableAssetLocationEntity(Guid.NewGuid(), DataPersistenceCore.DataPersistenceConstants.SectionLocation ,"2"),
                SpatialWeighting = "[DECK_AREA]"
            },
            new MaintainableAssetEntity() {
                Id = Guid.Parse("cf28e62e-0a02-4195-8d28-5cdb9646dd58"),
                NetworkId = _networkId,
                MaintainableAssetLocation = new MaintainableAssetLocationEntity(Guid.NewGuid(), DataPersistenceCore.DataPersistenceConstants.SectionLocation ,"3"),
                SpatialWeighting = "[DECK_AREA]"
            },
            new MaintainableAssetEntity() {
                Id = Guid.Parse("75b07f98-e168-438f-84b6-fcc57b3e3d8f"),
                NetworkId = _networkId,
                MaintainableAssetLocation = new MaintainableAssetLocationEntity(Guid.NewGuid(), DataPersistenceCore.DataPersistenceConstants.SectionLocation ,"4"),
                SpatialWeighting = "[DECK_AREA]"
            },
            new MaintainableAssetEntity() {
                Id = Guid.Parse("dd10baa8-142d-41ec-a8f6-5410d8d1a141"),
                NetworkId = _networkId,
                MaintainableAssetLocation = new MaintainableAssetLocationEntity(Guid.NewGuid(), DataPersistenceCore.DataPersistenceConstants.SectionLocation ,"5"),
                SpatialWeighting = "[DECK_AREA]"
            }
        };

        public static List<BaseCommittedProjectDTO> ValidCommittedProjects => new List<BaseCommittedProjectDTO>()
        {
            new SectionCommittedProjectDTO()
            {
                Id = Guid.Parse("2e9e66df-4436-49b1-ae68-9f5c10656b1b"),
                Year = 2022,
                Treatment = "Something",
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 1,
                Cost = 10000,
                SimulationId = Simulations.Single(_ => _.Name == "Test").Id,
                ScenarioBudgetId = ScenarioBudgetDTOs().Single(_ => _.Name == "Local").Id,
                LocationKeys = new Dictionary<string, string>()
                {
                    { "BRKEY_", "1" },
                    { "BMSID", "12345678" }
                },
                Consequences = new List<CommittedProjectConsequenceDTO>()
                {
                    new CommittedProjectConsequenceDTO()
                    {
                        Id = Guid.NewGuid(),
                        Attribute = "DECK_SEEDED",
                        ChangeValue = "+3"
                    },
                    new CommittedProjectConsequenceDTO()
                    {
                        Id = Guid.NewGuid(),
                        Attribute = "DECK_DURATION_N",
                        ChangeValue = "1"
                    }
                }
            },
            new SectionCommittedProjectDTO()
            {
                Id = Guid.Parse("091001e2-c1f0-4af6-90e7-e998bbea5d00"),
                Year = 2023,
                Treatment = "Simple",
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 3,
                Cost = 200000,
                SimulationId = Simulations.Single(_ => _.Name == "Test").Id,
                ScenarioBudgetId = ScenarioBudgetDTOs().Single(_ => _.Name == "Interstate").Id,
                LocationKeys = new Dictionary<string, string>()
                {
                    { "BRKEY_", "2" },
                    { "BMSID", "9876543" }
                },
                Consequences = new List<CommittedProjectConsequenceDTO>()
                {
                    new CommittedProjectConsequenceDTO()
                    {
                        Id = Guid.NewGuid(),
                        Attribute = "DECK_SEEDED",
                        ChangeValue = "9"
                    },
                    new CommittedProjectConsequenceDTO()
                    {
                        Id = Guid.NewGuid(),
                        Attribute = "DECK_DURATION_N",
                        ChangeValue = "1"
                    }
                }
            },
        };

        public static List<CommittedProjectEntity> CommittedProjectEntities => new List<CommittedProjectEntity>()
        {
            new CommittedProjectEntity()
            {
                Id = Guid.Parse("2e9e66df-4436-49b1-ae68-9f5c10656b1b"),
                Year = 2022,
                Name = "Something",
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 1,
                Cost = 10000,
                SimulationId = Simulations.Single(_ => _.Name == "Test").Id,
                ScenarioBudgetId = ScenarioBudgetEntities.Single(_ => _.Name == "Local").Id,
                ScenarioBudget = ScenarioBudgetEntities.Single(_ => _.Name == "Local"),
                CommittedProjectLocation = new CommittedProjectLocationEntity(Guid.NewGuid(), DataPersistenceCore.DataPersistenceConstants.SectionLocation ,"1"),
                CommittedProjectConsequences = new List<CommittedProjectConsequenceEntity>()
                {
                    new CommittedProjectConsequenceEntity()
                    {
                        Id = Guid.NewGuid(),
                        AttributeId = AttribureEntities.Single(_ => _.Name == "DECK_SEEDED").Id,
                        ChangeValue = "+3"
                    },
                    new CommittedProjectConsequenceEntity()
                    {
                        Id = Guid.NewGuid(),
                        AttributeId = AttribureEntities.Single(_ => _.Name == "DECK_DURATION_N").Id,
                        ChangeValue = "1"
                    }
                }
            },
            new CommittedProjectEntity()
            {
                Id = Guid.Parse("091001e2-c1f0-4af6-90e7-e998bbea5d00"),
                Year = 2023,
                Name = "Simple",
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 3,
                Cost = 200000,
                SimulationId = Simulations.Single(_ => _.Name == "Test").Id,
                ScenarioBudgetId = ScenarioBudgetEntities.Single(_ => _.Name == "Interstate").Id,
                ScenarioBudget = ScenarioBudgetEntities.Single(_ => _.Name == "Interstate"),
                CommittedProjectLocation = new CommittedProjectLocationEntity(Guid.NewGuid(), DataPersistenceCore.DataPersistenceConstants.SectionLocation ,"2"),
                CommittedProjectConsequences = new List<CommittedProjectConsequenceEntity>()
                {
                    new CommittedProjectConsequenceEntity()
                    {
                        Id = Guid.NewGuid(),
                        AttributeId = AttribureEntities.Single(_ => _.Name == "DECK_SEEDED").Id,
                        ChangeValue = "9"
                    },
                    new CommittedProjectConsequenceEntity()
                    {
                        Id = Guid.NewGuid(),
                        AttributeId = AttribureEntities.Single(_ => _.Name == "DECK_DURATION_N").Id,
                        ChangeValue = "1"
                    }
                }
            }
        };

        public static List<BaseCommittedProjectDTO> UnmatchedAssetCommittedProjects()
        {
            var returnData = ValidCommittedProjects;
            returnData.Add(new SectionCommittedProjectDTO()
            {
                Id = Guid.Parse("2d232e7b-d745-4ea5-b2a4-1de5a96d7efe"),
                Year = 2023,
                Treatment = "Simple",
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 3,
                Cost = 200000,
                SimulationId = Simulations.Single(_ => _.Name == "Test").Id,
                ScenarioBudgetId = ScenarioBudgetDTOs().Single(_ => _.Name == "Interstate").Id,
                LocationKeys = new Dictionary<string, string>()
                {
                    { "BRKEY_", "9" },
                    { "BMSID", "0876500" }
                },
                Consequences = new List<CommittedProjectConsequenceDTO>()
                {
                    new CommittedProjectConsequenceDTO()
                    {
                        Id = Guid.NewGuid(),
                        Attribute = "DECK_SEEDED",
                        ChangeValue = "9"
                    },
                    new CommittedProjectConsequenceDTO()
                    {
                        Id = Guid.NewGuid(),
                        Attribute = "DECK_DURATION_N",
                        ChangeValue = "1"
                    }
                }
            });
            return returnData;
        }

        public static List<BaseCommittedProjectDTO> NullBudgetCommittedProjects()
        {
            var returnData = ValidCommittedProjects;
            returnData.Add(new SectionCommittedProjectDTO()
            {
                Id = Guid.Parse("2d232e7b-d745-4ea5-b2a4-1de5a96d7efe"),
                Year = 2023,
                Treatment = "Simple",
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 3,
                Cost = 200000,
                SimulationId = Simulations.Single(_ => _.Name == "Test").Id,
                ScenarioBudgetId = null,
                LocationKeys = new Dictionary<string, string>()
                {
                    { "BRKEY_", "3" },
                    { "BMSID", "11122233" }
                },
                Consequences = new List<CommittedProjectConsequenceDTO>()
                {
                    new CommittedProjectConsequenceDTO()
                    {
                        Id = Guid.NewGuid(),
                        Attribute = "DECK_SEEDED",
                        ChangeValue = "+4"
                    }
                }
            });
            return returnData;
        }

        public static List<BudgetDTO> ScenarioBudgets => ScenarioBudgetDTOs();

        #region Helpers
        private static List<ScenarioBudgetEntity> ScenarioBudgetEntities => new List<ScenarioBudgetEntity>()
        {
            new ScenarioBudgetEntity()
            {
                Id = Guid.Parse("4dcf6bbc-d135-458c-a6fc-db9bb0801bfd"),
                Name = "Interstate",
                ScenarioBudgetAmounts = new List<ScenarioBudgetAmountEntity>()
                {
                    new ScenarioBudgetAmountEntity() { Year = 2022, Value = 10000000 },
                    new ScenarioBudgetAmountEntity() { Year = 2023, Value = 10000000 }
                }
            },
            new ScenarioBudgetEntity()
            {
                Id = Guid.Parse("93d59432-c9e5-4a4a-8f1b-d18dcfc0524d"),
                Name = "Local",
                ScenarioBudgetAmounts = new List<ScenarioBudgetAmountEntity>()
                {
                    new ScenarioBudgetAmountEntity() { Year = 2022, Value = 5000000 },
                    new ScenarioBudgetAmountEntity() { Year = 2023, Value = 3000000 }
                }
            }
        };

        private static List<BudgetDTO> ScenarioBudgetDTOs()
        {
            // Avoiding the use of the mapper here as that is not what we are tesitng
            var result = new List<BudgetDTO>();
            foreach (var budget in ScenarioBudgetEntities)
            {
                var budgetAmounts = new List<BudgetAmountDTO>();
                foreach (var amount in budget.ScenarioBudgetAmounts)
                {
                    budgetAmounts.Add(new BudgetAmountDTO()
                    {
                        Id = Guid.NewGuid(),
                        Year = amount.Year,
                        Value = amount.Value,
                        BudgetName = budget.Name
                    });
                }
                result.Add(new BudgetDTO()
                {
                    Id = budget.Id,
                    Name = budget.Name,
                    BudgetAmounts = budgetAmounts
                });
            }
            return result;
        }

        private static List<MaintainableAsset> CompleteMaintainableAssets => new List<MaintainableAsset>()
        {
            new MaintainableAsset(
                Guid.Parse("f286b7cf-445d-4291-9167-0f225b170cae"),
                _networkId,
                new SectionLocation(Guid.NewGuid(), "1"),
                "[DECK_AREA]"
            ),
            new MaintainableAsset(
                Guid.Parse("46f5da89-5e65-4b8a-9b36-03d9af0302f7"),
                _networkId,
                new SectionLocation(Guid.NewGuid(), "2"),
                "[DECK_AREA]"
            ),
            new MaintainableAsset(
                Guid.Parse("cf28e62e-0a02-4195-8d28-5cdb9646dd58"),
                _networkId,
                new SectionLocation(Guid.NewGuid(), "3"),
                "[DECK_AREA]"
            ),
            new MaintainableAsset(
                Guid.Parse("75b07f98-e168-438f-84b6-fcc57b3e3d8f"),
                _networkId,
                new SectionLocation(Guid.NewGuid(), "4"),
                "[DECK_AREA]"
            ),
            new MaintainableAsset(
                Guid.Parse("dd10baa8-142d-41ec-a8f6-5410d8d1a141"),
                _networkId,
                new SectionLocation(Guid.NewGuid(), "5"),
                "[DECK_AREA]"
            )
        };

        private static List<KeySegmentDatum> DummyKeySegmentDatum(AttributeDTO attribute) {
            var keySegmentData = new List<KeySegmentDatum>();
            foreach (var asset in CompleteMaintainableAssets)
            {
                keySegmentData.Add(new KeySegmentDatum() { AssetId = asset.Id, KeyValue = new SegmentAttributeDatum(attribute.Name, asset.Location.LocationIdentifier) });
            }
            return keySegmentData;
        }

        #endregion
    }
}
