using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistence.Models;
using AppliedResearchAssociates.iAM.DataPersistence.Repositories.MSSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistence.Repositories.MSSQL
{
    public class IAMContext : DbContext
    {
        public IAMContext(DbContextOptions<IAMContext> options)
            : base(options)
        {

        }
        public DbSet<AttributeEntity> Attributes { get; set; }
        public DbSet<DirectionalRouteEntity> DirectionalRoutes { get; set; }
        public DbSet<LinearLocationEntity> LinearLocations { get; set; }
        public DbSet<NetworkEntity> Networks { get; set; }
        public DbSet<RouteEntity> Routes { get; set; }
        public DbSet<SectionLocationEntity> SectionLocations { get; set; }
        public DbSet<SegmentEntity> Segments { get; set; }
    }
}
