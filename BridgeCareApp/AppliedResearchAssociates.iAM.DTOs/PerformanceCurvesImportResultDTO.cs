using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class PerformanceCurvesImportResultDTO : WarningServiceResultDTO
    {
        public List<PerformanceCurveDTO> PerformanceCurves { get; set; } = new List<PerformanceCurveDTO>();
    }
}
