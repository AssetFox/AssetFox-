using AppliedResearchAssociates.iAM.DTOs;
using System.Collections.Generic;
using System;

namespace BridgeCareCore.Services
{
    public static class TreatmentDtoListHelper
    {
        public static void AddLibraryIdToScenarioSelectableTreatments(List<TreatmentDTO> treatmentDTOs, Guid? libraryId)
        {
            if (libraryId == null) return;
            foreach (var dto in treatmentDTOs)
            {
                dto.LibraryId = (Guid)libraryId;
            }
        }

        public static void AddModifiedToScenarioSelectableTreatments(List<TreatmentDTO> treatmentDTOs, bool IsModified)
        {
            foreach (var dto in treatmentDTOs)
            {
                dto.IsModified = IsModified;
            }
        }

    }
}
