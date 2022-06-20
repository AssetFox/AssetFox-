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
    public class ExcelWorksheetRepository : IExcelWorksheetRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public ExcelWorksheetRepository(UnitOfDataPersistenceWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Guid AddExcelWorksheet(ExcelSpreadsheetDTO dto)
        {
            var entity = dto.ToEntity();
            var userId = _unitOfWork.UserEntity?.Id;
            var entities = new List<ExcelWorksheetEntity> { entity };
            _unitOfWork.Context.AddAll(entities, userId);
            return entity.Id;
        }

        public ExcelSpreadsheetDTO GetExcelSpreadsheet(Guid id)
        {
            var entity = _unitOfWork.Context.ExcelWorksheets.Single(ew => ew.Id == id);
            var dto = ExcelDatabaseWorksheetMapper.ToDTO(entity);
            return dto;
        }

        public ExcelSpreadsheetDTO GetExcelWorksheet(Guid excelPackageId)
        {
            var entity = _unitOfWork.Context.ExcelWorksheets.FirstOrDefault(ew => ew.Id == excelPackageId);
            return ExcelDatabaseWorksheetMapper.ToDTONullPropagating(entity);
        }
    }
}
