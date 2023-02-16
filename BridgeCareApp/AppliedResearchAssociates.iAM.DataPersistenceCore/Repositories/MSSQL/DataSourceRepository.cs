using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using System.Data;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class DataSourceRepository : IDataSourceRepository
    {
        private UnitOfDataPersistenceWork _unitOfWork;

        public DataSourceRepository(UnitOfDataPersistenceWork uow)
        {
            _unitOfWork = uow;
        }

        public List<BaseDataSourceDTO> GetDataSources()
        {
            var result = new List<BaseDataSourceDTO>();
            foreach (var source in _unitOfWork.Context.DataSource.ToList())
            {
                try
                {
                    result.Add(source.ToDTO(_unitOfWork.EncryptionKey));
                }
                catch
                {
                    // Do nothing.  The data source was invalid
                }
            }

            return result;
        }

        public BaseDataSourceDTO GetDataSource(Guid id) =>
            _unitOfWork.Context.DataSource.FirstOrDefault(_ => _.Id == id)?.ToDTO(_unitOfWork.EncryptionKey);

        public void DeleteDataSource(Guid id)
        {
            var dataSource = _unitOfWork.Context.DataSource.Include(_ => _.ExcelRawData).FirstOrDefault(_ => _.Id == id);
            if (dataSource == null)
                throw new RowNotInTableException("The specified data source was not found.");

            // Setting all related attributes's datasource to none
            var attributes = _unitOfWork.Context.Attribute.Where(_ => _.DataSourceId == id).ToList();

            attributes.ForEach(_ => _.DataSourceId = null);
            _unitOfWork.Context.UpsertAll(attributes);

            _unitOfWork.Context.DeleteEntity<DataSourceEntity>(_ => _.Id == id);
            return;
        }
                
        public void UpsertDatasource(BaseDataSourceDTO dataSource)
        {
            if (!_unitOfWork.Context.DataSource.Any(_ => _.Id == dataSource.Id) && _unitOfWork.Context.DataSource.Any(_ => _.Name == dataSource.Name))
                throw new ArgumentException("An existing data source with the same name already exists");

            if (!dataSource.Validate())
                throw new ArgumentException("The data source could not be validated");                

            _unitOfWork.Context.Upsert(dataSource.ToEntity(_unitOfWork.EncryptionKey), dataSource.Id, _unitOfWork.UserEntity?.Id);
        }           
    }
}
