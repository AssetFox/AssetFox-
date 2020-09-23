using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistence.Models;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistence.Repositories.MSSQL
{
    public class IAMContext : DbContext
    {
        public IAMContext(DbContextOptions<IAMContext> options)
            : base(options)
        {

        }
        public DbSet<Inventory> Inventories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("connectionString"); // connection string will come from config file
            base.OnConfiguring(optionsBuilder);
        }
    }
}
