using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.SqlClient;
//using Microsoft.SqlServer.Management.Smo;
//using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Server;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using static Humanizer.In;
using System.Xml.Linq;
using MathNet.Numerics;
using AppliedResearchAssociates.iAM.Analysis;
using System.Data;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.SQLScripts
{
    public static class RunBatch
    {

        private static readonly UnitOfDataPersistenceWork _unitOfWork;
        public static void ExecuteCommand(string connectionString)
        {
            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                DirectoryInfo dirInfo = new DirectoryInfo(@"C:\BridgeCareAppRepository\Infrastructure Asset Management\BridgeCareApp\AppliedResearchAssociates.iAM.DataPersistenceCore\Repositories\MSSQL\SQLScripts\");
                FileInfo[] files = null;
                files = dirInfo.GetFiles("*.sql");

                foreach (FileInfo f in files)
                {
                    string fname = f.Name;
                    StreamReader fileReader = f.OpenText();
                    string fileContent = fileReader.ReadToEnd();
                    SqlCommand command = new SqlCommand(fileContent, conn);
                    int rowsAffected = command.ExecuteNonQuery();
                    fileContent = string.Empty;
                }


                // Execute the stored procedure
                _unitOfWork.Context.Database.ExecuteSqlRaw("EXEC usp_master_procedure");


            }
            catch (Exception ex)
            {
                // Handle any exceptions, e.g., database connection error
                var xxx = "Error: " + ex.Message;
            }

        }
    }

}
