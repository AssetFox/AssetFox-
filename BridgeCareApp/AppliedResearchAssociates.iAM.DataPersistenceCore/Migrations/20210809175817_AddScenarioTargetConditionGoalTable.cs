using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AddScenarioTargetConditionGoalTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CriterionLibrary_ScenarioTreatmentSupersession_ScenarioTreatmentSupersessionEntity_TreatmentSupersessionId",
                table: "CriterionLibrary_ScenarioTreatmentSupersession");

            migrationBuilder.DropForeignKey(
                name: "FK_ScenarioTreatmentSchedulingEntity_ScenarioSelectableTreatment_ScenarioSelectableTreatmentId",
                table: "ScenarioTreatmentSchedulingEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_ScenarioTreatmentSupersessionEntity_ScenarioSelectableTreatment_ScenarioSelectableTreatmentId",
                table: "ScenarioTreatmentSupersessionEntity");

            migrationBuilder.DropTable(
                name: "TargetConditionGoalLibrary_Simulation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScenarioTreatmentSupersessionEntity",
                table: "ScenarioTreatmentSupersessionEntity");

            migrationBuilder.DropIndex(
                name: "IX_ScenarioTreatmentSupersessionEntity_ScenarioSelectableTreatmentId",
                table: "ScenarioTreatmentSupersessionEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScenarioTreatmentSchedulingEntity",
                table: "ScenarioTreatmentSchedulingEntity");

            migrationBuilder.DropIndex(
                name: "IX_ScenarioTreatmentSchedulingEntity_ScenarioSelectableTreatmentId",
                table: "ScenarioTreatmentSchedulingEntity");

            migrationBuilder.DropColumn(
                name: "ScenarioSelectableTreatmentId",
                table: "ScenarioTreatmentSupersessionEntity");

            migrationBuilder.DropColumn(
                name: "ScenarioSelectableTreatmentId",
                table: "ScenarioTreatmentSchedulingEntity");

            migrationBuilder.RenameTable(
                name: "ScenarioTreatmentSupersessionEntity",
                newName: "ScenarioTreatmentSupersession");

            migrationBuilder.RenameTable(
                name: "ScenarioTreatmentSchedulingEntity",
                newName: "ScenarioTreatmentScheduling");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScenarioTreatmentSupersession",
                table: "ScenarioTreatmentSupersession",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScenarioTreatmentScheduling",
                table: "ScenarioTreatmentScheduling",
                column: "Id");

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
                name: "IX_ScenarioTreatmentSupersession_TreatmentId",
                table: "ScenarioTreatmentSupersession",
                column: "TreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioTreatmentScheduling_TreatmentId",
                table: "ScenarioTreatmentScheduling",
                column: "TreatmentId");

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
                name: "IX_ScenarioTargetConditionGoals_AttributeId",
                table: "ScenarioTargetConditionGoals",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioTargetConditionGoals_SimulationId",
                table: "ScenarioTargetConditionGoals",
                column: "SimulationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CriterionLibrary_ScenarioTreatmentSupersession_ScenarioTreatmentSupersession_TreatmentSupersessionId",
                table: "CriterionLibrary_ScenarioTreatmentSupersession",
                column: "TreatmentSupersessionId",
                principalTable: "ScenarioTreatmentSupersession",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScenarioTreatmentScheduling_ScenarioSelectableTreatment_TreatmentId",
                table: "ScenarioTreatmentScheduling",
                column: "TreatmentId",
                principalTable: "ScenarioSelectableTreatment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScenarioTreatmentSupersession_ScenarioSelectableTreatment_TreatmentId",
                table: "ScenarioTreatmentSupersession",
                column: "TreatmentId",
                principalTable: "ScenarioSelectableTreatment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CriterionLibrary_ScenarioTreatmentSupersession_ScenarioTreatmentSupersession_TreatmentSupersessionId",
                table: "CriterionLibrary_ScenarioTreatmentSupersession");

            migrationBuilder.DropForeignKey(
                name: "FK_ScenarioTreatmentScheduling_ScenarioSelectableTreatment_TreatmentId",
                table: "ScenarioTreatmentScheduling");

            migrationBuilder.DropForeignKey(
                name: "FK_ScenarioTreatmentSupersession_ScenarioSelectableTreatment_TreatmentId",
                table: "ScenarioTreatmentSupersession");

            migrationBuilder.DropTable(
                name: "CriterionLibrary_ScenarioTargetConditionGoal");

            migrationBuilder.DropTable(
                name: "ScenarioTargetConditionGoals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScenarioTreatmentSupersession",
                table: "ScenarioTreatmentSupersession");

            migrationBuilder.DropIndex(
                name: "IX_ScenarioTreatmentSupersession_TreatmentId",
                table: "ScenarioTreatmentSupersession");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScenarioTreatmentScheduling",
                table: "ScenarioTreatmentScheduling");

            migrationBuilder.DropIndex(
                name: "IX_ScenarioTreatmentScheduling_TreatmentId",
                table: "ScenarioTreatmentScheduling");

            migrationBuilder.RenameTable(
                name: "ScenarioTreatmentSupersession",
                newName: "ScenarioTreatmentSupersessionEntity");

            migrationBuilder.RenameTable(
                name: "ScenarioTreatmentScheduling",
                newName: "ScenarioTreatmentSchedulingEntity");

            migrationBuilder.AddColumn<Guid>(
                name: "ScenarioSelectableTreatmentId",
                table: "ScenarioTreatmentSupersessionEntity",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ScenarioSelectableTreatmentId",
                table: "ScenarioTreatmentSchedulingEntity",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScenarioTreatmentSupersessionEntity",
                table: "ScenarioTreatmentSupersessionEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScenarioTreatmentSchedulingEntity",
                table: "ScenarioTreatmentSchedulingEntity",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TargetConditionGoalLibrary_Simulation",
                columns: table => new
                {
                    TargetConditionGoalLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetConditionGoalLibrary_Simulation", x => new { x.TargetConditionGoalLibraryId, x.SimulationId });
                    table.ForeignKey(
                        name: "FK_TargetConditionGoalLibrary_Simulation_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TargetConditionGoalLibrary_Simulation_TargetConditionGoalLibrary_TargetConditionGoalLibraryId",
                        column: x => x.TargetConditionGoalLibraryId,
                        principalTable: "TargetConditionGoalLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioTreatmentSupersessionEntity_ScenarioSelectableTreatmentId",
                table: "ScenarioTreatmentSupersessionEntity",
                column: "ScenarioSelectableTreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioTreatmentSchedulingEntity_ScenarioSelectableTreatmentId",
                table: "ScenarioTreatmentSchedulingEntity",
                column: "ScenarioSelectableTreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetConditionGoalLibrary_Simulation_SimulationId",
                table: "TargetConditionGoalLibrary_Simulation",
                column: "SimulationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TargetConditionGoalLibrary_Simulation_TargetConditionGoalLibraryId",
                table: "TargetConditionGoalLibrary_Simulation",
                column: "TargetConditionGoalLibraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CriterionLibrary_ScenarioTreatmentSupersession_ScenarioTreatmentSupersessionEntity_TreatmentSupersessionId",
                table: "CriterionLibrary_ScenarioTreatmentSupersession",
                column: "TreatmentSupersessionId",
                principalTable: "ScenarioTreatmentSupersessionEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScenarioTreatmentSchedulingEntity_ScenarioSelectableTreatment_ScenarioSelectableTreatmentId",
                table: "ScenarioTreatmentSchedulingEntity",
                column: "ScenarioSelectableTreatmentId",
                principalTable: "ScenarioSelectableTreatment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScenarioTreatmentSupersessionEntity_ScenarioSelectableTreatment_ScenarioSelectableTreatmentId",
                table: "ScenarioTreatmentSupersessionEntity",
                column: "ScenarioSelectableTreatmentId",
                principalTable: "ScenarioSelectableTreatment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
