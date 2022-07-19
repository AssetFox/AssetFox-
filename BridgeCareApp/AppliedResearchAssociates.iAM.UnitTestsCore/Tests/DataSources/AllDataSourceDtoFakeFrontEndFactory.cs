using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using BridgeCareCore.Models;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class AllDataSourceDtoFakeFrontEndFactory
    {
        public static AllDataSource ToAll(ExcelDataSourceDTO dto)
        {
            return new AllDataSource
            {
                ConnectionString = "",
                DateColumn = dto.DateColumn,
                Id = dto.Id,
                LocationColumn = dto.LocationColumn,
                Name = dto.Name,
            };
        }
    }
}
