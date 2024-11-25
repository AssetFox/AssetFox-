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
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.CellData;

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
            _unitOfWork.Context.UpdateAll(attributes);

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

        public Dictionary<string, string> GetRawData(Dictionary<AttributeDTO, string> dictionary)
        {
            if (dictionary.Keys.Distinct().Count() < dictionary.Count())
                throw new InvalidAttributeException("Dictionary has repeated attributes.");

            //  https://stackoverflow.com/questions/51514029/get-objects-from-dictionary-with-equal-properties-values-for-every-key
            if (dictionary.Keys.Select(_ => _.DataSource.Id).Distinct().Count() != 1 || !dictionary.Keys.Select(_ => _.DataSource.Type).All(_ => _ == "Excel"))
                throw new ArgumentException("Not all attributes are from the same Excel data source.");

            var excelSpreadsheet = _unitOfWork.ExcelWorksheetRepository.GetExcelRawDataByDataSourceId(dictionary.Keys.FirstOrDefault().DataSource.Id);
            if (excelSpreadsheet is null)
            {
                var warningMessage = $@"Found DataSource {dictionary.Keys.FirstOrDefault().DataSource.Name}. The DataSource was of type ""EXCEL"". However, we did not find an ExcelRawData for that data source.";
                throw new RowNotInTableException(warningMessage);
            }
            var worksheet = ExcelRawDataSpreadsheetSerializer.Deserialize(excelSpreadsheet.SerializedWorksheetContent).Worksheet;

            List<List<int>> indices = new();
            foreach (KeyValuePair<AttributeDTO, string> kvp in dictionary)
            {
                List<int> ints = new();
                foreach (var column in worksheet.Columns)
                    for (int i = 1; i < column.Entries.Count; i++)
                        if ( column.Entries[i].GetType() != typeof(EmptyExcelCellDatum) &&
                            column.Entries[0].ObjectValue().ToString().ToUpper() == kvp.Key.Command.ToUpper() &&
                            kvp.Value == column.Entries[i].ObjectValue().ToString() )
                            ints.Add(i);

                if (ints.Count > 0)
                    indices.Add(ints);
                else
                    throw new ArgumentException("No assets match the criteria: " + kvp.Key.Name + ", \'" + kvp.Value + "\'.");
            }

            // https://stackoverflow.com/questions/45255495/find-common-items-in-multiple-lists-in-c-sharp-linq
            List<int> aggregate = indices.Aggregate<IEnumerable<int>>((a, b) => a.Intersect(b)).ToList();
            if (aggregate.Count > 1)
                throw new ArgumentException("More than one asset matches the criteria.");
            if (aggregate.Count == 0)
                throw new RowNotInTableException("No assets match all criteria.");

            Dictionary<string, string> results = new();
            foreach (var column in worksheet.Columns)
                if (column.Entries[aggregate[0]].GetType() != typeof(EmptyExcelCellDatum))
                {
                    string name = column.Entries[0].ObjectValue().ToString();
                    string value = column.Entries[aggregate[0]].ObjectValue().ToString();
                    results.Add(name, value);
                }
            if (results.Count == 0)
                throw new RowNotInTableException("The row of assets does not exist.");
            return results;
        }
    }
}
