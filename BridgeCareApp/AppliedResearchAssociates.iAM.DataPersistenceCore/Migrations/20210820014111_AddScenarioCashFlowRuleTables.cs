using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AddScenarioCashFlowRuleTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CashFlowRuleLibrary_Simulation");

            migrationBuilder.CreateTable(
                name: "ScenarioCashFlowRule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioCashFlowRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioCashFlowRule_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_ScenarioCashFlowRule",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioCashFlowRuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_ScenarioCashFlowRule", x => new { x.CriterionLibraryId, x.ScenarioCashFlowRuleId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioCashFlowRule_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioCashFlowRule_ScenarioCashFlowRule_ScenarioCashFlowRuleId",
                        column: x => x.ScenarioCashFlowRuleId,
                        principalTable: "ScenarioCashFlowRule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioCashFlowDistributionRule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioCashFlowRuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DurationInYears = table.Column<int>(type: "int", nullable: false),
                    CostCeiling = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    YearlyPercentages = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioCashFlowDistributionRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioCashFlowDistributionRule_ScenarioCashFlowRule_ScenarioCashFlowRuleId",
                        column: x => x.ScenarioCashFlowRuleId,
                        principalTable: "ScenarioCashFlowRule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioCashFlowRule_CriterionLibraryId",
                table: "CriterionLibrary_ScenarioCashFlowRule",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioCashFlowRule_ScenarioCashFlowRuleId",
                table: "CriterionLibrary_ScenarioCashFlowRule",
                column: "ScenarioCashFlowRuleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCashFlowDistributionRule_ScenarioCashFlowRuleId",
                table: "ScenarioCashFlowDistributionRule",
                column: "ScenarioCashFlowRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCashFlowRule_SimulationId",
                table: "ScenarioCashFlowRule",
                column: "SimulationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CriterionLibrary_ScenarioCashFlowRule");

            migrationBuilder.DropTable(
                name: "ScenarioCashFlowDistributionRule");

            migrationBuilder.DropTable(
                name: "ScenarioCashFlowRule");

            migrationBuilder.CreateTable(
                name: "CashFlowRuleLibrary_Simulation",
                columns: table => new
                {
                    CashFlowRuleLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashFlowRuleLibrary_Simulation", x => new { x.CashFlowRuleLibraryId, x.SimulationId });
                    table.ForeignKey(
                        name: "FK_CashFlowRuleLibrary_Simulation_CashFlowRuleLibrary_CashFlowRuleLibraryId",
                        column: x => x.CashFlowRuleLibraryId,
                        principalTable: "CashFlowRuleLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CashFlowRuleLibrary_Simulation_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CashFlowRuleLibrary_Simulation_CashFlowRuleLibraryId",
                table: "CashFlowRuleLibrary_Simulation",
                column: "CashFlowRuleLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CashFlowRuleLibrary_Simulation_SimulationId",
                table: "CashFlowRuleLibrary_Simulation",
                column: "SimulationId",
                unique: true);
        }
    }
}
