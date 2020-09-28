using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class IAMContext : DbContext
    {
        public IAMContext() {}
        public IAMContext(DbContextOptions<IAMContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "data source=RMD-PPATORN2-LT\\SQLSERVER2014;initial catalog=IAMV2;persist security info=True;user id=sa;password=20Pikachu^;MultipleActiveResultSets=True;App=EntityFramework");
            /*optionsBuilder.UseSqlServer(
                "data source=localhost;initial catalog=IAMV2;persist security info=True;user id=sa;password=20Pikachu^;MultipleActiveResultSets=True;App=EntityFramework");*/
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.NoAction;
            }
            /*modelBuilder.Entity<SegmentEntity>()
                .HasOne<NetworkEntity>()
                .WithMany(n => n.Segments)
                .HasForeignKey(s => s.NetworkId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AttributeDatumEntity>()
                .HasOne(e => e.Segment)
                .WithMany(s => s.AttributeData)
                .HasForeignKey(a => a.SegmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LocationEntity>()
                .HasOne(e => e.Segment)
                .WithOne(d => d.Location)
                .HasForeignKey<LocationEntity>(e => e.SegmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AttributeDatumEntity>()
                .HasOne<AttributeEntity>()
                .WithMany(a => a.AttributeData)
                .HasForeignKey(a => a.AttributeId)
                .OnDelete(DeleteBehavior.NoAction);*/
        }

        public DbSet<AttributeEntity> Attributes { get; set; }
        public DbSet<DirectionalRouteEntity> DirectionalRoutes { get; set; }
        public DbSet<NetworkEntity> Networks { get; set; }
        public DbSet<RouteEntity> Routes { get; set; }
        public DbSet<SegmentEntity> Segments { get; set; }
        public DbSet<LocationEntity> Locations { get; set; }
        public DbSet<NumericAttributeDatumEntity> NumericAttributeData { get; set; }
        public DbSet<TextAttributeDatumEntity> TextAttributeData { get; set; }
    }
}
