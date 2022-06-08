using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class UpdateSectionMaintainableAssetRemoveFacility : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Section_Facility_FacilityId",
                table: "Section");

            migrationBuilder.DropTable(
                name: "Facility");

            migrationBuilder.DropColumn(
                name: "FacilityName",
                table: "MaintainableAsset");

            migrationBuilder.RenameColumn(
                name: "FacilityId",
                table: "Section",
                newName: "NetworkId");

            migrationBuilder.RenameIndex(
                name: "IX_Section_FacilityId",
                table: "Section",
                newName: "IX_Section_NetworkId");

            migrationBuilder.RenameColumn(
                name: "SectionName",
                table: "MaintainableAsset",
                newName: "AssetName");

            migrationBuilder.AddForeignKey(
                name: "FK_Section_Network_NetworkId",
                table: "Section",
                column: "NetworkId",
                principalTable: "Network",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Section_Network_NetworkId",
                table: "Section");

            migrationBuilder.RenameColumn(
                name: "NetworkId",
                table: "Section",
                newName: "FacilityId");

            migrationBuilder.RenameIndex(
                name: "IX_Section_NetworkId",
                table: "Section",
                newName: "IX_Section_FacilityId");

            migrationBuilder.RenameColumn(
                name: "AssetName",
                table: "MaintainableAsset",
                newName: "SectionName");

            migrationBuilder.AddColumn<string>(
                name: "FacilityName",
                table: "MaintainableAsset",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Facility",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NetworkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facility", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Facility_Network_NetworkId",
                        column: x => x.NetworkId,
                        principalTable: "Network",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Facility_NetworkId",
                table: "Facility",
                column: "NetworkId");

            migrationBuilder.AddForeignKey(
                name: "FK_Section_Facility_FacilityId",
                table: "Section",
                column: "FacilityId",
                principalTable: "Facility",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
