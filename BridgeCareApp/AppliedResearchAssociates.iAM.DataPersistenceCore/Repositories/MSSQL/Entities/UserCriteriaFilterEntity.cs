using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class UserCriteriaFilterEntity : BaseEntity
    {
        public Guid UserCriteriaId { get; set; }

        public Guid UserId { get; set; }
        public string Criteria { get; set; }
        public bool HasCriteria { get; set; }

        public virtual UserEntity UserEntityJoin { get; set; }
    }
}
