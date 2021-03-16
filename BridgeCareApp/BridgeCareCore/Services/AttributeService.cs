using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Models;

namespace BridgeCareCore.Services
{
    public class AttributeService
    {
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public AttributeService(UnitOfDataPersistenceWork unitOfDataPersistenceWork) => _unitOfDataPersistenceWork =
            unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

        public List<AttributeSelectValuesResult> GetAttributeSelectValues(List<string> attributeNames)
        {
            var attributeSelectValuesResults = new List<AttributeSelectValuesResult>();

            var validAttributes = _unitOfDataPersistenceWork.Context.Attribute.Select(_ => _.Name)
                .Where(_ => attributeNames.Contains(_)).ToList();

            if (!validAttributes.Any())
            {
                throw new RowNotInTableException($"Provided attributes are not valid.");
            }

            var attributesWithValues = new List<string>();

            using var sqlConnection = new SqlConnection(_unitOfDataPersistenceWork.LegacyConnection.ConnectionString);
            sqlConnection.Open();

            var attributeCountSelects =
                validAttributes.Select(attribute => $"COUNT(DISTINCT({attribute})) AS {attribute}").ToList();
            var countsQuery = $"SELECT {string.Join(", ", attributeCountSelects)} FROM SEGMENT_13_NS0;";

            var sqlCommand = new SqlCommand(countsQuery, sqlConnection);

            var reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    for (var i = 0; i < validAttributes.Count; i++)
                    {
                        if (reader.GetInt32(i) > 100)
                        {
                            attributeSelectValuesResults.Add(new AttributeSelectValuesResult
                            {
                                Attribute = validAttributes[i],
                                Values = new List<string>(),
                                ResultMessage = $"Number of values for attribute {validAttributes[i]} exceeds 100; use text input",
                                ResultType = "warning"
                            });
                        }
                        else if (reader.GetInt32(i) == 0)
                        {
                            attributeSelectValuesResults.Add(new AttributeSelectValuesResult
                            {
                                Attribute = validAttributes[i],
                                Values = new List<string>(),
                                ResultMessage = $"No values found for attribute {validAttributes[i]}; use text input",
                                ResultType = "warning"
                            });
                        }
                        else
                        {
                            attributesWithValues.Add(validAttributes[i]);
                        }
                    }
                }
            }

            if (attributesWithValues.Any())
            {
                var multipleAttributeValuesSelectQueries = new List<string>();
                attributesWithValues.ForEach(attribute =>
                {
                    multipleAttributeValuesSelectQueries.Add($"SELECT DISTINCT(CAST({attribute} AS VARCHAR(255))) AS {attribute} FROM SEGMENT_13_NS0;");
                });

                sqlCommand = new SqlCommand(string.Join("", multipleAttributeValuesSelectQueries), sqlConnection);

                var index = 0;

                reader = sqlCommand.ExecuteReader();
                while (reader.HasRows)
                {
                    var values = new List<string>();

                    while (reader.Read())
                    {
                        if (reader[reader.GetName(0)] != null)
                        {
                            values.Add(reader[reader.GetName(0)].ToString());
                        }
                    }

                    attributeSelectValuesResults.Add(new AttributeSelectValuesResult
                    {
                        Attribute = attributesWithValues[index],
                        Values = values,
                        ResultMessage = "Success",
                        ResultType = "success"
                    });

                    index++;

                    reader.NextResult();
                }
            }

            sqlCommand.Dispose();
            sqlConnection.Close();

            return attributeSelectValuesResults;
        }
    }
}
