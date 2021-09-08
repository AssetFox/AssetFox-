using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Enums
{
    public class TreatmentEnum
    {
        public enum TreatmentCategory
        {
            Preservation,
            CapacityAdding,
            Rehabilitation,
            Replacement,
            Maintenance,
            Other
        }
        public enum AssetType
        {
            Bridge,
            Culvert
        }
    }
}
