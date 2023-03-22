using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork
{
    public static class UnitOfDataPersistenceWorkExtensions
    {
        public const string CannotStartTransactionWhileAnotherTransactionIsInProgress
            = "Cannot start a database transaction while another transaction is in progress.";
        public static void AsTransaction(this UnitOfDataPersistenceWork unitOfWork, Action transactionContents)
        {
            if (unitOfWork.Context.Database.CurrentTransaction != null)
            {
                throw new InvalidOperationException(CannotStartTransactionWhileAnotherTransactionIsInProgress);
            }
            try
            {
                unitOfWork.BeginTransaction();
                transactionContents();
                unitOfWork.Commit();
            } catch
            {
                unitOfWork.Rollback();
                throw;
            }
        }
    }
}
