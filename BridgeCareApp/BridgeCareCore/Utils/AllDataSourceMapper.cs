using System;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using BridgeCareCore.Models;

namespace BridgeCareCore.Utils
{
    public static class AllDataSourceMapper
    {
        private static ExcelDataSourceDTO ToExcel(AllDataSource allDataSource)
        {
            var returnValue = new ExcelDataSourceDTO
            {
                DateColumn = allDataSource.DateColumn,
                Name = allDataSource.Name,
                Id = allDataSource.Id,
                LocationColumn = allDataSource.LocationColumn,
            };
            return returnValue;
        }
        public static BaseDataSourceDTO ToSpecificDto(AllDataSource allDataSource)
        {
            switch (allDataSource.Type.ToLower())
            {
            case "excel":
                return ToExcel(allDataSource);
            default:
                throw new NotImplementedException();
            }
        }
    }
}
