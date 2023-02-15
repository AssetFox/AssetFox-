using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class Relational_Json_Relation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SimulationOutputId",
                table: "SimulationOutputJson",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SimulationOutputJson_SimulationOutputId",
                table: "SimulationOutputJson",
                column: "SimulationOutputId");

            migrationBuilder.AddForeignKey(
                name: "FK_SimulationOutputJson_SimulationOutput_SimulationOutputId",
                table: "SimulationOutputJson",
                column: "SimulationOutputId",
                principalTable: "SimulationOutput",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SimulationOutputJson_SimulationOutput_SimulationOutputId",
                table: "SimulationOutputJson");

            migrationBuilder.DropIndex(
                name: "IX_SimulationOutputJson_SimulationOutputId",
                table: "SimulationOutputJson");

            migrationBuilder.DropColumn(
                name: "SimulationOutputId",
                table: "SimulationOutputJson");
        }
    }
}
