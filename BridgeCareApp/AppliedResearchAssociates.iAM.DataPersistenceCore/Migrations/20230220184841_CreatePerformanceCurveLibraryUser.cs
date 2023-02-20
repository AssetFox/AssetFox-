using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class CreatePerformanceCurveLibraryUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PerformanceCurveLibrary_User_PerformanceCurveLibrary_PerformanceCurveLibraryId",
                table: "PerformanceCurveLibrary_User");

            migrationBuilder.RenameColumn(
                name: "PerformanceCurveLibraryId",
                table: "PerformanceCurveLibrary_User",
                newName: "LibraryId");

            migrationBuilder.RenameIndex(
                name: "IX_PerformanceCurveLibrary_User_PerformanceCurveLibraryId",
                table: "PerformanceCurveLibrary_User",
                newName: "IX_PerformanceCurveLibrary_User_LibraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_PerformanceCurveLibrary_User_PerformanceCurveLibrary_LibraryId",
                table: "PerformanceCurveLibrary_User",
                column: "LibraryId",
                principalTable: "PerformanceCurveLibrary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PerformanceCurveLibrary_User_PerformanceCurveLibrary_LibraryId",
                table: "PerformanceCurveLibrary_User");

            migrationBuilder.RenameColumn(
                name: "LibraryId",
                table: "PerformanceCurveLibrary_User",
                newName: "PerformanceCurveLibraryId");

            migrationBuilder.RenameIndex(
                name: "IX_PerformanceCurveLibrary_User_LibraryId",
                table: "PerformanceCurveLibrary_User",
                newName: "IX_PerformanceCurveLibrary_User_PerformanceCurveLibraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_PerformanceCurveLibrary_User_PerformanceCurveLibrary_PerformanceCurveLibraryId",
                table: "PerformanceCurveLibrary_User",
                column: "PerformanceCurveLibraryId",
                principalTable: "PerformanceCurveLibrary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
