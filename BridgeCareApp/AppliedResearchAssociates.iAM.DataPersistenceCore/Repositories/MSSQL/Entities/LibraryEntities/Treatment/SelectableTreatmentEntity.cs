﻿using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using static AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Enums.TreatmentEnum;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment
{
    public class SelectableTreatmentEntity : TreatmentEntity
    {
        public SelectableTreatmentEntity()
        {
            TreatmentConsequences = new HashSet<ConditionalTreatmentConsequenceEntity>();
            TreatmentCosts = new HashSet<TreatmentCostEntity>();
            TreatmentPerformanceFactors = new HashSet<TreatmentPerformanceFactorEntity>();
            TreatmentSchedulings = new HashSet<TreatmentSchedulingEntity>();
            TreatmentSupersedeRules = new HashSet<TreatmentSupersedeRuleEntity>();
        }

        public string Description { get; set; }

        public Guid TreatmentLibraryId { get; set; }

        public TreatmentCategory Category { get; set; } = TreatmentCategory.Preservation;

        public string AssetType { get; set; }

        public virtual TreatmentLibraryEntity TreatmentLibrary { get; set; }

        public virtual CriterionLibrarySelectableTreatmentEntity CriterionLibrarySelectableTreatmentJoin { get; set; }

        public virtual ICollection<ConditionalTreatmentConsequenceEntity> TreatmentConsequences { get; set; }

        public virtual ICollection<TreatmentCostEntity> TreatmentCosts { get; set; }

        public virtual ICollection<TreatmentPerformanceFactorEntity> TreatmentPerformanceFactors { get; set; }

        public virtual ICollection<TreatmentSchedulingEntity> TreatmentSchedulings { get; set; }

        public virtual ICollection<TreatmentSupersedeRuleEntity> TreatmentSupersedeRules { get; set; }

        public bool IsUnselectable { get; set; }
    }
}
