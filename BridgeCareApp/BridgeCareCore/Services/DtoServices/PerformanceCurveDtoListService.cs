using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services
{
    public static class PerformanceCurveDtoListService
    {
        public static void AddLibraryIdToScenarioPerformanceCurves(List<PerformanceCurveDTO> performanceCurveDTOs, Guid? libraryId)
        {
            if (libraryId == null) return;
            foreach (var dto in performanceCurveDTOs)
            {
                dto.LibraryId = (Guid)libraryId;
            }
        }

        public static void AddModifiedToScenarioPerformanceCurve(List<PerformanceCurveDTO> performanceCurveDTOs, bool IsModified)
        {
            foreach (var dto in performanceCurveDTOs)
            {
                dto.IsModified = IsModified;
            }
        }
    }
}
