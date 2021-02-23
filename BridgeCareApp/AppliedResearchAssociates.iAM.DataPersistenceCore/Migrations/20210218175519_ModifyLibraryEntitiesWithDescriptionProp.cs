using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class ModifyLibraryEntitiesWithDescriptionProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TreatmentLibrary",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TargetConditionGoalLibrary",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "RemainingLifeLimitLibrary",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PerformanceCurveLibrary",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "DeficientConditionGoalLibrary",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CriterionLibrary",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CashFlowRuleLibrary",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "BudgetPriorityLibrary",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "BudgetLibrary",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "TreatmentLibrary");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "TargetConditionGoalLibrary");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "RemainingLifeLimitLibrary");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "PerformanceCurveLibrary");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "DeficientConditionGoalLibrary");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CriterionLibrary");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CashFlowRuleLibrary");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "BudgetPriorityLibrary");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "BudgetLibrary");
        }
    }
}
