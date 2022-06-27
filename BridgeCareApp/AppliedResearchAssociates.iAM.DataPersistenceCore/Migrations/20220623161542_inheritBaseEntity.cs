using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class inheritBaseEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExcelWorksheet_DataSourceId",
                table: "ExcelWorksheet");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "ExcelWorksheet",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ExcelWorksheet",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "ExcelWorksheet",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "ExcelWorksheet",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "DataSource",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "DataSource",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "DataSource",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "DataSource",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_ExcelWorksheet_DataSourceId",
                table: "ExcelWorksheet",
                column: "DataSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ExcelWorksheet_Id",
                table: "ExcelWorksheet",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DataSource_Id",
                table: "DataSource",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExcelWorksheet_DataSourceId",
                table: "ExcelWorksheet");

            migrationBuilder.DropIndex(
                name: "IX_ExcelWorksheet_Id",
                table: "ExcelWorksheet");

            migrationBuilder.DropIndex(
                name: "IX_DataSource_Id",
                table: "DataSource");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ExcelWorksheet");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ExcelWorksheet");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "ExcelWorksheet");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "ExcelWorksheet");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "DataSource");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "DataSource");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "DataSource");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "DataSource");

            migrationBuilder.CreateIndex(
                name: "IX_ExcelWorksheet_DataSourceId",
                table: "ExcelWorksheet",
                column: "DataSourceId",
                unique: true);
        }
    }
}
