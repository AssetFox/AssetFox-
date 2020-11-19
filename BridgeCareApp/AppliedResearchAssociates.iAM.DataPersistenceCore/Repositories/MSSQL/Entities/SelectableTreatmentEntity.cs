using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class SelectableTreatmentEntity : TreatmentEntity
    {
        public SelectableTreatmentEntity()
        {
            CriterionLibrarySelectableTreatmentJoins = new HashSet<CriterionLibrarySelectableTreatmentEntity>();
            TreatmentBudgetJoins = new HashSet<TreatmentBudgetEntity>();
            TreatmentConsequences = new HashSet<TreatmentConsequenceEntity>();
            TreatmentCosts = new HashSet<TreatmentCostEntity>();
            CommittedProjects = new HashSet<CommittedProjectEntity>();
            TreatmentSchedulings = new HashSet<TreatmentSchedulingEntity>();
            TreatmentSupersessions = new HashSet<TreatmentSupersessionEntity>();
        }

        public Guid TreatmentLibraryId { get; set; }

        public virtual TreatmentLibraryEntity TreatmentLibrary { get; set; }

        public virtual ICollection<CriterionLibrarySelectableTreatmentEntity> CriterionLibrarySelectableTreatmentJoins { get; set; }

        public virtual ICollection<TreatmentBudgetEntity> TreatmentBudgetJoins { get; set; }

        public virtual ICollection<TreatmentConsequenceEntity> TreatmentConsequences { get; set; }

        public virtual ICollection<TreatmentCostEntity> TreatmentCosts { get; set; }

        public virtual ICollection<CommittedProjectEntity> CommittedProjects { get; set; }

        public virtual ICollection<TreatmentSchedulingEntity> TreatmentSchedulings { get; set; }

        public virtual ICollection<TreatmentSupersessionEntity> TreatmentSupersessions { get; set; }
    }
}
