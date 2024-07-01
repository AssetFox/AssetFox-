using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CommittedProjectEntity : TreatmentEntity
    {
        public Guid SimulationId { get; set; }

        public Guid? ScenarioBudgetId { get; set; }

        public double Cost { get; set; }

        public int Year { get; set; }

        public string Category { get; set; }

        public TreatmentCategory treatmentCategory { get; set; }

        public virtual SimulationEntity Simulation { get; set; }

        public virtual ScenarioBudgetEntity ScenarioBudget { get; set; }

        public virtual CommittedProjectLocationEntity CommittedProjectLocation { get; set; }

        public string ProjectSource { get; set; }

        public string ProjectId { get; set; }
    }
}
