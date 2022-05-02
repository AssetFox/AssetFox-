using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class SharedLibraries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsShared",
                table: "TreatmentLibrary",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShared",
                table: "TargetConditionGoalLibrary",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShared",
                table: "RemainingLifeLimitLibrary",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShared",
                table: "PerformanceCurveLibrary",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShared",
                table: "DeficientConditionGoalLibrary",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShared",
                table: "CriterionLibrary",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShared",
                table: "CashFlowRuleLibrary",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShared",
                table: "CalculatedAttributeLibrary",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShared",
                table: "BudgetPriorityLibrary",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShared",
                table: "BudgetLibrary",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsShared",
                table: "TreatmentLibrary");

            migrationBuilder.DropColumn(
                name: "IsShared",
                table: "TargetConditionGoalLibrary");

            migrationBuilder.DropColumn(
                name: "IsShared",
                table: "RemainingLifeLimitLibrary");

            migrationBuilder.DropColumn(
                name: "IsShared",
                table: "PerformanceCurveLibrary");

            migrationBuilder.DropColumn(
                name: "IsShared",
                table: "DeficientConditionGoalLibrary");

            migrationBuilder.DropColumn(
                name: "IsShared",
                table: "CriterionLibrary");

            migrationBuilder.DropColumn(
                name: "IsShared",
                table: "CashFlowRuleLibrary");

            migrationBuilder.DropColumn(
                name: "IsShared",
                table: "CalculatedAttributeLibrary");

            migrationBuilder.DropColumn(
                name: "IsShared",
                table: "BudgetPriorityLibrary");

            migrationBuilder.DropColumn(
                name: "IsShared",
                table: "BudgetLibrary");
        }
    }
}
