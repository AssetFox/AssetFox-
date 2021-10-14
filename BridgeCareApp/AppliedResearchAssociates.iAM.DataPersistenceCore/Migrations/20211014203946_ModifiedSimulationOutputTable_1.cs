using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class ModifiedSimulationOutputTable_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SimulationOutput",
                table: "SimulationOutput");

            migrationBuilder.DropIndex(
                name: "IX_SimulationOutput_SimulationId",
                table: "SimulationOutput");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "SimulationOutput",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "OutputType",
                table: "SimulationOutput",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "SelectableTreatment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Preservation",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AssetType",
                table: "SelectableTreatment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Bridge",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "ScenarioSelectableTreatment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Preservation",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AssetType",
                table: "ScenarioSelectableTreatment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Bridge",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SimulationOutput",
                table: "SimulationOutput",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SimulationOutput_Id",
                table: "SimulationOutput",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SimulationOutput_SimulationId",
                table: "SimulationOutput",
                column: "SimulationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SimulationOutput",
                table: "SimulationOutput");

            migrationBuilder.DropIndex(
                name: "IX_SimulationOutput_Id",
                table: "SimulationOutput");

            migrationBuilder.DropIndex(
                name: "IX_SimulationOutput_SimulationId",
                table: "SimulationOutput");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SimulationOutput");

            migrationBuilder.DropColumn(
                name: "OutputType",
                table: "SimulationOutput");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "SelectableTreatment",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "Preservation");

            migrationBuilder.AlterColumn<string>(
                name: "AssetType",
                table: "SelectableTreatment",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "Bridge");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "ScenarioSelectableTreatment",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "Preservation");

            migrationBuilder.AlterColumn<string>(
                name: "AssetType",
                table: "ScenarioSelectableTreatment",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "Bridge");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SimulationOutput",
                table: "SimulationOutput",
                column: "SimulationId");

            migrationBuilder.CreateIndex(
                name: "IX_SimulationOutput_SimulationId",
                table: "SimulationOutput",
                column: "SimulationId",
                unique: true);
        }
    }
}
