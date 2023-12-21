using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CommittedProjects
{
    public static class SectionCommittedProjectDtos
    {
        public static SectionCommittedProjectDTO Dto1(Guid id, Guid simulationId) => new SectionCommittedProjectDTO
        {
            Id = id,
            Year = 2023,
            Treatment = "Simple",
            ShadowForAnyTreatment = 1,
            ShadowForSameTreatment = 3,
            Cost = 210000,
            SimulationId = simulationId,
            //ScenarioBudgetId = ScenarioBudgetDTOs().Single(_ => _.Name == "Interstate").Id,
            LocationKeys = new Dictionary<string, string>()
                {
                    { "ID", TestDataForCommittedProjects.MaintainableAssetIdString2 },
                    { TestAttributeNames.CulvDurationN, "3"},
                    { TestAttributeNames.BrKey, "2" },
                    { TestAttributeNames.BmsId, "9876543" }
                }
        };

        public static SectionCommittedProjectDTO Dto2(Guid id, Guid simulationId)
            => new SectionCommittedProjectDTO
            {
                Id = id,
                Year = 2024,
                Treatment = "Simple again",
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 3,
                Cost = 10000,
                SimulationId = simulationId,

                LocationKeys = new Dictionary<string, string>()
                {
                    { "ID", "46f5da89-5e65-4b8a-9b36-03d9af0302f7" },
                    { TestAttributeNames.CulvDurationN, "3"},
                    { TestAttributeNames.BrKey, "2" },
                    { TestAttributeNames.BmsId, "9876543" }
                },
            };
    }
}
