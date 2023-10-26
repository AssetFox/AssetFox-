using System;
using System.IO;
using System.Data.SqlClient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;


namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.SQLScripts
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
