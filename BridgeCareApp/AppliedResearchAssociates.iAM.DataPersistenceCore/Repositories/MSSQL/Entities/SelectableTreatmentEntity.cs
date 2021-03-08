using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class SelectableTreatmentEntity : TreatmentEntity
    {
        public SelectableTreatmentEntity()
        {
            TreatmentBudgetJoins = new HashSet<SelectableTreatmentBudgetEntity>();
            TreatmentConsequences = new HashSet<ConditionalTreatmentConsequenceEntity>();
            TreatmentCosts = new HashSet<TreatmentCostEntity>();
            CommittedProjects = new HashSet<CommittedProjectEntity>();
            TreatmentSchedulings = new HashSet<TreatmentSchedulingEntity>();
            TreatmentSupersessions = new HashSet<TreatmentSupersessionEntity>();
        }

        public string Description { get; set; }

        public Guid TreatmentLibraryId { get; set; }

        public virtual TreatmentLibraryEntity TreatmentLibrary { get; set; }

        public virtual CriterionLibrarySelectableTreatmentEntity CriterionLibrarySelectableTreatmentJoin { get; set; }

        public virtual ICollection<SelectableTreatmentBudgetEntity> TreatmentBudgetJoins { get; set; }

        public virtual ICollection<ConditionalTreatmentConsequenceEntity> TreatmentConsequences { get; set; }

        public virtual ICollection<TreatmentCostEntity> TreatmentCosts { get; set; }

        public virtual ICollection<CommittedProjectEntity> CommittedProjects { get; set; }

        public virtual ICollection<TreatmentSchedulingEntity> TreatmentSchedulings { get; set; }

        public virtual ICollection<TreatmentSupersessionEntity> TreatmentSupersessions { get; set; }
    }
}
