using System;
using System.IO;
using System.Data.SqlClient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using MathNet.Numerics.Optimization;
using System.Reflection;

namespace AppliedResearchAssociates.iAM.DataUnitTests
{
    public static class RunBatch
    {
        public static void SetupStoredProcedures(UnitOfDataPersistenceWork unitOfWork, string connectionString)
        {
            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                var directoryInfo = GetSqlScriptDirectoryInfo();
                var files = directoryInfo.GetFiles("*.sql");

                foreach (FileInfo f in files)
                {
                    string fname = f.Name;
                    StreamReader fileReader = f.OpenText();
                    string fileContent = fileReader.ReadToEnd();
                    SqlCommand command = new SqlCommand(fileContent, conn);
                    command.CommandTimeout = 600;
                    int rowsAffected = command.ExecuteNonQuery();
                    fileContent = string.Empty;
                }

                // Execute the stored procedure
                unitOfWork.Context.Database.SetCommandTimeout(TimeSpan.FromSeconds(4000));
                unitOfWork.Context.Database.ExecuteSqlRaw("EXEC usp_master_procedure;");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message + " Error occurred in RunBatch-ExecuteCommand.");
            }

        }

        private static DirectoryInfo GetSqlScriptDirectoryInfo()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyLocation = assembly.Location;
            var assemblyFolder = Path.GetDirectoryName(assemblyLocation);
            var folder = Path.Combine(assemblyFolder, "DataTestSetup", "SqlScripts");
            DirectoryInfo dirInfo = new DirectoryInfo(folder);
            return dirInfo;
        }
    }
}
