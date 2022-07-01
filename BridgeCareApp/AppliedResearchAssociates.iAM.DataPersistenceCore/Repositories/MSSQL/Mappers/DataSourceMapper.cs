using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class DataSourceMapper
    {
        public static DataSourceEntity ToEntity(this BaseDataSourceDTO dto) =>
            new DataSourceEntity
            {
                Id = dto.Id,
                Name = dto.Name,
                Type = dto.Type,
                Secure = dto.Secure,
                Details = dto.MapDetails()
            };

        public static BaseDataSourceDTO ToDTO(this DataSourceEntity entity)
        {
            if (string.IsNullOrEmpty(entity.Type))
                throw new InvalidOperationException($"Data source {entity.Name} (ID: {entity.Id}) does not have a specified type");

            if (entity.Type == DataSourceTypeStrings.SQL.ToString())
            {
                var source = new SQLDataSourceDTO
                {
                    Id = entity.Id,
                    Name = entity.Name
                };
                source.PopulateDetails(entity.Details);
                return source;
            }

            if (entity.Type.ToLowerInvariant() == DataSourceTypeStrings.Excel.ToString().ToLowerInvariant())
            {
                var source = new ExcelDataSourceDTO
                {
                    Id = entity.Id,
                    Name = entity.Name
                };
                source.PopulateDetails(entity.Details);
                return source;
            }

            throw new InvalidOperationException($"Cannot convert a data source of {entity.Type}");
        }
    }
}
