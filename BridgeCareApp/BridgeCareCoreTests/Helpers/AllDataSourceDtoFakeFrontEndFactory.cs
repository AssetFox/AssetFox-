using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using BridgeCareCore.Models;

namespace BridgeCareCoreTests
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
                Type = "Excel",
            };
        }

        public static AllDataSource ToAll(SQLDataSourceDTO dto)
        {
            return new AllDataSource
            {
                ConnectionString = dto.ConnectionString,
                Id = dto.Id,
                Name = dto.Name,
                Type = "SQL",
            };
        }

        public static AllDataSource ToAll(BaseDataSourceDTO baseDataSource)
        {
            if (baseDataSource == null)
            {
                return null;
            }
            if (baseDataSource is ExcelDataSourceDTO excel)
            {
                return ToAll(excel);
            }
            if (baseDataSource is SQLDataSourceDTO sql)
            {
                return ToAll(sql);
            }
            if (baseDataSource is AllDataSource all)
            {
                return all;
            }
            throw new InvalidOperationException($"Don't know how to convert a dataSource of type {baseDataSource.Type}");
        }
    }
}
