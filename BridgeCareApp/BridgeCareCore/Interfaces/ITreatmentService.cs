using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Interfaces
{
    public interface ITreatmentService
    {
        FileInfoDTO GenerateExcelFile(Guid libraryId);
    }
}
