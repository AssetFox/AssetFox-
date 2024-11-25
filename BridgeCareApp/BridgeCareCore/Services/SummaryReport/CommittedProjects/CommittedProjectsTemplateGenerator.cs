using System;
using System.Collections.Generic;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.DTOs;


namespace BridgeCareCore.Services.SummaryReport.CommittedProjects
{
    public class CommittedProjectsTemplateGenerator
    {
        private readonly string _networkKeyField;

        public CommittedProjectsTemplateGenerator(string networkKeyField)
        {
            _networkKeyField = networkKeyField;
        }


        public FileInfoDTO CreateCommittedProjectTemplate()
        {
            var fileName = $"CommittedProjectsTemplate.xlsx";

            using var excelPackage = new ExcelPackage();

            var worksheet = excelPackage.Workbook.Worksheets.Add("Committed Projects");

            AddHeaderCells(worksheet);

            return new FileInfoDTO
            {
                FileName = fileName,
                FileData = Convert.ToBase64String(excelPackage.GetAsByteArray()),
                MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
        }

        private void AddHeaderCells(ExcelWorksheet worksheet)
        {
            var column = 1;
            var headersToUse = _networkKeyField.Equals(CommittedProjectsColumnHeaders.CRS, StringComparison.OrdinalIgnoreCase)
                ? CommittedProjectsColumnHeaders.RoadHeaders
                : _networkKeyField.Equals(CommittedProjectsColumnHeaders.BRKey, StringComparison.OrdinalIgnoreCase)
                    ? CommittedProjectsColumnHeaders.BridgeHeaders
                    : throw new Exception($"Unknown network key field: {_networkKeyField}");            

            // Add the headers to the worksheet
            foreach (var header in headersToUse)
            {
                worksheet.Cells[1, column++].Value = header;
            }
        }
    }    
}
