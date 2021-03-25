using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class UserCriteriaDTO
    {
        public Guid CriteriaId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Criteria { get; set; }
        public bool HasCriteria { get; set; }
        public bool HasAccess { get; set; }
    }
}
