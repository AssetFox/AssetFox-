using System;
using AppliedResearchAssociates.iAM.DTOs;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces
{
    public interface ITreatmentService
    {
        FileInfoDTO GenerateExcelFile(Guid libraryId);
        TreatmentImportResultDTO ImportLibraryTreatmentsFile(
            Guid treatmentLibraryId,
            ExcelPackage excelPackage);
    }
}
