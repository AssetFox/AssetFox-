﻿// <auto-generated />
using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    [DbContext(typeof(IAMContext))]
    [Migration("20200925194753_AddLocation")]
    partial class AddLocation
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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

                    b.Property<Guid>("SegmentId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("SegmentId")
                        .IsUnique();

                    b.ToTable("Locations");

                    b.HasDiscriminator<string>("Discriminator").HasValue("LocationEntity");
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

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("LinearLocationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("LinearLocationId")
                        .IsUnique();

                    b.ToTable("Routes");

                    b.HasDiscriminator<string>("Discriminator").HasValue("RouteEntity");
                });

            modelBuilder.Entity("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.SegmentEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AttributeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("NetworkId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AttributeId")
                        .IsUnique();

                    b.HasIndex("NetworkId");

                    b.ToTable("Segments");
                });

            modelBuilder.Entity("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LinearLocationEntity", b =>
                {
                    b.HasBaseType("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LocationEntity");

                    b.Property<double>("End")
                        .HasColumnType("float");

                    b.Property<double>("Start")
                        .HasColumnType("float");

                    b.HasDiscriminator().HasValue("LinearLocationEntity");
                });

            modelBuilder.Entity("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.SectionLocationEntity", b =>
                {
                    b.HasBaseType("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LocationEntity");

                    b.Property<string>("UniqueIdentifier")
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("SectionLocationEntity");
                });

            modelBuilder.Entity("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.DirectionalRouteEntity", b =>
                {
                    b.HasBaseType("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.RouteEntity");

                    b.Property<int>("Direction")
                        .HasColumnType("int");

                    b.HasDiscriminator().HasValue("DirectionalRouteEntity");
                });

            modelBuilder.Entity("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LocationEntity", b =>
                {
                    b.HasOne("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.SegmentEntity", "Segment")
                        .WithOne("Location")
                        .HasForeignKey("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LocationEntity", "SegmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.RouteEntity", b =>
                {
                    b.HasOne("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LinearLocationEntity", "LinearLocation")
                        .WithOne("Route")
                        .HasForeignKey("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.RouteEntity", "LinearLocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.SegmentEntity", b =>
                {
                    b.HasOne("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.AttributeEntity", "Attribute")
                        .WithOne("Segment")
                        .HasForeignKey("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.SegmentEntity", "AttributeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.NetworkEntity", "Network")
                        .WithMany("Segments")
                        .HasForeignKey("NetworkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
