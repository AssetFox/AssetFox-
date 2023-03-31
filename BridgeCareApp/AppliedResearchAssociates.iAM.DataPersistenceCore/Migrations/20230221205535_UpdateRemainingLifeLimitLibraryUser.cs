using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class UpdateRemainingLifeLimitLibraryUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                            name: "FK_RemainingLifeLimitLibrary_User_RemainingLifeLimitLibrary_RemainingLifeLimitLibraryId",
                            table: "RemainingLifeLimitLibrary_User");

            migrationBuilder.RenameColumn(
                name: "RemainingLifeLimitLibraryId",
                table: "RemainingLifeLimitLibrary_User",
                newName: "LibraryId");

            migrationBuilder.RenameIndex(
                name: "IX_RemainingLifeLimitLibrary_User_RemainingLifeLimitLibraryId",
                table: "RemainingLifeLimitLibrary_User",
                newName: "IX_RemainingLifeLimitLibrary_User_LibraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_RemainingLifeLimitLibrary_User_RemainingLifeLimitLibrary_LibraryId",
                table: "RemainingLifeLimitLibrary_User",
                column: "LibraryId",
                principalTable: "RemainingLifeLimitLibrary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                            name: "FK_RemainingLifeLimitLibrary_User_RemainingLifeLimitLibrary_LibraryId",
                            table: "RemainingLifeLimitLibrary_User");

            migrationBuilder.RenameColumn(
                name: "LibraryId",
                table: "RemainingLifeLimitLibrary_User",
                newName: "RemainingLifeLimitLibraryId");

            migrationBuilder.RenameIndex(
                name: "IX_RemainingLifeLimitLibrary_User_LibraryId",
                table: "RemainingLifeLimitLibrary_User",
                newName: "IX_RemainingLifeLimitLibrary_User_RemainingLifeLimitLibraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_RemainingLifeLimitLibrary_User_RemainingLifeLimitLibrary_RemainingLifeLimitLibraryId",
                table: "RemainingLifeLimitLibrary_User",
                column: "RemainingLifeLimitLibraryId",
                principalTable: "RemainingLifeLimitLibrary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
