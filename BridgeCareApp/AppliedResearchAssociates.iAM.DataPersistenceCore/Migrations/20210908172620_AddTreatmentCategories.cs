using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AddTreatmentCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssetType",
                table: "SelectableTreatment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "SelectableTreatment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AssetType",
                table: "ScenarioSelectableTreatment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "ScenarioSelectableTreatment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssetType",
                table: "SelectableTreatment");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "SelectableTreatment");

            migrationBuilder.DropColumn(
                name: "AssetType",
                table: "ScenarioSelectableTreatment");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "ScenarioSelectableTreatment");
        }
    }
}
