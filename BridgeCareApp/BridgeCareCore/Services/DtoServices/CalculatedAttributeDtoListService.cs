using AppliedResearchAssociates.iAM.DTOs;
using System.Collections.Generic;
using System;

namespace BridgeCareCore.Services
{
    public static class CalculatedAttributeDtoListService
    {

        public static void AddLibraryIdToScenarioCalculatedAttributes(List<CalculatedAttributeDTO> calculatedAttributesDTOs, Guid? libraryId)
        {
            if (libraryId == null) return;
            foreach (var dto in calculatedAttributesDTOs)
            {
                dto.LibraryId = (Guid)libraryId;
            }
        }

        public static void AddModifiedToScenarioCalculatedAttributes(List<CalculatedAttributeDTO> calculatedAttributesDTOs, bool IsModified)
        {
            foreach (var dto in calculatedAttributesDTOs)
            {
                dto.IsModified = IsModified;
            }
        }

    }
}
