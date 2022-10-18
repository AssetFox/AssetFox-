﻿using System;
using System.Text;
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
                var connectionString = ((SQLDataSourceDTO)dto).ConnectionString;

                AES256GCM.NewKey();               
                var keyBytes = Encoding.UTF8.GetBytes(EncryptDecryptConstants.Key);
                // Encrypt
                var encryptedText = string.IsNullOrEmpty(connectionString) ? string.Empty : AES256GCM.Encrypt(connectionString, keyBytes);
                entityDetail = encryptedText;
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
                var keyBytes = Encoding.UTF8.GetBytes(EncryptDecryptConstants.Key);
                // Decrypt
                var decryptedConnetionString = string.IsNullOrEmpty(entity.Details) ? string.Empty: AES256GCM.Decrypt(entity.Details, keyBytes);
                var source = new SQLDataSourceDTO
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    ConnectionString = decryptedConnetionString
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
