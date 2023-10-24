using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;
using AppliedResearchAssociates.iAM.DTOs;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public class TestDataForSelectableTreatments
    {
        public static Guid NetworkId => Guid.Parse("7940d27c-c9b1-4ef2-b5e7-2f1d8240a061");

        public const string NetworkName = "Primary";
        public const string Username = "pdsystbamsusr01";

        public static Guid AuthorizedUser => Guid.Parse("b047f934-2a40-4cbb-b3cd-0a17c8a5af22");

        public static Guid SimulationId => Guid.Parse("dcdacfde-02da-4109-b8aa-add932756de3");

        public static Guid InterstateBudgetId => Guid.Parse("4dcf6bbc-d135-458c-a6fc-db9bb0801bf4");

        public static Guid LocalBudgetId => Guid.Parse("93d59432-c9e5-4a4a-8f1b-d18dcfc05245");

        public static Guid NoTreatmentId => Guid.Parse("00dacfde-02da-4109-b8aa-add932756dea");

        public static Guid CostId => Guid.Parse("100dacfe-02da-4109-b8aa-add932756deb");

        public const string InterstateBudgetName = "Interstate";
        public const string LocalBudgetName = "Local";
        public const decimal TenMillion = 10000000;
        public const decimal ThreeMillion = 3000000;
        public const decimal FiveMillion = 5000000;
                        
        public static Dictionary<string, List<KeySegmentDatum>> KeyProperties()
        {
            var result = new Dictionary<string, List<KeySegmentDatum>>();
            var dummyAttribute = new AttributeDTO() { Id = Guid.Parse("25fba698-d19e-46bf-a1a9-1b61e9560166"), Name = "ID" };
            result.Add("ID", DummyKeySegmentDatum(dummyAttribute));
            result.Add("BRKEY_", DummyKeySegmentDatum(Attributes.Single(_ => _.Name == "BRKEY_")));
            result.Add("BMSID", DummyKeySegmentDatum(Attributes.Single(_ => _.Name == "BMSID")));
            return result;
        }

        public static SimulationUserDTO SimulationUserDto()
        {
            var dto = new SimulationUserDTO
            {
                UserId = AuthorizedUser,
                Username = Username,
                IsOwner = true,
                CanModify = true,
            };
            return dto;
        }
        

        public static SimulationDTO TestSimulationDTO()
        {
            var date = new DateTime(2023, 2, 9);
            var dto = new SimulationDTO
            {
                Id = SimulationId,
                CreatedDate = date,
                NetworkId = NetworkId,
                NetworkName = NetworkName,
                Creator = Username,
                LastModifiedDate = date,
                Users = new List<SimulationUserDTO>
                {
                    SimulationUserDto(),
                },
                Name = "Test"
            };
            return dto;
        }

        public static List<AttributeDTO> Attributes => new List<AttributeDTO>()
        {
            // Must include name and ID
            new AttributeDTO()
            {
                Id = Guid.Parse("c31ea5bb-3d48-45bb-a68f-01ee75f17f01"),
                Name = "BRKEY_",
                Type = "NUMBER"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("cbdc2aac-f2b7-405e-8ff8-21f2785330c2"),
                Name = "BMSID",
                Type = "STRING"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("67abf485-b3bc-4899-b492-f9165b571043"),
                Name = "DECK_SEEDED",
                Type = "NUMBER"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("fb86603f-7bc5-4e29-b643-cd739ef065e4"),
                Name = "SUP_SEEDED",
                Type = "NUMBER"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("cea45b74-f6c2-4e5c-8d2c-3102a85bf335"),
                Name = "SUB_SEEDED",
                Type = "NUMBER"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("e276df35-a96b-4bdd-b57b-31236a0ddbc6"),
                Name = "CULV_SEEDED",
                Type = "NUMBER"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("a0c921cc-c40a-41e4-a1d6-33f810397ab7"),
                Name = "DECK_DURATION_N",
                Type = "NUMBER"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("8f81bdf8-f492-40d9-b790-f87ad7de26a8"),
                Name = "SUP_DURATION_N",
                Type = "NUMBER"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("eb6d72ec-5801-4ec0-bd8b-c5641c291f59"),
                Name = "SUB_DURATION_N",
                Type = "NUMBER"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("f7693cfe-8705-4f58-8d16-9be6e5d9a2a0"),
                Name = "CULV_DURATION_N",
                Type = "NUMBER"
            }
        };


        public static List<MaintainableAsset> MaintainableAssets => CompleteMaintainableAssets;


        public static List<SectionCommittedProjectDTO> ValidCommittedProjects
        {
            get
            {
                var id = ScenarioBudgetDTOs().Single(_ => _.Name == "Interstate").Id;

                return new List<SectionCommittedProjectDTO>()
              {
              SomethingSectionCommittedProjectDTO(),
              SimpleSectionCommittedProjectDTO(Guid.Parse("091001e2-c1f0-4af6-90e7-e998bbea5d00"), SimulationId, 2023, id)              
          };
            }
        }

        public static SectionCommittedProjectDTO SimpleSectionCommittedProjectDTO(Guid id, Guid simulationId, int year, Guid scenarioBudgetId) => new SectionCommittedProjectDTO()
        {
            Id = id,
            Year = year,
            Treatment = "Simple",
            ShadowForAnyTreatment = 1,
            ShadowForSameTreatment = 3,
            Cost = 200000,
            SimulationId = simulationId,
            ScenarioBudgetId = scenarioBudgetId,
            LocationKeys = new Dictionary<string, string>()
                {
                    { "ID", "46f5da89-5e65-4b8a-9b36-03d9af0302f7" },
                    { "CULV_DURATION_N", "Y" },
                    { "BRKEY_", "2" },
                    { "BMSID", "9876543" }
                }
        };

        private static SectionCommittedProjectDTO SomethingSectionCommittedProjectDTO() => new SectionCommittedProjectDTO()
        {
            Id = Guid.Parse("2e9e66df-4436-49b1-ae68-9f5c10656b1b"),
            Year = 2022,
            Treatment = "Something",
            ShadowForAnyTreatment = 1,
            ShadowForSameTreatment = 1,
            Cost = 10000,
            SimulationId = SimulationId,
            ScenarioBudgetId = ScenarioBudgetDTOs().Single(_ => _.Name == "Local").Id,
            LocationKeys = new Dictionary<string, string>()
                {
                    { "ID", "f286b7cf-445d-4291-9167-0f225b170cae" },
                    { "CULV_DURATION_N", "Y" },
                    { "BRKEY_", "1" },
                    { "BMSID", "12345678" }
                },
        };
        
        #region Helpers

        private static List<BudgetDTO> ScenarioBudgetDTOs()
        {
            // Avoiding the use of the mapper here as that is not what we are tesitng
            var interstate = new BudgetDTO
            {
                Name = InterstateBudgetName,
                Id = InterstateBudgetId,
                BudgetAmounts = new List<BudgetAmountDTO>
                {
                    new BudgetAmountDTO
                    {
                        Year = 2022,
                        BudgetName = InterstateBudgetName,
                        Id = Guid.NewGuid(),
                        Value = TenMillion,
                    },
                    new BudgetAmountDTO
                    {
                        Year = 2023,
                        BudgetName = InterstateBudgetName,
                        Id = Guid.NewGuid(),
                        Value = TenMillion,
                    }
                }

            };
            var local = new BudgetDTO
            {
                Name = LocalBudgetName,
                Id = LocalBudgetId,
                BudgetAmounts = new List<BudgetAmountDTO>
                {
                    new BudgetAmountDTO
                    {
                        Id= Guid.NewGuid(),
                        BudgetName = LocalBudgetName,
                        Value = FiveMillion,
                        Year = 2022,
                    },
                    new BudgetAmountDTO
                    {
                        Id= Guid.NewGuid(),
                        BudgetName = LocalBudgetName,
                        Value = ThreeMillion,
                        Year = 2023,
                    }
                }
            };
            var result = new List<BudgetDTO>
            {
                interstate,
                local,
            };
            var bar = JsonConvert.SerializeObject(result, Formatting.Indented);
            return result;
        }

        private static List<MaintainableAsset> CompleteMaintainableAssets => new List<MaintainableAsset>()
        {
            new MaintainableAsset(
                Guid.Parse("f286b7cf-445d-4291-9167-0f225b170ca1"),
                NetworkId,
                new SectionLocation(Guid.NewGuid(), "1"),
                "[DECK_AREA]"
            ),
            new MaintainableAsset(
                Guid.Parse("46f5da89-5e65-4b8a-9b36-03d9af0302f2"),
                NetworkId,
                new SectionLocation(Guid.NewGuid(), "2"),
                "[DECK_AREA]"
            ),
            new MaintainableAsset(
                Guid.Parse("cf28e62e-0a02-4195-8d28-5cdb9646dd53"),
                NetworkId,
                new SectionLocation(Guid.NewGuid(), "3"),
                "[DECK_AREA]"
            ),
            new MaintainableAsset(
                Guid.Parse("75b07f98-e168-438f-84b6-fcc57b3e3d84"),
                NetworkId,
                new SectionLocation(Guid.NewGuid(), "4"),
                "[DECK_AREA]"
            ),
            new MaintainableAsset(
                Guid.Parse("dd10baa8-142d-41ec-a8f6-5410d8d1a145"),
                NetworkId,
                new SectionLocation(Guid.NewGuid(), "5"),
                "[DECK_AREA]"
            )
        };

        private static List<KeySegmentDatum> DummyKeySegmentDatum(AttributeDTO attribute)
        {
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
