using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.DataMinerUnitTests.Tests
{
    public static class ExcelRawDataSetup
    {
        public static ExcelRawDataDTO RawData(Guid dataSourceId)
        {
            var serializedWorksheetContent = @"[[""BRKEY"", 1, 10, 100, 10001],[""DISTRICT"", 11, 20, 110, 10011],[""Inspection_Date"", ""2022-01-01T00:00:00"", ""2022-02-01T00:00:00"", ""2022-03-01T00:00:00"", ""2022-04-01T00:00:00""]]";
            var returnValue = new ExcelRawDataDTO
            {
                Id = Guid.NewGuid(),
                DataSourceId = dataSourceId,
                SerializedWorksheetContent = serializedWorksheetContent
            };
            return returnValue;
        }
    }
}
