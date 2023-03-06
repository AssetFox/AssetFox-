using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class UpdateTreatmentLibraryUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentLibrary_User_TreatmentLibrary_TreatmentLibraryId",
                table: "TreatmentLibrary_User");

            migrationBuilder.RenameColumn(
                name: "TreatmentLibraryId",
                table: "TreatmentLibrary_User",
                newName: "LibraryId");

            migrationBuilder.RenameIndex(
                name: "IX_TreatmentLibrary_User_TreatmentLibraryId",
                table: "TreatmentLibrary_User",
                newName: "IX_TreatmentLibrary_User_LibraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentLibrary_User_TreatmentLibrary_LibraryId",
                table: "TreatmentLibrary_User",
                column: "LibraryId",
                principalTable: "TreatmentLibrary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentLibrary_User_TreatmentLibrary_LibraryId",
                table: "TreatmentLibrary_User");

            migrationBuilder.RenameColumn(
                name: "LibraryId",
                table: "TreatmentLibrary_User",
                newName: "TreatmentLibraryId");

            migrationBuilder.RenameIndex(
                name: "IX_TreatmentLibrary_User_LibraryId",
                table: "TreatmentLibrary_User",
                newName: "IX_TreatmentLibrary_User_TreatmentLibraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentLibrary_User_TreatmentLibrary_TreatmentLibraryId",
                table: "TreatmentLibrary_User",
                column: "TreatmentLibraryId",
                principalTable: "TreatmentLibrary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
