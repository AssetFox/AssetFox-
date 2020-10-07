﻿using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;

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
            //optionsBuilder.UseSqlServer("data source=RMD-PPATORN2-LT\\SQLSERVER2014;initial catalog=IAMv2;persist security info=True;user id=sa;password=20Pikachu^;MultipleActiveResultSets=True;App=EntityFramework");
            optionsBuilder.UseSqlServer("data source=localhost;initial catalog=IAMV2;persist security info=True;user id=sa;password=20Pikachu^;MultipleActiveResultSets=True;App=EntityFramework");
#endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MaintainableAssetEntity>(entity =>
            {
                entity.HasIndex(e => e.NetworkId);
                entity.HasIndex(e => e.UniqueIdentifier);

                entity.HasOne(d => d.Network)
                    .WithMany(p => p.MaintainableAssetEntities)
                    .HasForeignKey(d => d.NetworkId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Location)
                    .WithOne(p => p.MaintainableAssetEntity)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<LocationEntity>(entity =>
            {
                entity.HasOne(d => d.Route)
                    .WithOne(p => p.Location)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AttributeDatumEntity>(entity =>
            {
                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.AttributeData)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.AttributeData)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Segment)
                    .WithMany(p => p.AttributeData)
                    .HasForeignKey(d => d.MaintainableAssetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AggregationResultEntity>(entity =>
            {
                entity.HasOne(d => d.Attribute)
                    .WithOne(p => p.AggregationResult)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Segment)
                    .WithMany(p => p.AggregatedResults)
                    .HasForeignKey(d => d.MaintainableAssetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        public DbSet<NetworkEntity> Networks { get; set; }

        public DbSet<MaintainableAssetEntity> MaintainableAssets { get; set; }

        public DbSet<LocationEntity> Locations { get; set; }

        public DbSet<AttributeEntity> Attributes { get; set; }

        public DbSet<AttributeDatumEntity> AttributeData { get; set; }

        public DbSet<AggregationResultEntity> AggregationResults { get; set; }
    }
}
