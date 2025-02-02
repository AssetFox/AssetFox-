﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class BudgetUsageDetailEntity
    {
        public Guid Id { get; set; }

        public Guid TreatmentConsiderationDetailId { get; set; }

        public virtual TreatmentConsiderationDetailEntity TreatmentConsiderationDetail { get; set; }

        public string BudgetName { get; set; }

        public decimal CoveredCost { get; set; }

        public int Status { get; set; }
    }
}
