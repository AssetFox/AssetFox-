using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AddLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocationEntity_Segments_SegmentId",
                table: "LocationEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_LocationEntity_LinearLocationId",
                table: "Routes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LocationEntity",
                table: "LocationEntity");

            migrationBuilder.RenameTable(
                name: "LocationEntity",
                newName: "Locations");

            migrationBuilder.RenameIndex(
                name: "IX_LocationEntity_SegmentId",
                table: "Locations",
                newName: "IX_Locations_SegmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Locations",
                table: "Locations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Segments_SegmentId",
                table: "Locations",
                column: "SegmentId",
                principalTable: "Segments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Locations_LinearLocationId",
                table: "Routes",
                column: "LinearLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Segments_SegmentId",
                table: "Locations");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Locations_LinearLocationId",
                table: "Routes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Locations",
                table: "Locations");

            migrationBuilder.RenameTable(
                name: "Locations",
                newName: "LocationEntity");

            migrationBuilder.RenameIndex(
                name: "IX_Locations_SegmentId",
                table: "LocationEntity",
                newName: "IX_LocationEntity_SegmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LocationEntity",
                table: "LocationEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LocationEntity_Segments_SegmentId",
                table: "LocationEntity",
                column: "SegmentId",
                principalTable: "Segments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_LocationEntity_LinearLocationId",
                table: "Routes",
                column: "LinearLocationId",
                principalTable: "LocationEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
