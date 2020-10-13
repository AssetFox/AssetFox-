using System;
using System.Collections.Generic;
using System.Text;
using LiteDB;
using Microsoft.Extensions.Options;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb
{
    public class LiteDbContext : ILiteDbContext
    {
        public LiteDatabase Database { get; }

        public LiteDbContext(IOptions<LiteDbOptions> options)
        {
            Database = new LiteDatabase(options.Value.LiteDbFilePath, );
        }
    }
}
