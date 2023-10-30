using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class UpdateTreatmentSupersedeRule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PreventSelectableTreatmentId",
                table: "TreatmentSupersedeRule",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PreventScenarioSelectableTreatmentId",
                table: "ScenarioTreatmentSupersedeRule",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentSupersedeRule_PreventSelectableTreatmentId",
                table: "TreatmentSupersedeRule",
                column: "PreventSelectableTreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioTreatmentSupersedeRule_PreventScenarioSelectableTreatmentId",
                table: "ScenarioTreatmentSupersedeRule",
                column: "PreventScenarioSelectableTreatmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScenarioTreatmentSupersedeRule_ScenarioSelectableTreatment_PreventScenarioSelectableTreatmentId",
                table: "ScenarioTreatmentSupersedeRule",
                column: "PreventScenarioSelectableTreatmentId",
                principalTable: "ScenarioSelectableTreatment",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentSupersedeRule_SelectableTreatment_PreventSelectableTreatmentId",
                table: "TreatmentSupersedeRule",
                column: "PreventSelectableTreatmentId",
                principalTable: "SelectableTreatment",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScenarioTreatmentSupersedeRule_ScenarioSelectableTreatment_PreventScenarioSelectableTreatmentId",
                table: "ScenarioTreatmentSupersedeRule");

            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentSupersedeRule_SelectableTreatment_PreventSelectableTreatmentId",
                table: "TreatmentSupersedeRule");

            migrationBuilder.DropIndex(
                name: "IX_TreatmentSupersedeRule_PreventSelectableTreatmentId",
                table: "TreatmentSupersedeRule");

            migrationBuilder.DropIndex(
                name: "IX_ScenarioTreatmentSupersedeRule_PreventScenarioSelectableTreatmentId",
                table: "ScenarioTreatmentSupersedeRule");

            migrationBuilder.DropColumn(
                name: "PreventSelectableTreatmentId",
                table: "TreatmentSupersedeRule");

            migrationBuilder.DropColumn(
                name: "PreventScenarioSelectableTreatmentId",
                table: "ScenarioTreatmentSupersedeRule");
        }
    }
}
