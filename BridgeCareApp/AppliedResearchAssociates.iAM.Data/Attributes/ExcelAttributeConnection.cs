using System;
using System.Data;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.CellData;

namespace AppliedResearchAssociates.iAM.Data.Attributes
{
    public class ExcelAttributeConnection : AttributeConnection
    {
        private readonly ExcelRawDataSpreadsheet _rawData;
        private ExcelRawDataColumn _idColumn = null;
        private ExcelRawDataColumn _dateColumn = null;
        private ExcelRawDataColumn _dataColumn = null;

        public ExcelAttributeConnection(Attribute attribute, BaseDataSourceDTO dataSource, ExcelRawDataSpreadsheet sourceData) : base(attribute, dataSource)
        {
            if (dataSource is ExcelDataSourceDTO)
            {
                // This should always happen if this is being called from the connection builder
                var excelDataSource = (ExcelDataSourceDTO)dataSource;
                _rawData = sourceData;
                //var columnHeaders = _rawData.Columns.Select(c => c.Entries[0].ObjectValue().ToString());
                foreach (var column in _rawData.Columns)
                {
                    var columnName = column.Entries[0].ObjectValue().ToString();
                    if (columnName == excelDataSource.LocationColumn)
                    {
                        _idColumn = column;
                    }
                    else if (columnName == excelDataSource.DateColumn)
                    {
                        _dateColumn = column;
                    }
                    else if (columnName == attribute.Command)
                    {
                        _dataColumn = column;
                    }
                }

                if (_idColumn == null)
                {
                    throw new RowNotInTableException($"Provided data source did not have column {excelDataSource.LocationColumn}");
                }
                if (_dateColumn == null)
                {
                    throw new RowNotInTableException($"Provided data source did not have column {excelDataSource.DateColumn}");
                }
                if (_dataColumn == null)
                {
                    throw new RowNotInTableException($"Provided data source did not have column {attribute.Name}");
                }
            }
            else
            {
                if (dataSource == null) throw new ArgumentNullException("Data source passed to the SQL Attribute Connection was null");
                throw new ArgumentException($"{dataSource.Name} has a type of {dataSource.Type}.  It should be Excel");
            }
        }

        public override IEnumerable<IAttributeDatum> GetData<T>()
        {
            double? start = null;
            double? end = null;
            Direction? direction = null;
            string wellKnownText = null;
            var dataSize = Math.Min(_idColumn.Entries.Count, Math.Min(_dateColumn.Entries.Count, _dataColumn.Entries.Count));

            // Skip the entries at the 0 index - these are the headers

            for (int rowIndex = 1; rowIndex < dataSize; rowIndex++)
            {
                // Ensure the date is valid
                DateTime dateEntry;
                if (_dateColumn.Entries[rowIndex] is DateTimeExcelCellDatum)
                {
                    dateEntry = ((DateTimeExcelCellDatum)_dateColumn.Entries[rowIndex]).Value;
                }
                else
                {
                    // Ignore the row
                    break;
                }

                // Ensure the data is valid
                T dataValue = default(T);
                bool assignedFlag = false;  // Use this because the use of break in the switch will not be possible
                switch (Attribute.DataType)
                {
                case "NUMBER":
                    if (!(_dataColumn.Entries[rowIndex] is EmptyExcelCellDatum))
                    {
                        if(_dataColumn.Entries[rowIndex] is StringExcelCellDatum)
                        {
                            var conversionResult = double.TryParse(((StringExcelCellDatum)_dataColumn.Entries[rowIndex]).Value, out double result);
                            if (conversionResult)
                            {
                                dataValue = (T)Convert.ChangeType(result, typeof(T));
                            }
                        }
                        else
                        {
                            dataValue = (T)Convert.ChangeType(((DoubleExcelCellDatum)_dataColumn.Entries[rowIndex]).Value, typeof(T));
                        }

                        assignedFlag = true;
                    }
                    break;
                case "STRING":
                    if (!(_dataColumn.Entries[rowIndex] is EmptyExcelCellDatum))
                    {
                        dataValue = (T)Convert.ChangeType(_dataColumn.Entries[rowIndex].ObjectValue().ToString(), typeof(T));
                        assignedFlag |= true;
                    }
                    break;
                default:
                    break;
                }
                if (!assignedFlag)
                {
                    // Ignore the row
                    break;
                }

                // Ensure the location is not empty
                if (_idColumn.Entries[rowIndex] is EmptyExcelCellDatum)
                {
                    // Ignore the row
                    break;
                }

                // All values are good, build the datum
                yield return new AttributeDatum<T>(Guid.NewGuid(), Attribute, dataValue,
                    LocationBuilder.CreateLocation(_idColumn.Entries[rowIndex].ObjectValue().ToString(), start, end, direction, wellKnownText),
                    dateEntry);
            }
        }
    }
}
