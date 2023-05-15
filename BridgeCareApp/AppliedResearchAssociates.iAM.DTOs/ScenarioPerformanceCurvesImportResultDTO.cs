using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// .
    /// </summary>
    public class ScenarioPerformanceCurvesImportResultDTO : WarningServiceResultDTO
    {
        /// <summary>
        /// .
        /// </summary>
        public List<PerformanceCurveDTO> PerformanceCurves { get; set; }
    }
}
