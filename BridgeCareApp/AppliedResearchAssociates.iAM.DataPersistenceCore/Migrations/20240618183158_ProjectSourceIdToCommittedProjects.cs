using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class ProjectSourceIdToCommittedProjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AssetType",
                table: "SelectableTreatment",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "Bridge");

            migrationBuilder.AlterColumn<string>(
                name: "AssetType",
                table: "ScenarioSelectableTreatment",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "Bridge");

            migrationBuilder.AddColumn<string>(
                name: "ProjectSourceId",
                table: "CommittedProject",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectSourceId",
                table: "CommittedProject");

            migrationBuilder.AlterColumn<string>(
                name: "AssetType",
                table: "SelectableTreatment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Bridge",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AssetType",
                table: "ScenarioSelectableTreatment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Bridge",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
