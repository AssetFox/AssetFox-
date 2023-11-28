using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.DataSources
{
    public static class ExcelRawDataDtos
    {
        public static ExcelRawDataDTO Dto(Guid datasourceId, Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var dto = new ExcelRawDataDTO
            {
                DataSourceId = datasourceId,
                Id = resolveId,
                SerializedWorksheetContent = @"[[""BRKEY"", 1, 10, 100, 10001],
                [""DISTRICT"", 11, 20, 110, 10011],
                [""Inspection_Date"", ""2022-01-01T00:00:00"", ""2022-02-01T00:00:00"", ""2022-03-01T00:00:00"", ""2022-04-01T00:00:00""]]"
            };
            return dto;
        }
    }
}
