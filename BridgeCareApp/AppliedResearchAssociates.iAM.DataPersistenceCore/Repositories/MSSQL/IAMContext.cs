using System;
using System.Collections.Generic;
using System.IO;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class IAMContext : DbContext
    {
        private string ConnectionString;

        public IAMContext() { }

        public IAMContext(DbContextOptions<IAMContext> options)
            : base(options)
        {
#if DEBUG
            var sqlServerOptionsExtension =
                    options.FindExtension<SqlServerOptionsExtension>();
            if (sqlServerOptionsExtension != null)
            {
                ConnectionString = sqlServerOptionsExtension.ConnectionString;
            }
#endif
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if DEBUG
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Repositories\\MSSQL", "migrationConnection.json");
            var migrationConnection = new MigrationConnection();
            using (FileStream fs = File.Open(filePath, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string rawConnection = sr.ReadToEnd();
                    migrationConnection = JsonConvert
                                .DeserializeAnonymousType(rawConnection, new { ConnectionStrings = default(MigrationConnection) })
                                .ConnectionStrings;
                }
            }

            optionsBuilder.UseSqlServer(migrationConnection.BridgeCareConnex);
#endif
        }

        private class MigrationConnection
        {
            public string BridgeCareConnex { get; set; }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MaintainableAssetEntity>(entity =>
            {
                entity.HasIndex(e => e.NetworkId);

                entity.HasOne(d => d.Network)
                    .WithMany(p => p.MaintainableAssets)
                    .HasForeignKey(d => d.NetworkId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MaintainableAssetLocationEntity>(entity =>
            {
                entity.HasOne(d => d.MaintainableAsset)
                    .WithOne(p => p.MaintainableAssetLocation)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AttributeDatumEntity>(entity =>
            {
                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.AttributeData)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.MaintainableAsset)
                    .WithMany(p => p.AttributeData)
                    .HasForeignKey(d => d.MaintainableAssetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AttributeDatumLocationEntity>(entity =>
            {
                entity.HasOne(d => d.AttributeDatum)
                    .WithOne(p => p.AttributeDatumLocation)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AggregatedResultEntity>(entity =>
            {
                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.AggregatedResults)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.MaintainableAsset)
                    .WithMany(p => p.AggregatedResults)
                    .HasForeignKey(d => d.MaintainableAssetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        public DbSet<NetworkEntity> Networks { get; set; }

        public DbSet<MaintainableAssetEntity> MaintainableAssets { get; set; }

        public DbSet<MaintainableAssetLocationEntity> MaintainableAssetLocations { get; set; }

        public DbSet<AttributeEntity> Attributes { get; set; }

        public DbSet<AttributeDatumEntity> AttributeData { get; set; }

        public DbSet<AttributeDatumLocationEntity> AttributeDatumLocations { get; set; }

        public DbSet<AggregatedResultEntity> AggregatedResults { get; set; }
    }
}
