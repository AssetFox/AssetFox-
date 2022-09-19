using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class indexesForSimulationOutput : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TreatmentSchedulingCollisionDetail_Id",
                table: "TreatmentSchedulingCollisionDetail",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentRejectionDetail_Id",
                table: "TreatmentRejectionDetail",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentOptionDetail_Id",
                table: "TreatmentOptionDetail",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentConsiderationDetail_Id",
                table: "TreatmentConsiderationDetail",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TargetConditionGoalDetail_Id",
                table: "TargetConditionGoalDetail",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SimulationYearDetail_Id",
                table: "SimulationYearDetail",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeficientConditionGoalDetail_Id",
                table: "DeficientConditionGoalDetail",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CashFlowConsiderationDetail_Id",
                table: "CashFlowConsiderationDetail",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetUsageDetail_Id",
                table: "BudgetUsageDetail",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetDetail_Id",
                table: "BudgetDetail",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetSummaryDetailValue_Id",
                table: "AssetSummaryDetailValue",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetSummaryDetail_Id",
                table: "AssetSummaryDetail",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetDetailValue_Id",
                table: "AssetDetailValue",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetDetail_Id",
                table: "AssetDetail",
                column: "Id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TreatmentSchedulingCollisionDetail_Id",
                table: "TreatmentSchedulingCollisionDetail");

            migrationBuilder.DropIndex(
                name: "IX_TreatmentRejectionDetail_Id",
                table: "TreatmentRejectionDetail");

            migrationBuilder.DropIndex(
                name: "IX_TreatmentOptionDetail_Id",
                table: "TreatmentOptionDetail");

            migrationBuilder.DropIndex(
                name: "IX_TreatmentConsiderationDetail_Id",
                table: "TreatmentConsiderationDetail");

            migrationBuilder.DropIndex(
                name: "IX_TargetConditionGoalDetail_Id",
                table: "TargetConditionGoalDetail");

            migrationBuilder.DropIndex(
                name: "IX_SimulationYearDetail_Id",
                table: "SimulationYearDetail");

            migrationBuilder.DropIndex(
                name: "IX_DeficientConditionGoalDetail_Id",
                table: "DeficientConditionGoalDetail");

            migrationBuilder.DropIndex(
                name: "IX_CashFlowConsiderationDetail_Id",
                table: "CashFlowConsiderationDetail");

            migrationBuilder.DropIndex(
                name: "IX_BudgetUsageDetail_Id",
                table: "BudgetUsageDetail");

            migrationBuilder.DropIndex(
                name: "IX_BudgetDetail_Id",
                table: "BudgetDetail");

            migrationBuilder.DropIndex(
                name: "IX_AssetSummaryDetailValue_Id",
                table: "AssetSummaryDetailValue");

            migrationBuilder.DropIndex(
                name: "IX_AssetSummaryDetail_Id",
                table: "AssetSummaryDetail");

            migrationBuilder.DropIndex(
                name: "IX_AssetDetailValue_Id",
                table: "AssetDetailValue");

            migrationBuilder.DropIndex(
                name: "IX_AssetDetail_Id",
                table: "AssetDetail");
        }
    }
}
