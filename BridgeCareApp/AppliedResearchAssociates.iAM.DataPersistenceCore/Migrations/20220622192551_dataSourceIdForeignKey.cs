using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class dataSourceIdForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DataSourceId",
                table: "ExcelWorksheet",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ExcelWorksheet_DataSourceId",
                table: "ExcelWorksheet",
                column: "DataSourceId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ExcelWorksheet_DataSource_DataSourceId",
                table: "ExcelWorksheet",
                column: "DataSourceId",
                principalTable: "DataSource",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExcelWorksheet_DataSource_DataSourceId",
                table: "ExcelWorksheet");

            migrationBuilder.DropIndex(
                name: "IX_ExcelWorksheet_DataSourceId",
                table: "ExcelWorksheet");

            migrationBuilder.DropColumn(
                name: "DataSourceId",
                table: "ExcelWorksheet");
        }
    }
}
