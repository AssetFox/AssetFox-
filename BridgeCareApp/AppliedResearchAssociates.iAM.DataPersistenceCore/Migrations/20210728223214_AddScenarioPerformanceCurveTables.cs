using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AddScenarioPerformanceCurveTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScenarioPerformanceCurve",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Shift = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioPerformanceCurve", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioPerformanceCurve_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScenarioPerformanceCurve_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_ScenarioPerformanceCurve",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioPerformanceCurveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_ScenarioPerformanceCurve", x => new { x.CriterionLibraryId, x.ScenarioPerformanceCurveId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioPerformanceCurve_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioPerformanceCurve_ScenarioPerformanceCurve_ScenarioPerformanceCurveId",
                        column: x => x.ScenarioPerformanceCurveId,
                        principalTable: "ScenarioPerformanceCurve",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioPerformanceCurve_Equation",
                columns: table => new
                {
                    EquationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioPerformanceCurveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioPerformanceCurve_Equation", x => new { x.ScenarioPerformanceCurveId, x.EquationId });
                    table.ForeignKey(
                        name: "FK_ScenarioPerformanceCurve_Equation_Equation_EquationId",
                        column: x => x.EquationId,
                        principalTable: "Equation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScenarioPerformanceCurve_Equation_ScenarioPerformanceCurve_ScenarioPerformanceCurveId",
                        column: x => x.ScenarioPerformanceCurveId,
                        principalTable: "ScenarioPerformanceCurve",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioPerformanceCurve_CriterionLibraryId",
                table: "CriterionLibrary_ScenarioPerformanceCurve",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioPerformanceCurve_ScenarioPerformanceCurveId",
                table: "CriterionLibrary_ScenarioPerformanceCurve",
                column: "ScenarioPerformanceCurveId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioPerformanceCurve_AttributeId",
                table: "ScenarioPerformanceCurve",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioPerformanceCurve_SimulationId",
                table: "ScenarioPerformanceCurve",
                column: "SimulationId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioPerformanceCurve_Equation_EquationId",
                table: "ScenarioPerformanceCurve_Equation",
                column: "EquationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioPerformanceCurve_Equation_ScenarioPerformanceCurveId",
                table: "ScenarioPerformanceCurve_Equation",
                column: "ScenarioPerformanceCurveId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CriterionLibrary_ScenarioPerformanceCurve");

            migrationBuilder.DropTable(
                name: "ScenarioPerformanceCurve_Equation");

            migrationBuilder.DropTable(
                name: "ScenarioPerformanceCurve");
        }
    }
}
