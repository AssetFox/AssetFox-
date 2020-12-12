using System.Runtime.Serialization;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class CrudDTO
    {
        public CrudDTO()
        {
            matched = false;
        }

        [IgnoreDataMember]
        public bool matched { get; set; }
    }
}
