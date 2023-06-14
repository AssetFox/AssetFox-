using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AddPerformanceFactorToCommittedProjectConsequence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "PerformanceFactor",
                table: "CommittedProjectConsequence",
                type: "real",
                nullable: false,
                defaultValue: 1.2f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PerformanceFactor",
                table: "CommittedProjectConsequence");
        }
    }
}
