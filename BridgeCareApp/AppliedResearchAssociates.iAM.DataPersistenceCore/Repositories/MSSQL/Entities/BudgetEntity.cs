using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class BudgetEntity : BaseEntity
    {
        public BudgetEntity()
        {
            BudgetAmounts = new HashSet<BudgetAmountEntity>();
            BudgetPercentagePairs = new HashSet<BudgetPercentagePairEntity>();
            TreatmentBudgetJoins = new HashSet<SelectableTreatmentBudgetEntity>();
            ScenarioSelectableTreatmentBudgetJoins = new HashSet<ScenarioSelectableTreatmentBudgetEntity>();
        }

        public Guid Id { get; set; }

        public Guid BudgetLibraryId { get; set; }

        public string Name { get; set; }

        public virtual BudgetLibraryEntity BudgetLibrary { get; set; }

        public virtual CriterionLibraryBudgetEntity CriterionLibraryBudgetJoin { get; set; }

        public virtual ICollection<BudgetAmountEntity> BudgetAmounts { get; set; }

        public virtual ICollection<BudgetPercentagePairEntity> BudgetPercentagePairs { get; set; }

        public virtual ICollection<SelectableTreatmentBudgetEntity> TreatmentBudgetJoins { get; set; }

        public virtual ICollection<ScenarioSelectableTreatmentBudgetEntity> ScenarioSelectableTreatmentBudgetJoins { get; set; }

        public virtual ICollection<CommittedProjectEntity> CommittedProjects { get; set; }
    }
}
