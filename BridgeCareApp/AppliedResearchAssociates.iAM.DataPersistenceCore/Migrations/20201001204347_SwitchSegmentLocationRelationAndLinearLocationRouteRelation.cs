using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class SwitchSegmentLocationRelationAndLinearLocationRouteRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Segments_SegmentId",
                table: "Locations");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Locations_LinearLocationId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_LinearLocationId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Locations_SegmentId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "LinearLocationId",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "SegmentId",
                table: "Locations");

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "Segments",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RouteId",
                table: "Locations",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Segments_LocationId",
                table: "Segments",
                column: "LocationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_RouteId",
                table: "Locations",
                column: "RouteId",
                unique: true,
                filter: "[RouteId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Routes_RouteId",
                table: "Locations",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Segments_Locations_LocationId",
                table: "Segments",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Routes_RouteId",
                table: "Locations");

            migrationBuilder.DropForeignKey(
                name: "FK_Segments_Locations_LocationId",
                table: "Segments");

            migrationBuilder.DropIndex(
                name: "IX_Segments_LocationId",
                table: "Segments");

            migrationBuilder.DropIndex(
                name: "IX_Locations_RouteId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Segments");

            migrationBuilder.DropColumn(
                name: "RouteId",
                table: "Locations");

            migrationBuilder.AddColumn<Guid>(
                name: "LinearLocationId",
                table: "Routes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SegmentId",
                table: "Locations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Routes_LinearLocationId",
                table: "Routes",
                column: "LinearLocationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_SegmentId",
                table: "Locations",
                column: "SegmentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Segments_SegmentId",
                table: "Locations",
                column: "SegmentId",
                principalTable: "Segments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Locations_LinearLocationId",
                table: "Routes",
                column: "LinearLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
