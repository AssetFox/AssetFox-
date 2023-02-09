using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public class TestDataForCommittedProjects
    {
        public static Guid NetworkId => Guid.Parse("7940d27c-c9b1-4ef2-b5e7-2f1d8240a064");

        public static Guid AuthorizedUser => Guid.Parse("b047f934-2a40-4cbb-b3cd-0a17c8a5af21");

        public static Guid UnauthorizedUser => Guid.Parse("4be6302a-e8c8-484a-a64b-67d66b3e21a8");

        public static Guid SimulationId => Guid.Parse("dcdacfde-02da-4109-b8aa-add932756dee");

        public static Guid FourYearSimulationId => Guid.Parse("4cdacfde-02da-4109-b8aa-add932756dee");

        public static Guid NoCommitSimulationId => Guid.Parse("dae1c62c-adba-4510-bfe5-61260c49ec99");

        public static Guid NoTreatmentId => Guid.Parse("00dacfde-02da-4109-b8aa-add932756dee");

        public static Guid CostId => Guid.Parse("100dacfe-02da-4109-b8aa-add932756dee");

        //public static List<string> KeyProperties => new List<string> { "ID", "BRKEY_", "BMSID" };
        public static Dictionary<string, List<KeySegmentDatum>> KeyProperties()
        {
            var result = new Dictionary<string, List<KeySegmentDatum>>();
            var dummyAttribute = new AttributeDTO() { Id = Guid.Parse("25fba698-d19e-46bf-a1a9-1b61e9560165"), Name = "ID" };
            result.Add("ID", DummyKeySegmentDatum(dummyAttribute));
            result.Add("BRKEY_", DummyKeySegmentDatum(Attributes.Single(_ => _.Name == "BRKEY_")));
            result.Add("BMSID", DummyKeySegmentDatum(Attributes.Single(_ => _.Name == "BMSID")));
            return result;
        }

        public static List<SimulationDTO> AuthorizedSimulationDTOs()
        {
            var authorizedUserDto = new SimulationUserDTO
            {
                UserId = AuthorizedUser,
                Username = "pdsystbamsusr01",
                IsOwner = true,
                CanModify = true
            };
            var result = TestEntitiesForCommittedProjects.Simulations.Select(_ => _.ToDto(new UserEntity() { Id = authorizedUserDto.UserId, Username = authorizedUserDto.Username })).ToList();
            foreach (var simulation in result)
            {
                simulation.Users.Add(authorizedUserDto);
            }
            return result;
        }

        public static List<AttributeDTO> Attributes => new List<AttributeDTO>()
        {
            // Must include name and ID
            new AttributeDTO()
            {
                Id = Guid.Parse("c31ea5bb-3d48-45bb-a68f-01ee75f17f0c"),
                Name = "BRKEY_",
                Type = "NUMBER"
            },
            new AttributeDTO()
            {
                Id = Guid.Parse("cbdc2aac-f2b7-405e-8ff8-21f2785330c1"),
                Name = "BMSID",
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
                Name = "CULV_DURATION_N",
                Type = "NUMBER"
            }
        };


        public static List<MaintainableAsset> MaintainableAssets => CompleteMaintainableAssets;


        public static List<SectionCommittedProjectDTO> ValidCommittedProjects => new List<SectionCommittedProjectDTO>()
        {
            SomethingSectionCommittedProjectDTO(),
            SimpleSectionCommittedProjectDTO(Guid.Parse("091001e2-c1f0-4af6-90e7-e998bbea5d00"), SimulationId, 2023),
            SimpleSectionCommittedProjectDTO(Guid.Parse("491001e2-c1f0-4af6-90e7-e998bbea5d00"), FourYearSimulationId, 2025),
        };

        private static SectionCommittedProjectDTO SimpleSectionCommittedProjectDTO(Guid id, Guid simulationId, int year) => new SectionCommittedProjectDTO()
        {
            Id = id,
            Year = year,
            Treatment = "Simple",
            ShadowForAnyTreatment = 1,
            ShadowForSameTreatment = 3,
            Cost = 200000,
            SimulationId = simulationId,
            ScenarioBudgetId = ScenarioBudgetDTOs().Single(_ => _.Name == "Interstate").Id,
            LocationKeys = new Dictionary<string, string>()
                {
                    { "ID", "46f5da89-5e65-4b8a-9b36-03d9af0302f7" },
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
            var result = new List<BudgetDTO>();
            foreach (var budget in TestEntitiesForCommittedProjects.ScenarioBudgetEntities)
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
                NetworkId,
                new SectionLocation(Guid.NewGuid(), "1"),
                "[DECK_AREA]"
            ),
            new MaintainableAsset(
                Guid.Parse("46f5da89-5e65-4b8a-9b36-03d9af0302f7"),
                NetworkId,
                new SectionLocation(Guid.NewGuid(), "2"),
                "[DECK_AREA]"
            ),
            new MaintainableAsset(
                Guid.Parse("cf28e62e-0a02-4195-8d28-5cdb9646dd58"),
                NetworkId,
                new SectionLocation(Guid.NewGuid(), "3"),
                "[DECK_AREA]"
            ),
            new MaintainableAsset(
                Guid.Parse("75b07f98-e168-438f-84b6-fcc57b3e3d8f"),
                NetworkId,
                new SectionLocation(Guid.NewGuid(), "4"),
                "[DECK_AREA]"
            ),
            new MaintainableAsset(
                Guid.Parse("dd10baa8-142d-41ec-a8f6-5410d8d1a141"),
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
