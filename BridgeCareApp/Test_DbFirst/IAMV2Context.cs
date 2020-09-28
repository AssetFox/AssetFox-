using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Test_DbFirst
{
    public partial class IAMV2Context : DbContext
    {
        public IAMV2Context()
        {
        }

        public IAMV2Context(DbContextOptions<IAMV2Context> options)
            : base(options)
        {
        }

        public virtual DbSet<AttributeDatumEntity> AttributeDatumEntity { get; set; }
        public virtual DbSet<Attributes> Attributes { get; set; }
        public virtual DbSet<Locations> Locations { get; set; }
        public virtual DbSet<Networks> Networks { get; set; }
        public virtual DbSet<Routes> Routes { get; set; }
        public virtual DbSet<Segments> Segments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=RMD-PPATORN2-LT\\SQLSERVER2014;User ID=sa;Password=20Pikachu^;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;Initial Catalog=IAMV2");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttributeDatumEntity>(entity =>
            {
                entity.HasIndex(e => e.AttributeId);

                entity.HasIndex(e => e.LocationId)
                    .IsUnique();

                entity.HasIndex(e => e.SegmentId);

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Discriminator).IsRequired();

                entity.Property(e => e.TextAttributeDatumEntityValue).HasColumnName("TextAttributeDatumEntity_Value");

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.AttributeDatumEntity)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Location)
                    .WithOne(p => p.AttributeDatumEntity)
                    .HasForeignKey<AttributeDatumEntity>(d => d.LocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Segment)
                    .WithMany(p => p.AttributeDatumEntity)
                    .HasForeignKey(d => d.SegmentId);
            });

            modelBuilder.Entity<Attributes>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Locations>(entity =>
            {
                entity.HasIndex(e => e.SegmentId)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Discriminator).IsRequired();

                entity.HasOne(d => d.Segment)
                    .WithOne(p => p.Locations)
                    .HasForeignKey<Locations>(d => d.SegmentId);
            });

            modelBuilder.Entity<Networks>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Routes>(entity =>
            {
                entity.HasIndex(e => e.LinearLocationId)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Discriminator).IsRequired();

                entity.HasOne(d => d.LinearLocation)
                    .WithOne(p => p.Routes)
                    .HasForeignKey<Routes>(d => d.LinearLocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Segments>(entity =>
            {
                entity.HasIndex(e => e.NetworkId);

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Network)
                    .WithMany(p => p.Segments)
                    .HasForeignKey(d => d.NetworkId);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
