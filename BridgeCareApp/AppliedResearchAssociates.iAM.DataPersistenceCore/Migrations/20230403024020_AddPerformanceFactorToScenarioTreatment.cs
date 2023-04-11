using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AddPerformanceFactorToScenarioTreatment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PerformanceFactor",
                table: "SelectableTreatment",
                type: "float",
                nullable: false,
                defaultValue: 1.0);

            migrationBuilder.AddColumn<double>(
                name: "PerformanceFactor",
                table: "ScenarioSelectableTreatment",
                type: "float",
                nullable: false,
                defaultValue: 1.0);

            migrationBuilder.AddColumn<double>(
                name: "PerformanceFactor",
                table: "CommittedProject",
                type: "float",
                nullable: false,
                defaultValue: 1.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PerformanceFactor",
                table: "SelectableTreatment");

            migrationBuilder.DropColumn(
                name: "PerformanceFactor",
                table: "ScenarioSelectableTreatment");

            migrationBuilder.DropColumn(
                name: "PerformanceFactor",
                table: "CommittedProject");
        }
    }
}
