using System;
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
            if (!_unitOfWork.Context.DataSource.Any(ds => ds.Id == dto.DataSourceId)) {
                throw new InvalidOperationException($"There is no DataSource with id {dto.DataSourceId}");
            };
            if(_unitOfWork.Context.ExcelRawData.Any(_ => _.DataSourceId == dto.DataSourceId))
            {
                _unitOfWork.Context.DeleteAll<ExcelRawDataEntity>(_ => _.DataSourceId == dto.DataSourceId);
            }
            var entity = dto.ToEntity();
            var userId = _unitOfWork.UserEntity?.Id;
            var entities = new List<ExcelRawDataEntity> { entity };
            _unitOfWork.Context.AddAll(entities, userId);
            return entity.Id;
        }

        public ExcelRawDataDTO GetExcelRawDataByDataSourceId(Guid dataSourceId)
        {
            var entity = _unitOfWork.Context.ExcelRawData.FirstOrDefault(ew => ew.DataSourceId == dataSourceId);
            return ExcelDatabaseWorksheetMapper.ToDTONullPropagating(entity);
        }
    }
}
