using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils.DatabaseRecordCounting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using NLog.Layouts;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Org.BouncyCastle.Tsp;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    //https://stackoverflow.com/questions/1443704/query-to-list-number-of-records-in-each-table-in-a-database
    public static class TableRowCounter
    {
        public const string RowCountSql = @"select t.name TableName, i.rows Records from sysobjects t, sysindexes i where t.xtype = 'U' and i.id = t.id and i.indid in (0,1) order by TableName;";

        public static List<T> ExecSQL<T>(DatabaseFacade database, string query)
        {
            // hopefully no longer needed once SqlQuery comes out in EfCore 7 or 8.
            using (var command = database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                database.OpenConnection();

                List<T> list = new List<T>();
                using (var result = command.ExecuteReader())
                {
                    T obj = default(T);
                    while (result.Read())
                    {
                        obj = Activator.CreateInstance<T>();
                        foreach (PropertyInfo prop in obj.GetType().GetProperties())
                        {
                            if (!object.Equals(result[prop.Name], DBNull.Value))
                            {
                                prop.SetValue(obj, result[prop.Name], null);
                            }
                        }
                        list.Add(obj);
                    }
                }
                database.CloseConnection();
                return list;
            }
        }

        public static List<RowCountReturnCustomEntity> CountRows()
        {
            var sql = RowCountSql;
            var database = TestHelper.UnitOfWork.Context.Database;
            var rowCounts = ExecSQL<RowCountReturnCustomEntity>(database, sql);
            return rowCounts;
        }
    }
}
