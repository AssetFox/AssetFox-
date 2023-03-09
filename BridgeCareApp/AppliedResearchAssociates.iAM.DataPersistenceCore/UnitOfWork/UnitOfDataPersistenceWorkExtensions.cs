using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork
{
    public static class UnitOfDataPersistenceWorkExtensions
    {
        public static void AsTransaction(this UnitOfDataPersistenceWork unitOfWork, Action<UnitOfDataPersistenceWork> transactionContents)
        {
            try
            {
                unitOfWork.BeginTransaction();
                transactionContents(unitOfWork);
                unitOfWork.Commit();
            } catch
            {
                unitOfWork.Rollback();
                throw;
            }
        }
    }
}
