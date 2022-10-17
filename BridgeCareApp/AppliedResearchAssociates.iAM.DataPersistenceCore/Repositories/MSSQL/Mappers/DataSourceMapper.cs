﻿using System;
using System.Text.Json;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class DataSourceMapper
    {
        public static DataSourceEntity ToEntity(this BaseDataSourceDTO dto)
        {
            var entityDetail = "";
            if (dto is SQLDataSourceDTO)
            {
                entityDetail = ((SQLDataSourceDTO)dto).ConnectionString;
                // key genaration, TODO get generated value once and put that in appsettings
                var newKey = AES256GCM.NewKey();
                // Encrypt
                var encryptedText = AES256GCM.Encrypt(entityDetail, newKey);
                entityDetail = encryptedText;
                // Decrypt, to test
                var plainText = AES256GCM.Decrypt(encryptedText, newKey);
            }
            else if (dto is ExcelDataSourceDTO)
            {
                var excelEntity = (ExcelDataSourceDTO)dto;
                var details = new ExcelDataSourceDetails
                {
                    LocationColumn = excelEntity.LocationColumn,
                    DateColumn = excelEntity.DateColumn
                };
                entityDetail = JsonSerializer.Serialize(details);
            }
            else
            {
                // Do nothing
            }

            return new DataSourceEntity
            {
                Id = dto.Id,
                Name = dto.Name,
                Type = dto.Type,
                Secure = dto.Secure,
                Details = entityDetail
            };
        }

        public static BaseDataSourceDTO ToDTO(this DataSourceEntity entity)
        {
            if (string.IsNullOrEmpty(entity.Type))
                throw new InvalidOperationException($"Data source {entity.Name} (ID: {entity.Id}) does not have a specified type");

            if (entity.Type == DataSourceTypeStrings.SQL.ToString())
            {
                var source = new SQLDataSourceDTO
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    ConnectionString = entity.Details
                };
                return source;
            }

            if (entity.Type.ToLowerInvariant() == DataSourceTypeStrings.Excel.ToString().ToLowerInvariant())
            {
                var hydratedDetails = JsonSerializer.Deserialize<ExcelDataSourceDetails>(entity.Details);
                var source = new ExcelDataSourceDTO
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    LocationColumn = hydratedDetails.LocationColumn,
                    DateColumn = hydratedDetails.DateColumn
                };
                return source;
            }

            throw new InvalidOperationException($"Cannot convert a data source of {entity.Type}");
        }

        private class ExcelDataSourceDetails
        {
            public string LocationColumn { get; set; }
            public string DateColumn { get; set; }
        }
    }
}
