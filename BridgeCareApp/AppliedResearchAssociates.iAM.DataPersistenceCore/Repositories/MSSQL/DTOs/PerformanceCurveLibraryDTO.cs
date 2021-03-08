using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class PerformanceCurveLibraryDTO : BaseLibraryDTO
    {
        public List<PerformanceCurveDTO> PerformanceCurves { get; set; } = new List<PerformanceCurveDTO>();
    }
}
