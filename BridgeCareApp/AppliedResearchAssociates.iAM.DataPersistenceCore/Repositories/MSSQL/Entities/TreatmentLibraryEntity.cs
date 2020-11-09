using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class TreatmentLibraryEntity
    {
        public TreatmentLibraryEntity()
        {
            TreatmentLibrarySimulationJoins = new HashSet<TreatmentLibrarySimulationEntity>();
            Treatments = new HashSet<SelectableTreatmentEntity>();
        }

        public Guid Id { get; set; }

        public virtual ICollection<TreatmentLibrarySimulationEntity> TreatmentLibrarySimulationJoins { get; set; }
        public virtual ICollection<SelectableTreatmentEntity> Treatments { get; set; }
    }
}
