using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class UpdateCashFlowRuleLibraryUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CashFlowRuleLibrary_User_CashFlowRuleLibrary_CashFlowRuleLibraryId",
                table: "CashFlowRuleLibrary_User");

            migrationBuilder.RenameColumn(
                name: "CashFlowRuleLibraryId",
                table: "CashFlowRuleLibrary_User",
                newName: "LibraryId");

            migrationBuilder.RenameIndex(
                name: "IX_CashFlowRuleLibrary_User_CashFlowRuleLibraryId",
                table: "CashFlowRuleLibrary_User",
                newName: "IX_CashFlowRuleLibrary_User_LibraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CashFlowRuleLibrary_User_CashFlowRuleLibrary_LibraryId",
                table: "CashFlowRuleLibrary_User",
                column: "LibraryId",
                principalTable: "CashFlowRuleLibrary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CashFlowRuleLibrary_User_CashFlowRuleLibrary_LibraryId",
                table: "CashFlowRuleLibrary_User");

            migrationBuilder.RenameColumn(
                name: "LibraryId",
                table: "CashFlowRuleLibrary_User",
                newName: "CashFlowRuleLibraryId");

            migrationBuilder.RenameIndex(
                name: "IX_CashFlowRuleLibrary_User_LibraryId",
                table: "CashFlowRuleLibrary_User",
                newName: "IX_CashFlowRuleLibrary_User_CashFlowRuleLibraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CashFlowRuleLibrary_User_CashFlowRuleLibrary_CashFlowRuleLibraryId",
                table: "CashFlowRuleLibrary_User",
                column: "CashFlowRuleLibraryId",
                principalTable: "CashFlowRuleLibrary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
