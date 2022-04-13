using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;

namespace BridgeCareCore.Services
{
    public class TreatmentService : ITreatmentService
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public TreatmentService(
            UnitOfDataPersistenceWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
        }
        public FileInfoDTO GenerateExcelFile(Guid libraryId)
        {
            var library = _unitOfWork.SelectableTreatmentRepo.GetTreatmentLibary(libraryId);
            var found = library != null;
            var dummyName = $"Dummy treatments export";
            var filename = found ? dummyName : $"{dummyName} (library not found)";
            var r = new FileInfoDTO
            {
                FileData = libraryId.ToString(),
                MimeType = "text",
                FileName = filename,
            };
            return r;
        }
    }
}
