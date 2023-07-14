using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AddPerformanceFactorToSelectedTreatment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PerformanceFactor",
                table: "SelectableTreatment");

            migrationBuilder.DropColumn(
                name: "PerformanceFactor",
                table: "ScenarioSelectableTreatment");

            migrationBuilder.DropColumn(
                name: "PerformanceFactor",
                table: "CommittedProject");

            migrationBuilder.CreateTable(
                name: "ScenarioTreatmentPerformanceFactor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioSelectableTreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Attribute = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PerformanceFactor = table.Column<float>(type: "real", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioTreatmentPerformanceFactor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioTreatmentPerformanceFactor_ScenarioSelectableTreatment_ScenarioSelectableTreatmentId",
                        column: x => x.ScenarioSelectableTreatmentId,
                        principalTable: "ScenarioSelectableTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreatmentPerformanceFactor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Attribute = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PerformanceFactor = table.Column<float>(type: "real", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentPerformanceFactor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreatmentPerformanceFactor_SelectableTreatment_TreatmentId",
                        column: x => x.TreatmentId,
                        principalTable: "SelectableTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioTreatmentPerformanceFactor_ScenarioSelectableTreatmentId",
                table: "ScenarioTreatmentPerformanceFactor",
                column: "ScenarioSelectableTreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentPerformanceFactor_TreatmentId",
                table: "TreatmentPerformanceFactor",
                column: "TreatmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScenarioTreatmentPerformanceFactor");

            migrationBuilder.DropTable(
                name: "TreatmentPerformanceFactor");

            migrationBuilder.AddColumn<double>(
                name: "PerformanceFactor",
                table: "SelectableTreatment",
                type: "float",
                nullable: false,
                defaultValue: 1.0);

            migrationBuilder.AddColumn<double>(
                name: "PerformanceFactor",
                table: "ScenarioSelectableTreatment",
                type: "float",
                nullable: false,
                defaultValue: 1.0);

            migrationBuilder.AddColumn<double>(
                name: "PerformanceFactor",
                table: "CommittedProject",
                type: "float",
                nullable: false,
                defaultValue: 1.0);
        }
    }
}
