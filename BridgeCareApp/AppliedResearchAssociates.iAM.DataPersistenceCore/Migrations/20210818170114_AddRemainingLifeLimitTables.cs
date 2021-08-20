using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AddRemainingLifeLimitTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RemainingLifeLimitLibrary_Simulation");

            migrationBuilder.AlterColumn<Guid>(
                name: "ScenarioBudgetPriorityId",
                table: "BudgetPercentagePair",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ScenarioRemainingLifeLimit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioRemainingLifeLimit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioRemainingLifeLimit_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScenarioRemainingLifeLimit_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_ScenarioRemainingLifeLimit",
                columns: table => new
                {
                    ScenarioRemainingLifeLimitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_ScenarioRemainingLifeLimit", x => new { x.CriterionLibraryId, x.ScenarioRemainingLifeLimitId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioRemainingLifeLimit_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioRemainingLifeLimit_ScenarioRemainingLifeLimit_ScenarioRemainingLifeLimitId",
                        column: x => x.ScenarioRemainingLifeLimitId,
                        principalTable: "ScenarioRemainingLifeLimit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioRemainingLifeLimit_CriterionLibraryId",
                table: "CriterionLibrary_ScenarioRemainingLifeLimit",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioRemainingLifeLimit_ScenarioRemainingLifeLimitId",
                table: "CriterionLibrary_ScenarioRemainingLifeLimit",
                column: "ScenarioRemainingLifeLimitId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioRemainingLifeLimit_AttributeId",
                table: "ScenarioRemainingLifeLimit",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioRemainingLifeLimit_SimulationId",
                table: "ScenarioRemainingLifeLimit",
                column: "SimulationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CriterionLibrary_ScenarioRemainingLifeLimit");

            migrationBuilder.DropTable(
                name: "ScenarioRemainingLifeLimit");

            migrationBuilder.AlterColumn<Guid>(
                name: "ScenarioBudgetPriorityId",
                table: "BudgetPercentagePair",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateTable(
                name: "RemainingLifeLimitLibrary_Simulation",
                columns: table => new
                {
                    RemainingLifeLimitLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemainingLifeLimitLibrary_Simulation", x => new { x.RemainingLifeLimitLibraryId, x.SimulationId });
                    table.ForeignKey(
                        name: "FK_RemainingLifeLimitLibrary_Simulation_RemainingLifeLimitLibrary_RemainingLifeLimitLibraryId",
                        column: x => x.RemainingLifeLimitLibraryId,
                        principalTable: "RemainingLifeLimitLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RemainingLifeLimitLibrary_Simulation_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RemainingLifeLimitLibrary_Simulation_RemainingLifeLimitLibraryId",
                table: "RemainingLifeLimitLibrary_Simulation",
                column: "RemainingLifeLimitLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_RemainingLifeLimitLibrary_Simulation_SimulationId",
                table: "RemainingLifeLimitLibrary_Simulation",
                column: "SimulationId",
                unique: true);
        }
    }
}
