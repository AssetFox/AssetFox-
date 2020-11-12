using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CriterionLibraryAnalysisMethodEntity
    {
        public Guid CriterionLibraryId { get; set; }

        public Guid AnalysisMethodId { get; set; }

        public virtual AnalysisMethodEntity AnalysisMethod { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }
    }
}
