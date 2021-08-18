﻿using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CommittedProjectEntity : TreatmentEntity
    {
        public Guid SimulationId { get; set; }

        public Guid ScenarioBudgetId { get; set; }

        public Guid MaintainableAssetId { get; set; }

        public double Cost { get; set; }

        public int Year { get; set; }

        public virtual SimulationEntity Simulation { get; set; }

        public virtual ScenarioBudgetEntity ScenarioBudget { get; set; }

        public virtual MaintainableAssetEntity MaintainableAsset { get; set; }

        public virtual ICollection<CommittedProjectConsequenceEntity> CommittedProjectConsequences { get; set; }
    }
}
