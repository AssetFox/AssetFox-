﻿using System;
using System.IO;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Services.Treatment;
using OfficeOpenXml;

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
            var fileInfo = new FileInfo(filename);
            var package = new ExcelPackage(fileInfo);
            var workbook = package.Workbook;
            TreatmentWorksheetGenerator.Fill(workbook, library);
            var bytes = package.GetAsByteArray();
            var fileData = Convert.ToBase64String(bytes);
            var r = new FileInfoDTO
            {
                FileData = fileData,
                FileName = filename,
                MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            };
            return r;
        }
    }
}
