﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataUnitTests
{
    public static class DatabaseResetter
    {
        public static void ResetDatabase(UnitOfDataPersistenceWork unitOfWork)
        {
            unitOfWork.Context.Database.EnsureDeleted();
            unitOfWork.Context.Database.EnsureCreated();
            var connectionString = unitOfWork.Context.Database.GetConnectionString();
            var rb = new RunBatch(unitOfWork);
            rb.ExecuteCommand(connectionString);
        }

        public static void EnsureDatabaseExists(UnitOfDataPersistenceWork unitOfWork)
        {
            unitOfWork.Context.Database.EnsureCreated();
        }
    }
}
