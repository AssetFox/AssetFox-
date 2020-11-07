using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQLLegacy.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQLLegacy
{
    public class LegacyDbContext : DbContext
    {
        private string ConnectionString;

        public LegacyDbContext() { }

        public LegacyDbContext(DbContextOptions<LegacyDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(@"data source=localhost;initial catalog=DbBackup;persist security info=True;user id=sa;password=20Pikachu^;MultipleActiveResultSets=True;App=EntityFramework");
        }

        public DbSet<PennDotReportAEntity> pennDotReportAResults { get; set; }
    }
}
