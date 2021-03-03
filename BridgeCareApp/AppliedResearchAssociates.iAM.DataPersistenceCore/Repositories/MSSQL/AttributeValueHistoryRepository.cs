using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MoreLinq;
using Attribute = AppliedResearchAssociates.iAM.Domains.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AttributeValueHistoryRepository : IAttributeValueHistoryRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private static readonly List<string> Props = new List<string>
        {
            "Id", "CreatedDate", "LastModifiedDate", "CreatedBy", "LastModifiedBy", "SectionId", "AttributeId", "Year", "Value"
        };

        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public AttributeValueHistoryRepository(UnitOfDataPersistenceWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public void CreateNumericAttributeValueHistories(
            Dictionary<(Guid sectionId, Guid attributeId), AttributeValueHistory<double>>
                numericAttributeValueHistoryPerSectionIdAttributeIdTuple)
        {
            var numericAttributeValueHistoryEntities = new List<NumericAttributeValueHistoryEntity>();

            MoreEnumerable.ForEach(numericAttributeValueHistoryPerSectionIdAttributeIdTuple.Keys, tuple =>
            {
                var numericAttributeValueHistory = numericAttributeValueHistoryPerSectionIdAttributeIdTuple[tuple];
                numericAttributeValueHistoryEntities.AddRange(numericAttributeValueHistory.ToEntity(tuple.sectionId, tuple.attributeId));
            });

            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.NumericAttributeValueHistory.AddRange(numericAttributeValueHistoryEntities);
            }
            else
            {
                if (numericAttributeValueHistoryEntities.Count > 10000)
                {
                    //DataTable dt;
                    var take = 1000;
                    var skip = 0;
                    var totalPages = (int)Math.Ceiling((decimal)numericAttributeValueHistoryEntities.Count / (decimal)take);
                    while (skip < totalPages)
                    {
                        var currentEntities = numericAttributeValueHistoryEntities
                            .Skip(skip * take).Take(take);/*.ToDataTable(Props);*/
                        //BulkInsert(dt, "NumericAttributeValueHistory");
                        _unitOfWork.Context.BulkInsertOrUpdate(currentEntities.ToList());
                        skip++;
                    }
                }
                else
                {
                    _unitOfWork.Context.BulkInsertOrUpdate(numericAttributeValueHistoryEntities);
                }
            }

            _unitOfWork.Context.SaveChanges();
        }

        public void CreateTextAttributeValueHistories(
            Dictionary<(Guid sectionId, Guid attributeId), AttributeValueHistory<string>>
                textAttributeValueHistoryPerSectionIdAttributeIdTuple)
        {
            var textAttributeValueHistoryEntities = new List<TextAttributeValueHistoryEntity>();

            MoreEnumerable.ForEach(textAttributeValueHistoryPerSectionIdAttributeIdTuple.Keys, key =>
            {
                var textAttributeValueHistory = textAttributeValueHistoryPerSectionIdAttributeIdTuple[key];
                textAttributeValueHistoryEntities.AddRange(textAttributeValueHistory.ToEntity(key.sectionId, key.attributeId));
            });

            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.TextAttributeValueHistory.AddRange(textAttributeValueHistoryEntities);
            }
            else
            {

                if (textAttributeValueHistoryEntities.Count > 10000)
                {
                    //DataTable dt;
                    var take = 1000;
                    var skip = 0;
                    var totalPages = (int)Math.Ceiling((decimal)textAttributeValueHistoryEntities.Count / (decimal)take);
                    while (skip < totalPages)
                    {
                        var currentEntities = textAttributeValueHistoryEntities
                            .Skip(skip * take).Take(take);/*.ToDataTable(Props);*/
                        //BulkInsert(dt, "TextAttributeValueHistory");
                        _unitOfWork.Context.BulkInsertOrUpdate(currentEntities.ToList());
                        skip++;
                    }
                }
                else
                {
                    _unitOfWork.Context.BulkInsertOrUpdate(textAttributeValueHistoryEntities);
                }
            }

            _unitOfWork.Context.SaveChanges();
        }

        public void BulkInsert(DataTable dt, string tableName)
        {
            // make sure to enable triggers
            // more on triggers in next post
            var bulkCopy = new SqlBulkCopy
                (
                    _unitOfWork.Connection,
                    SqlBulkCopyOptions.TableLock |
                    SqlBulkCopyOptions.FireTriggers |
                    SqlBulkCopyOptions.UseInternalTransaction,
                    null
                )
                { DestinationTableName = tableName };

            // set the destination table name
            _unitOfWork.Connection.Open();

            // write the data in the "dataTable"
            bulkCopy.WriteToServer(dt);
            _unitOfWork.Connection.Close();
        }

    }

    public static class BulkUploadToSqlHelper
    {
        public static DataTable ToDataTable<T>(this IEnumerable<T> data, List<string> props)
        {
            var properties = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();

            foreach (var propName in props)
            {
                var prop = properties.Find(propName, false);
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
                
            foreach (var item in data)
            {
                var row = table.NewRow();

                foreach (var propName in props)
                {
                    var prop = properties.Find(propName, false);
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    
                }

                table.Rows.Add(row);
            }

            return table;
        }
    }
}
