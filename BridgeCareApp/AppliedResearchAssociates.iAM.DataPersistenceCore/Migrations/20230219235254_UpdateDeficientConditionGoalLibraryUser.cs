using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class UpdateDeficientConditionGoalLibraryUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeficientConditionGoalLibrary_User_DeficientConditionGoalLibrary_DeficientConditionGoalLibraryId",
                table: "DeficientConditionGoalLibrary_User");

            migrationBuilder.RenameColumn(
                name: "DeficientConditionGoalLibraryId",
                table: "DeficientConditionGoalLibrary_User",
                newName: "LibraryId");

            migrationBuilder.RenameIndex(
                name: "IX_DeficientConditionGoalLibrary_User_DeficientConditionGoalLibraryId",
                table: "DeficientConditionGoalLibrary_User",
                newName: "IX_DeficientConditionGoalLibrary_User_LibraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeficientConditionGoalLibrary_User_DeficientConditionGoalLibrary_LibraryId",
                table: "DeficientConditionGoalLibrary_User",
                column: "LibraryId",
                principalTable: "DeficientConditionGoalLibrary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeficientConditionGoalLibrary_User_DeficientConditionGoalLibrary_LibraryId",
                table: "DeficientConditionGoalLibrary_User");

            migrationBuilder.RenameColumn(
                name: "LibraryId",
                table: "DeficientConditionGoalLibrary_User",
                newName: "DeficientConditionGoalLibraryId");

            migrationBuilder.RenameIndex(
                name: "IX_DeficientConditionGoalLibrary_User_LibraryId",
                table: "DeficientConditionGoalLibrary_User",
                newName: "IX_DeficientConditionGoalLibrary_User_DeficientConditionGoalLibraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeficientConditionGoalLibrary_User_DeficientConditionGoalLibrary_DeficientConditionGoalLibraryId",
                table: "DeficientConditionGoalLibrary_User",
                column: "DeficientConditionGoalLibraryId",
                principalTable: "DeficientConditionGoalLibrary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
