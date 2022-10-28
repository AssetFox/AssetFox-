using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AttributeValueHistoryRepository : IAttributeValueHistoryRepository
    {
        private static readonly List<string> Props = new List<string>
        {
            "Id", "CreatedDate", "LastModifiedDate", "CreatedBy", "LastModifiedBy", "SectionId", "AttributeId", "Year", "Value"
        };

        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public AttributeValueHistoryRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateNumericAttributeValueHistories(
            Dictionary<(Guid sectionId, Guid attributeId), IAttributeValueHistory<double>>
                numericAttributeValueHistoryPerSectionIdAttributeIdTuple)
        {
            var numericAttributeValueHistoryEntities = new List<NumericAttributeValueHistoryEntity>();

            numericAttributeValueHistoryPerSectionIdAttributeIdTuple.Keys.ForEach(tuple =>
            {
                var numericAttributeValueHistory = numericAttributeValueHistoryPerSectionIdAttributeIdTuple[tuple];
                //numericAttributeValueHistoryEntities.AddRange(numericAttributeValueHistory.ToEntity(tuple.sectionId, tuple.attributeId));
            });

            _unitOfWork.Context.AddAll(numericAttributeValueHistoryEntities, _unitOfWork.UserEntity?.Id);
            /*if (numericAttributeValueHistoryEntities.Count > 10000)
            {
                var take = 1000;
                var skip = 0;
                var totalPages = (int)Math.Ceiling((decimal)numericAttributeValueHistoryEntities.Count / (decimal)take);
                while (skip < totalPages)
                {
                    var currentEntities = numericAttributeValueHistoryEntities
                        .Skip(skip * take).Take(take);
                    _unitOfWork.Context.AddAll(currentEntities.ToList(), _unitOfWork.UserEntity?.Id);
                    skip++;
                }
            }
            else
            {
                _unitOfWork.Context.AddAll(numericAttributeValueHistoryEntities, _unitOfWork.UserEntity?.Id);
            }*/
        }

        public void CreateTextAttributeValueHistories(
            Dictionary<(Guid sectionId, Guid attributeId), IAttributeValueHistory<string>>
                textAttributeValueHistoryPerSectionIdAttributeIdTuple)
        {
            var textAttributeValueHistoryEntities = new List<TextAttributeValueHistoryEntity>();

            textAttributeValueHistoryPerSectionIdAttributeIdTuple.Keys.ForEach(key =>
            {
                var textAttributeValueHistory = textAttributeValueHistoryPerSectionIdAttributeIdTuple[key];
                //textAttributeValueHistoryEntities.AddRange(textAttributeValueHistory.ToEntity(key.sectionId, key.attributeId));
            });

            _unitOfWork.Context.AddAll(textAttributeValueHistoryEntities, _unitOfWork.UserEntity?.Id);
            /*if (textAttributeValueHistoryEntities.Count > 10000)
            {
                var take = 1000;
                var skip = 0;
                var totalPages = (int)Math.Ceiling((decimal)textAttributeValueHistoryEntities.Count / (decimal)take);
                while (skip < totalPages)
                {
                    var currentEntities = textAttributeValueHistoryEntities
                        .Skip(skip * take).Take(take);
                    _unitOfWork.Context.AddAll(currentEntities.ToList(), _unitOfWork.UserEntity?.Id);
                    skip++;
                }
            }
            else
            {
                _unitOfWork.Context.AddAll(textAttributeValueHistoryEntities, _unitOfWork.UserEntity?.Id);
            }*/
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
