using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public class TestDataForCommittedProjects
    {
        public static List<string> KeyProperties => new List<string> { "ID", "BRKEY_", "BMSID" };

        public static List<Simulation> Simulations => new List<Simulation>()
        {
            // Must be complete, but only need ID and associated libraries
        };

        public static List<AttributeEntity> Attributes => new List<AttributeEntity>()
        {
            // Must include name and ID
        };

        public static List<MaintainableAssetEntity> MaintainableAssets = new List<MaintainableAssetEntity>()
        {
            // Needs full MA with location
        };

        public static List<SectionCommittedProjectDTO> ValidCommittedProjects => new List<SectionCommittedProjectDTO>()
        {

        };

        public static List<SectionCommittedProjectDTO> InvalidCommittedProjects => new List<SectionCommittedProjectDTO>()
        {

        };
    }
}
