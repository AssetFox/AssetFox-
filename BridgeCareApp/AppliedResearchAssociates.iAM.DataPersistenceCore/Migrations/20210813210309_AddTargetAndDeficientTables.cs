using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AddTargetAndDeficientTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeficientConditionGoalLibrary_Simulation");

            migrationBuilder.DropTable(
                name: "TargetConditionGoalLibrary_Simulation");

            migrationBuilder.CreateTable(
                name: "ScenarioDeficientConditionGoal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllowedDeficientPercentage = table.Column<double>(type: "float", nullable: false),
                    DeficientLimit = table.Column<double>(type: "float", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioDeficientConditionGoal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioDeficientConditionGoal_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScenarioDeficientConditionGoal_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioTargetConditionGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Target = table.Column<double>(type: "float", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioTargetConditionGoals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioTargetConditionGoals_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScenarioTargetConditionGoals_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_ScenarioDeficientConditionGoal",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioDeficientConditionGoalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_ScenarioDeficientConditionGoal", x => new { x.CriterionLibraryId, x.ScenarioDeficientConditionGoalId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioDeficientConditionGoal_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioDeficientConditionGoal_ScenarioDeficientConditionGoal_ScenarioDeficientConditionGoalId",
                        column: x => x.ScenarioDeficientConditionGoalId,
                        principalTable: "ScenarioDeficientConditionGoal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_ScenarioTargetConditionGoal",
                columns: table => new
                {
                    ScenarioTargetConditionGoalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_ScenarioTargetConditionGoal", x => new { x.CriterionLibraryId, x.ScenarioTargetConditionGoalId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioTargetConditionGoal_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioTargetConditionGoal_ScenarioTargetConditionGoals_ScenarioTargetConditionGoalId",
                        column: x => x.ScenarioTargetConditionGoalId,
                        principalTable: "ScenarioTargetConditionGoals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioDeficientConditionGoal_CriterionLibraryId",
                table: "CriterionLibrary_ScenarioDeficientConditionGoal",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioDeficientConditionGoal_ScenarioDeficientConditionGoalId",
                table: "CriterionLibrary_ScenarioDeficientConditionGoal",
                column: "ScenarioDeficientConditionGoalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioTargetConditionGoal_CriterionLibraryId",
                table: "CriterionLibrary_ScenarioTargetConditionGoal",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioTargetConditionGoal_ScenarioTargetConditionGoalId",
                table: "CriterionLibrary_ScenarioTargetConditionGoal",
                column: "ScenarioTargetConditionGoalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioDeficientConditionGoal_AttributeId",
                table: "ScenarioDeficientConditionGoal",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioDeficientConditionGoal_SimulationId",
                table: "ScenarioDeficientConditionGoal",
                column: "SimulationId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioTargetConditionGoals_AttributeId",
                table: "ScenarioTargetConditionGoals",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioTargetConditionGoals_SimulationId",
                table: "ScenarioTargetConditionGoals",
                column: "SimulationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CriterionLibrary_ScenarioDeficientConditionGoal");

            migrationBuilder.DropTable(
                name: "CriterionLibrary_ScenarioTargetConditionGoal");

            migrationBuilder.DropTable(
                name: "ScenarioDeficientConditionGoal");

            migrationBuilder.DropTable(
                name: "ScenarioTargetConditionGoals");

            migrationBuilder.CreateIndex(
                name: "IX_DeficientConditionGoalLibrary_Simulation_DeficientConditionGoalLibraryId",
                table: "DeficientConditionGoalLibrary_Simulation",
                column: "DeficientConditionGoalLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_DeficientConditionGoalLibrary_Simulation_SimulationId",
                table: "DeficientConditionGoalLibrary_Simulation",
                column: "SimulationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TargetConditionGoalLibrary_Simulation_SimulationId",
                table: "TargetConditionGoalLibrary_Simulation",
                column: "SimulationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TargetConditionGoalLibrary_Simulation_TargetConditionGoalLibraryId",
                table: "TargetConditionGoalLibrary_Simulation",
                column: "TargetConditionGoalLibraryId");
        }
    }
}
