using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AddConditionChangeToAnalysisOutput : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PotentialConditionChange",
                table: "TreatmentRejectionDetail",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ConditionChange",
                table: "TreatmentOptionDetail",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PotentialConditionChange",
                table: "TreatmentRejectionDetail");

            migrationBuilder.DropColumn(
                name: "ConditionChange",
                table: "TreatmentOptionDetail");
        }
    }
}
