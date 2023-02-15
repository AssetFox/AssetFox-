using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AddTreatmentCategoryToCommittedProjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "treatmentCategory",
                table: "CommittedProject",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "treatmentCategory",
                table: "CommittedProject");
        }
    }
}
