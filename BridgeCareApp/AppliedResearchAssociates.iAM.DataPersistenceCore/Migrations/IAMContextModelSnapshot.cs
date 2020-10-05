﻿// <auto-generated />
using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    [DbContext(typeof(IAMContext))]
    partial class IAMContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.AggregationResultEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AttributeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("NumericValue")
                        .HasColumnType("float");

                    b.Property<Guid>("SegmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TextValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AttributeId")
                        .IsUnique();

                    b.HasIndex("SegmentId");

                    b.ToTable("AggregationResults");
                });

            modelBuilder.Entity("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.AttributeDatumEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AttributeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("LocationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double?>("NumericValue")
                        .HasColumnType("float");

                    b.Property<Guid>("SegmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TextValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AttributeId");

                    b.HasIndex("LocationId");

                    b.HasIndex("SegmentId");

                    b.ToTable("AttributeData");
                });

            modelBuilder.Entity("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.AttributeEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Command")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ConnectionType")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Attributes");
                });

            modelBuilder.Entity("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LocationEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("End")
                        .HasColumnType("float");

                    b.Property<Guid?>("RouteId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double?>("Start")
                        .HasColumnType("float");

                    b.Property<string>("UniqueIdentifier")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("RouteId")
                        .IsUnique()
                        .HasFilter("[RouteId] IS NOT NULL");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.NetworkEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Networks");
                });

            modelBuilder.Entity("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.RouteEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("Direction")
                        .HasColumnType("int");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UniqueIdentifier")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Routes");
                });

            modelBuilder.Entity("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.SegmentEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("LocationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("NetworkId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UniqueIdentifier")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("LocationId")
                        .IsUnique();

                    b.HasIndex("NetworkId");

                    b.HasIndex("UniqueIdentifier");

                    b.ToTable("Segments");
                });

            modelBuilder.Entity("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.AggregationResultEntity", b =>
                {
                    b.HasOne("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.AttributeEntity", "Attribute")
                        .WithOne("AggregationResult")
                        .HasForeignKey("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.AggregationResultEntity", "AttributeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.SegmentEntity", "Segment")
                        .WithMany("AggregatedResults")
                        .HasForeignKey("SegmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.AttributeDatumEntity", b =>
                {
                    b.HasOne("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.AttributeEntity", "Attribute")
                        .WithMany("AttributeData")
                        .HasForeignKey("AttributeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LocationEntity", "Location")
                        .WithMany("AttributeData")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.SegmentEntity", "Segment")
                        .WithMany("AttributeData")
                        .HasForeignKey("SegmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LocationEntity", b =>
                {
                    b.HasOne("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.RouteEntity", "Route")
                        .WithOne("Location")
                        .HasForeignKey("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LocationEntity", "RouteId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.SegmentEntity", b =>
                {
                    b.HasOne("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LocationEntity", "Location")
                        .WithOne("Segment")
                        .HasForeignKey("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.SegmentEntity", "LocationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.NetworkEntity", "Network")
                        .WithMany("SegmentEntities")
                        .HasForeignKey("NetworkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
