using System;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;

namespace BridgeCareCore.Services
{
    public class TreatmentService : ITreatmentService
    {
        public FileInfoDTO GenerateExcelFile(Guid libraryId)
        {
            var r = new FileInfoDTO
            {
                FileData = libraryId.ToString(),
                MimeType = "text",
                FileName = "Dummy treatments export",
            };
            return r;
        }
    }
}
