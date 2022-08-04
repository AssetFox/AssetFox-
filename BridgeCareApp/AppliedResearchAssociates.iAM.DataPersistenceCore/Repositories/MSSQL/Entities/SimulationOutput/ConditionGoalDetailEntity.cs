﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public abstract class ConditionGoalDetailEntity: BaseEntity
    {
        public Guid Id { get; set; }

        public Guid AttributeId { get; set; }

        public bool GoalIsMet { get; set; }

        public string GoalName { get; set; }
    }
}
