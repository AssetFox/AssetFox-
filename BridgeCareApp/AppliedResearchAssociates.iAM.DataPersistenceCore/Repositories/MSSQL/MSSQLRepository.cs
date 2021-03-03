using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public abstract class MSSQLRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        protected MSSQLRepository(IAMContext context)
        {
            Context = context;
            if (!IsRunningFromXUnit)
            {
                Context.Database.SetCommandTimeout(1800);
            }
        }

        protected IAMContext Context { get; }

        protected IDbContextTransaction Transaction { get; set; }

        public void StartTransaction() => Transaction = Context.Database.BeginTransaction();

        public void Commit()
        {
            Transaction.Commit();
            Transaction.Dispose();
        }

        public void Rollback()
        {
            Transaction.Commit();
            Transaction.Dispose();
        }
    }
}
