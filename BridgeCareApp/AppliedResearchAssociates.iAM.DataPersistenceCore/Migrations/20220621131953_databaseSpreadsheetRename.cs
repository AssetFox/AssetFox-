using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class databaseSpreadsheetRename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ExcelWorksheets",
                table: "ExcelWorksheets");

            migrationBuilder.RenameTable(
                name: "ExcelWorksheets",
                newName: "ExcelWorksheet");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExcelWorksheet",
                table: "ExcelWorksheet",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ExcelWorksheet",
                table: "ExcelWorksheet");

            migrationBuilder.RenameTable(
                name: "ExcelWorksheet",
                newName: "ExcelWorksheets");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExcelWorksheets",
                table: "ExcelWorksheets",
                column: "Id");
        }
    }
}
