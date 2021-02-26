using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class TreatmentLibraryDTO : BaseLibraryDTO
    {
        public List<TreatmentDTO> Treatments { get; set; }
    }
}
