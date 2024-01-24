using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;
using AppliedResearchAssociates.iAM.DataUnitTests.TestUtils;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public class TestDataForCommittedProjects
    {
        public static Guid NetworkId => Guid.Parse("7940d27c-c9b1-4ef2-b5e7-2f1d8240a064");

        public const string NetworkName = "Primary";
        public const string Username = "pdsystbamsusr01";
        public const string MaintainableAssetIdString1 = "f286b7cf-445d-4291-9167-0f225b170cae";
        public const string MaintainableAssetIdString2 = "46f5da89-5e65-4b8a-9b36-03d9af0302f7";
        public const string CommittedProjectIdString1 = "2e9e66df-4436-49b1-ae68-9f5c10656b1b";
        public const string CommittedProjectIdString2 = "091001e2-c1f0-4af6-90e7-e998bbea5d00";
        public static Guid MaintainableAssetId1 => Guid.Parse(MaintainableAssetIdString1);
        public static Guid MaintainableAssetId2 => Guid.Parse(MaintainableAssetIdString2);
        public static Guid CommittedProjectId1 => Guid.Parse(CommittedProjectIdString1);
        public static Guid CommittedProjectId2 => Guid.Parse(CommittedProjectIdString2);
        public static Guid AuthorizedUser => Guid.Parse("b047f934-2a40-4cbb-b3cd-0a17c8a5af21");

        public static Guid UnauthorizedUser => Guid.Parse("4be6302a-e8c8-484a-a64b-67d66b3e21a8");

        public static Guid SimulationId => Guid.Parse("dcdacfde-02da-4109-b8aa-add932756dee");

        public static Guid FourYearSimulationId => Guid.Parse("4cdacfde-02da-4109-b8aa-add932756dee");

        public static Guid NoCommitSimulationId => Guid.Parse("dae1c62c-adba-4510-bfe5-61260c49ec99");

        public static Guid NoTreatmentId => Guid.Parse("00dacfde-02da-4109-b8aa-add932756dee");

        public static Guid CostId => Guid.Parse("100dacfe-02da-4109-b8aa-add932756dee");

        public static Guid InterstateBudgetId => Guid.Parse("4dcf6bbc-d135-458c-a6fc-db9bb0801bfd");

        public static Guid LocalBudgetId => Guid.Parse("93d59432-c9e5-4a4a-8f1b-d18dcfc0524d");

        public const string InterstateBudgetName = "Interstate";
        public const string LocalBudgetName = "Local";
        public const decimal TenMillion = 10000000;
        public const decimal ThreeMillion = 3000000;
        public const decimal FiveMillion = 5000000;


        //public static List<string> KeyProperties => new List<string> { "ID", "BRKEY_", "BMSID" };
        public static Dictionary<string, List<KeySegmentDatum>> KeyProperties()
        {
            var result = new Dictionary<string, List<KeySegmentDatum>>();
            var dummyAttribute = new AttributeDTO() { Id = Guid.Parse("25fba698-d19e-46bf-a1a9-1b61e9560165"), Name = "ID" };
            result.Add("ID", DummyKeySegmentDatum(dummyAttribute));
            result.Add(TestAttributeNames.BrKey, DummyKeySegmentDatum(Attributes.Single(_ => _.Name == TestAttributeNames.BrKey)));
            result.Add(TestAttributeNames.BmsId, DummyKeySegmentDatum(Attributes.Single(_ => _.Name == "BMSID")));
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

        public static SimulationDTO TestSimulationDTONoCommittedProjects()
        {
            var date = new DateTime(2023, 2, 9);
            var dto = new SimulationDTO
            {
                Id = NoCommitSimulationId,
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
                Id = Guid.Parse("c31ea5bb-3d48-45bb-a68f-01ee75f17f0c"),
                Name = TestAttributeNames.BrKey,
                Type = "NUMBER"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("cbdc2aac-f2b7-405e-8ff8-21f2785330c1"),
                Name = TestAttributeNames.BmsId,
                Type = "STRING"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("67abf485-b3bc-4899-b492-f9165b571040"),
                Name = "DECK_SEEDED",
                Type = "NUMBER"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("fb86603f-7bc5-4e29-b643-cd739ef065e3"),
                Name = "SUP_SEEDED",
                Type = "NUMBER"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("cea45b74-f6c2-4e5c-8d2c-3102a85bf339"),
                Name = "SUB_SEEDED",
                Type = "NUMBER"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("e276df35-a96b-4bdd-b57b-31236a0ddbc9"),
                Name = "CULV_SEEDED",
                Type = "NUMBER"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("a0c921cc-c40a-41e4-a1d6-33f810397abe"),
                Name = "DECK_DURATION_N",
                Type = "NUMBER"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("8f81bdf8-f492-40d9-b790-f87ad7de26a5"),
                Name = "SUP_DURATION_N",
                Type = "NUMBER"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("eb6d72ec-5801-4ec0-bd8b-c5641c291f58"),
                Name = "SUB_DURATION_N",
                Type = "NUMBER"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("f7693cfe-8705-4f58-8d16-9be6e5d9a2af"),
                Name = TestAttributeNames.CulvDurationN,
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
              SimpleSectionCommittedProjectDTO(CommittedProjectId2, SimulationId, 2023, id),
              SimpleSectionCommittedProjectDTO(Guid.Parse("491001e2-c1f0-4af6-90e7-e998bbea5d00"), FourYearSimulationId, 2025, id),
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
                    { "ID", Guid.NewGuid().ToString() },
                    { TestAttributeNames.CulvDurationN, "Y" },
                    { TestAttributeNames.BrKey, "2" },
                    { TestAttributeNames.BmsId, "9876543" }
                }
        };
        private static SectionCommittedProjectDTO SomethingSectionCommittedProjectDTO() => new SectionCommittedProjectDTO()
        {
            Id = CommittedProjectId1,
            Year = 2022,
            Treatment = "Something",
            ShadowForAnyTreatment = 1,
            ShadowForSameTreatment = 1,
            Cost = 10000,
            SimulationId = SimulationId,
            ScenarioBudgetId = ScenarioBudgetDTOs().Single(_ => _.Name == "Local").Id,
            LocationKeys = new Dictionary<string, string>()
                {
                    { "ID", MaintainableAssetIdString1 },
                    { TestAttributeNames.CulvDurationN, "Y" },
                    { TestAttributeNames.BrKey, "1" },
                    { TestAttributeNames.BmsId, "12345678" }
                },
        };


        public static List<SectionCommittedProjectDTO> UnmatchedAssetCommittedProjects()
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
                SimulationId = SimulationId,
                ScenarioBudgetId = ScenarioBudgetDTOs().Single(_ => _.Name == "Interstate").Id,
                LocationKeys = new Dictionary<string, string>()
                {
                    { TestAttributeNames.BrKey, "9" },
                    { TestAttributeNames.BmsId, "0876500" }
                }
            });
            return returnData;
        }

        public static List<SectionCommittedProjectDTO> NullBudgetCommittedProjects()
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
                SimulationId = SimulationId,
                ScenarioBudgetId = null,
                LocationKeys = new Dictionary<string, string>()
                {
                    { TestAttributeNames.BrKey, "3" },
                    { TestAttributeNames.BmsId, "11122233" }
                }
            });
            return returnData;
        }

        public static List<BudgetDTO> ScenarioBudgets => ScenarioBudgetDTOs();



        public static FileInfoDTO GoodFile()
        {
            // Create the simplest committed proejct file possible
            var fileName = "Good Result";
            var package = new ExcelPackage(new FileInfo(fileName));
            var worksheet = package.Workbook.Worksheets.Add("Committed Projects");
            worksheet.Cells[1, 1].Value = "Some Data";

            // Send the file back to the user
            return new FileInfoDTO()
            {
                FileName = fileName,
                FileData = Convert.ToBase64String(package.GetAsByteArray()),
                MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
        }

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

        private const string DefaultEquation = CommonTestParameterValues.DefaultEquation;

        private static List<MaintainableAsset> CompleteMaintainableAssets => new List<MaintainableAsset>()
        {
            new MaintainableAsset(
                MaintainableAssetId1,
                NetworkId,
                new SectionLocation(Guid.NewGuid(), "1"),
                DefaultEquation
            ),
            new MaintainableAsset(
                MaintainableAssetId2,
                NetworkId,
                new SectionLocation(Guid.NewGuid(), "2"),
                DefaultEquation
            ),
            new MaintainableAsset(
                Guid.Parse("cf28e62e-0a02-4195-8d28-5cdb9646dd58"),
                NetworkId,
                new SectionLocation(Guid.NewGuid(), "3"),
                DefaultEquation
            ),
            new MaintainableAsset(
                Guid.Parse("75b07f98-e168-438f-84b6-fcc57b3e3d8f"),
                NetworkId,
                new SectionLocation(Guid.NewGuid(), "4"),
                DefaultEquation
            ),
            new MaintainableAsset(
                Guid.Parse("dd10baa8-142d-41ec-a8f6-5410d8d1a141"),
                NetworkId,
                new SectionLocation(Guid.NewGuid(), "5"),
                DefaultEquation
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

        public static InvestmentPlanDTO TestInvestmentPlan => new InvestmentPlanDTO
        {
            Id = Guid.Parse("ad1e1f67-486f-409a-b532-b03d7eb4b1c7"),
            FirstYearOfAnalysisPeriod = 2022,
            InflationRatePercentage = 3,
            MinimumProjectCostLimit = 1000,
            NumberOfYearsInAnalysisPeriod = 3
        };
    }
}
