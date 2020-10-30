using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
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
            //optionsBuilder.UseSqlServer(ConnectionString);
            //optionsBuilder.UseSqlServer("data source=RMD-PPATORN2-LT\\SQLSERVER2014;initial catalog=IAMv2;persist security info=True;user id=sa;password=20Pikachu^;MultipleActiveResultSets=True;App=EntityFramework");
            optionsBuilder.UseSqlServer("data source=localhost;initial catalog=IAMV2;persist security info=True;user id=sa;password=20Pikachu^;MultipleActiveResultSets=True;App=EntityFramework");
            //optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;Initial Catalog=IAMV2");
#endif
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
