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
                "data source=localhost;initial catalog=IAMV2;persist security info=True;user id=sa;password=20Pikachu^;MultipleActiveResultSets=True;App=EntityFramework");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        public DbSet<AttributeEntity> Attributes { get; set; }
        public DbSet<DirectionalRouteEntity> DirectionalRoutes { get; set; }
        public DbSet<LinearLocationEntity> LinearLocations { get; set; }
        public DbSet<NetworkEntity> Networks { get; set; }
        public DbSet<RouteEntity> Routes { get; set; }
        public DbSet<SectionLocationEntity> SectionLocations { get; set; }
        public DbSet<SegmentEntity> Segments { get; set; }
        public DbSet<LocationEntity> Locations { get; set; }
        public DbSet<AttributeDatumEntity> AttributeDatum { get; set; }
    }
}
