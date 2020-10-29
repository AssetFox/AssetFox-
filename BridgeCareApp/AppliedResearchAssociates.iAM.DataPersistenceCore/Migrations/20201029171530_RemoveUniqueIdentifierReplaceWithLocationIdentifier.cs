using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class RemoveUniqueIdentifierReplaceWithLocationIdentifier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UniqueIdentifier",
                table: "MaintainableAssets");

            migrationBuilder.DropColumn(
                name: "UniqueIdentifier",
                table: "MaintainableAssetLocations");

            migrationBuilder.DropColumn(
                name: "UniqueIdentifier",
                table: "AttributeDatumLocations");

            migrationBuilder.AddColumn<string>(
                name: "LocationIdentifier",
                table: "MaintainableAssetLocations",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LocationIdentifier",
                table: "AttributeDatumLocations",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocationIdentifier",
                table: "MaintainableAssetLocations");

            migrationBuilder.DropColumn(
                name: "LocationIdentifier",
                table: "AttributeDatumLocations");

            migrationBuilder.AddColumn<string>(
                name: "UniqueIdentifier",
                table: "MaintainableAssets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UniqueIdentifier",
                table: "MaintainableAssetLocations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UniqueIdentifier",
                table: "AttributeDatumLocations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
