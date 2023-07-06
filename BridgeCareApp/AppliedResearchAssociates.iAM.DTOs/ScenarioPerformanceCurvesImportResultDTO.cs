using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{

    public class ScenarioPerformanceCurvesImportResultDTO : WarningServiceResultDTO
    {

        public List<PerformanceCurveDTO> PerformanceCurves { get; set; }
    }
}
