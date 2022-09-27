﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class ExcelRawDataRepository : IExcelRawDataRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public ExcelRawDataRepository(UnitOfDataPersistenceWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Guid AddExcelRawData(ExcelRawDataDTO dto)
        {
            if(_unitOfWork.Context.ExcelRawData.Any(_ => _.DataSourceId == dto.DataSourceId))
            {
                _unitOfWork.Context.DeleteAll<ExcelRawDataEntity>(_ => _.DataSourceId == dto.DataSourceId);
            }
            var entity = dto.ToEntity();
            var userId = _unitOfWork.UserEntity?.Id;
            _unitOfWork.Context.Add(entity);
            _unitOfWork.Context.SaveChanges();
            return entity.Id;
        }

        public ExcelRawDataDTO GetExcelRawData(Guid excelPackageId)
        {
            var entity = _unitOfWork.Context.ExcelRawData.FirstOrDefault(ew => ew.Id == excelPackageId);
            return ExcelDatabaseWorksheetMapper.ToDTONullPropagating(entity);
        }

        public ExcelRawDataDTO GetExcelRawDataByDataSourceId(Guid dataSourceId)
        {
            var entity = _unitOfWork.Context.ExcelRawData.FirstOrDefault(ew => ew.DataSourceId == dataSourceId);
            return ExcelDatabaseWorksheetMapper.ToDTONullPropagating(entity);
        }
    }
}
