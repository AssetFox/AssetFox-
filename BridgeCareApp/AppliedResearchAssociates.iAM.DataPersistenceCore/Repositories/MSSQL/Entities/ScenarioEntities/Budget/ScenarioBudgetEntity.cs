using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget
{
    public class ScenarioBudgetEntity : BaseBudgetEntity
    {
        public ScenarioBudgetEntity()
        {
            ScenarioBudgetAmounts = new List<ScenarioBudgetAmountEntity>();
            BudgetPercentagePairs = new List<BudgetPercentagePairEntity>();
            CommittedProjects = new HashSet<CommittedProjectEntity>();
            ScenarioSelectableTreatmentScenarioBudgetJoins = new HashSet<ScenarioSelectableTreatmentScenarioBudgetEntity>();
        }

        public Guid SimulationId { get; set; }

        public Guid LibraryId { get; set; }

        public bool IsModified { get; set; }

        public virtual SimulationEntity Simulation { get; set; }

        public virtual ICollection<ScenarioBudgetAmountEntity> ScenarioBudgetAmounts { get; set; }

        public virtual ICollection<BudgetPercentagePairEntity> BudgetPercentagePairs { get; set; }

        public virtual ICollection<CommittedProjectEntity> CommittedProjects { get; set; }

        public virtual ICollection<ScenarioSelectableTreatmentScenarioBudgetEntity> ScenarioSelectableTreatmentScenarioBudgetJoins { get; set; }

        public virtual CriterionLibraryScenarioBudgetEntity CriterionLibraryScenarioBudgetJoin { get; set; }
    }
}
