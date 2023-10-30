using System;
using System.IO;
using System.Data.SqlClient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using MathNet.Numerics.Optimization;

namespace AppliedResearchAssociates.iAM.UnitTestsCore
{
    public class RunBatch
    {

        private readonly UnitOfDataPersistenceWork _unitOfWork;
  
        public RunBatch(UnitOfDataPersistenceWork unitOfWork)
        {
            _unitOfWork = unitOfWork ??
                          throw new ArgumentNullException(nameof(unitOfWork));
        }


        public void ExecuteCommand(string connectionString)
        {
            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                var folder = Directory.GetCurrentDirectory();
                DirectoryInfo dirInfo = new DirectoryInfo(@"C:\BridgeCareAppRepository\Infrastructure Asset Management\BridgeCareApp\AppliedResearchAssociates.iAM.DataPersistenceCore\Repositories\MSSQL\SQLScripts\");
                FileInfo[] files = null;
                files = dirInfo.GetFiles("*.sql");

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
                _unitOfWork.Context.Database.SetCommandTimeout(TimeSpan.FromSeconds(4000));
                _unitOfWork.Context.Database.ExecuteSqlRaw("EXEC usp_master_procedure;");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message + " Error occurred in RunBatch-ExecuteCommand.");
            }

        }
    }
}
