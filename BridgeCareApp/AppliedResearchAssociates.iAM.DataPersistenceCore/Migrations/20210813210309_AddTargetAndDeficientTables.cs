using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AddTargetAndDeficientTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPercentagePair_ScenarioBudget_ScenarioBudgetId",
                table: "BudgetPercentagePair");

            migrationBuilder.DropForeignKey(
                name: "FK_CommittedProject_ScenarioBudget_ScenarioBudgetId",
                table: "CommittedProject");

            migrationBuilder.DropForeignKey(
                name: "FK_ScenarioTreatmentCost_ScenarioSelectableTreatment_ScenarioSelectableTreatmentId",
                table: "ScenarioTreatmentCost");

            migrationBuilder.DropTable(
                name: "CriterionLibrary_ScenarioBudget");

            migrationBuilder.DropTable(
                name: "DeficientConditionGoalLibrary_Simulation");

            migrationBuilder.DropTable(
                name: "ScenarioBudgetAmount");

            migrationBuilder.DropTable(
                name: "ScenarioSelectableTreatment_ScenarioBudget");

            migrationBuilder.DropTable(
                name: "TargetConditionGoalLibrary_Simulation");

            migrationBuilder.DropTable(
                name: "ScenarioBudget");

            migrationBuilder.DropIndex(
                name: "IX_CommittedProject_ScenarioBudgetId",
                table: "CommittedProject");

            migrationBuilder.DropIndex(
                name: "IX_BudgetPercentagePair_ScenarioBudgetId",
                table: "BudgetPercentagePair");

            migrationBuilder.DropColumn(
                name: "ScenarioBudgetId",
                table: "CommittedProject");

            migrationBuilder.DropColumn(
                name: "ScenarioBudgetId",
                table: "BudgetPercentagePair");

            migrationBuilder.RenameColumn(
                name: "ScenarioSelectableTreatmentId",
                table: "ScenarioTreatmentCost",
                newName: "ScenarioTreatmentId");

            migrationBuilder.RenameIndex(
                name: "IX_ScenarioTreatmentCost_ScenarioSelectableTreatmentId",
                table: "ScenarioTreatmentCost",
                newName: "IX_ScenarioTreatmentCost_ScenarioTreatmentId");

            migrationBuilder.AddColumn<Guid>(
                name: "ScenarioTreatmentId",
                table: "ScenarioSelectableTreatment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "BudgetLibrary_Simulation",
                columns: table => new
                {
                    BudgetLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetLibrary_Simulation", x => new { x.BudgetLibraryId, x.SimulationId });
                    table.ForeignKey(
                        name: "FK_BudgetLibrary_Simulation_BudgetLibrary_BudgetLibraryId",
                        column: x => x.BudgetLibraryId,
                        principalTable: "BudgetLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetLibrary_Simulation_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "ScenarioTreatment_Budget",
                columns: table => new
                {
                    ScenarioSelectableTreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioTreatment_Budget", x => new { x.ScenarioSelectableTreatmentId, x.BudgetId });
                    table.ForeignKey(
                        name: "FK_ScenarioTreatment_Budget_Budget_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScenarioTreatment_Budget_ScenarioSelectableTreatment_ScenarioSelectableTreatmentId",
                        column: x => x.ScenarioSelectableTreatmentId,
                        principalTable: "ScenarioSelectableTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Treatment_Budget",
                columns: table => new
                {
                    SelectableTreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Treatment_Budget", x => new { x.SelectableTreatmentId, x.BudgetId });
                    table.ForeignKey(
                        name: "FK_Treatment_Budget_Budget_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Treatment_Budget_SelectableTreatment_SelectableTreatmentId",
                        column: x => x.SelectableTreatmentId,
                        principalTable: "SelectableTreatment",
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
                name: "IX_BudgetLibrary_Simulation_BudgetLibraryId",
                table: "BudgetLibrary_Simulation",
                column: "BudgetLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetLibrary_Simulation_SimulationId",
                table: "BudgetLibrary_Simulation",
                column: "SimulationId",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioTreatment_Budget_BudgetId",
                table: "ScenarioTreatment_Budget",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioTreatment_Budget_ScenarioSelectableTreatmentId",
                table: "ScenarioTreatment_Budget",
                column: "ScenarioSelectableTreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Treatment_Budget_BudgetId",
                table: "Treatment_Budget",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Treatment_Budget_SelectableTreatmentId",
                table: "Treatment_Budget",
                column: "SelectableTreatmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScenarioTreatmentCost_ScenarioSelectableTreatment_ScenarioTreatmentId",
                table: "ScenarioTreatmentCost",
                column: "ScenarioTreatmentId",
                principalTable: "ScenarioSelectableTreatment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScenarioTreatmentCost_ScenarioSelectableTreatment_ScenarioTreatmentId",
                table: "ScenarioTreatmentCost");

            migrationBuilder.DropTable(
                name: "BudgetLibrary_Simulation");

            migrationBuilder.DropTable(
                name: "CriterionLibrary_ScenarioDeficientConditionGoal");

            migrationBuilder.DropTable(
                name: "CriterionLibrary_ScenarioTargetConditionGoal");

            migrationBuilder.DropTable(
                name: "ScenarioTreatment_Budget");

            migrationBuilder.DropTable(
                name: "Treatment_Budget");

            migrationBuilder.DropTable(
                name: "ScenarioDeficientConditionGoal");

            migrationBuilder.DropTable(
                name: "ScenarioTargetConditionGoals");

            migrationBuilder.DropColumn(
                name: "ScenarioTreatmentId",
                table: "ScenarioSelectableTreatment");

            migrationBuilder.RenameColumn(
                name: "ScenarioTreatmentId",
                table: "ScenarioTreatmentCost",
                newName: "ScenarioSelectableTreatmentId");

            migrationBuilder.RenameIndex(
                name: "IX_ScenarioTreatmentCost_ScenarioTreatmentId",
                table: "ScenarioTreatmentCost",
                newName: "IX_ScenarioTreatmentCost_ScenarioSelectableTreatmentId");

            migrationBuilder.AddColumn<Guid>(
                name: "ScenarioBudgetId",
                table: "CommittedProject",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ScenarioBudgetId",
                table: "BudgetPercentagePair",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "DeficientConditionGoalLibrary_Simulation",
                columns: table => new
                {
                    DeficientConditionGoalLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeficientConditionGoalLibrary_Simulation", x => new { x.DeficientConditionGoalLibraryId, x.SimulationId });
                    table.ForeignKey(
                        name: "FK_DeficientConditionGoalLibrary_Simulation_DeficientConditionGoalLibrary_DeficientConditionGoalLibraryId",
                        column: x => x.DeficientConditionGoalLibraryId,
                        principalTable: "DeficientConditionGoalLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeficientConditionGoalLibrary_Simulation_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioBudget",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioBudget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioBudget_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_ScenarioBudget",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioBudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_ScenarioBudget", x => new { x.CriterionLibraryId, x.ScenarioBudgetId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioBudget_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioBudget_ScenarioBudget_ScenarioBudgetId",
                        column: x => x.ScenarioBudgetId,
                        principalTable: "ScenarioBudget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioBudgetAmount",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScenarioBudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioBudgetAmount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioBudgetAmount_ScenarioBudget_ScenarioBudgetId",
                        column: x => x.ScenarioBudgetId,
                        principalTable: "ScenarioBudget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioSelectableTreatment_ScenarioBudget",
                columns: table => new
                {
                    ScenarioSelectableTreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioBudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioSelectableTreatment_ScenarioBudget", x => new { x.ScenarioSelectableTreatmentId, x.ScenarioBudgetId });
                    table.ForeignKey(
                        name: "FK_ScenarioSelectableTreatment_ScenarioBudget_ScenarioBudget_ScenarioBudgetId",
                        column: x => x.ScenarioBudgetId,
                        principalTable: "ScenarioBudget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScenarioSelectableTreatment_ScenarioBudget_ScenarioSelectableTreatment_ScenarioSelectableTreatmentId",
                        column: x => x.ScenarioSelectableTreatmentId,
                        principalTable: "ScenarioSelectableTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommittedProject_ScenarioBudgetId",
                table: "CommittedProject",
                column: "ScenarioBudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPercentagePair_ScenarioBudgetId",
                table: "BudgetPercentagePair",
                column: "ScenarioBudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioBudget_CriterionLibraryId",
                table: "CriterionLibrary_ScenarioBudget",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioBudget_ScenarioBudgetId",
                table: "CriterionLibrary_ScenarioBudget",
                column: "ScenarioBudgetId",
                unique: true);

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
                name: "IX_ScenarioBudget_SimulationId",
                table: "ScenarioBudget",
                column: "SimulationId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioBudgetAmount_ScenarioBudgetId",
                table: "ScenarioBudgetAmount",
                column: "ScenarioBudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioSelectableTreatment_ScenarioBudget_ScenarioBudgetId",
                table: "ScenarioSelectableTreatment_ScenarioBudget",
                column: "ScenarioBudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioSelectableTreatment_ScenarioBudget_ScenarioSelectableTreatmentId",
                table: "ScenarioSelectableTreatment_ScenarioBudget",
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
                name: "FK_BudgetPercentagePair_ScenarioBudget_ScenarioBudgetId",
                table: "BudgetPercentagePair",
                column: "ScenarioBudgetId",
                principalTable: "ScenarioBudget",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommittedProject_ScenarioBudget_ScenarioBudgetId",
                table: "CommittedProject",
                column: "ScenarioBudgetId",
                principalTable: "ScenarioBudget",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScenarioTreatmentCost_ScenarioSelectableTreatment_ScenarioSelectableTreatmentId",
                table: "ScenarioTreatmentCost",
                column: "ScenarioSelectableTreatmentId",
                principalTable: "ScenarioSelectableTreatment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
