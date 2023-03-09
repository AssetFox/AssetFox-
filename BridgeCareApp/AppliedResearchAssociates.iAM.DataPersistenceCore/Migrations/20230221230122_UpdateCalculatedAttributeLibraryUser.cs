using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class UpdateCalculatedAttributeLibraryUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalculatedAttributeLibrary_User_CalculatedAttributeLibrary_CalculatedAttributeLibraryId",
                table: "CalculatedAttributeLibrary_User");

            migrationBuilder.RenameColumn(
                name: "CalculatedAttributeLibraryId",
                table: "CalculatedAttributeLibrary_User",
                newName: "LibraryId");

            migrationBuilder.RenameIndex(
                name: "IX_CalculatedAttributeLibrary_User_CalculatedAttributeLibraryId",
                table: "CalculatedAttributeLibrary_User",
                newName: "IX_CalculatedAttributeLibrary_User_LibraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CalculatedAttributeLibrary_User_CalculatedAttributeLibrary_LibraryId",
                table: "CalculatedAttributeLibrary_User",
                column: "LibraryId",
                principalTable: "CalculatedAttributeLibrary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalculatedAttributeLibrary_User_CalculatedAttributeLibrary_LibraryId",
                table: "CalculatedAttributeLibrary_User");

            migrationBuilder.RenameColumn(
                name: "LibraryId",
                table: "CalculatedAttributeLibrary_User",
                newName: "CalculatedAttributeLibraryId");

            migrationBuilder.RenameIndex(
                name: "IX_CalculatedAttributeLibrary_User_LibraryId",
                table: "CalculatedAttributeLibrary_User",
                newName: "IX_CalculatedAttributeLibrary_User_CalculatedAttributeLibraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CalculatedAttributeLibrary_User_CalculatedAttributeLibrary_CalculatedAttributeLibraryId",
                table: "CalculatedAttributeLibrary_User",
                column: "CalculatedAttributeLibraryId",
                principalTable: "CalculatedAttributeLibrary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
