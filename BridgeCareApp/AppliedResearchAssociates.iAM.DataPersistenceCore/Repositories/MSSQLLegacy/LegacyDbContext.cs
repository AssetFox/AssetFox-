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

        public DbSet<PennDotReportAEntity> pennDotReportAResults { get; set; }

        public DbSet<YearlyInvestmentEntity> yearlyInvestmentResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PennDotReportAEntity>()
                .Property(p => p.StructureLength)
                .HasColumnName("LENGTH");
        }
    }
}
