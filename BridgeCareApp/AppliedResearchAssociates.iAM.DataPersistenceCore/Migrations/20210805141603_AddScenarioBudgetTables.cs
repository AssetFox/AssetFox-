using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AddScenarioBudgetTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPercentagePair_Budget_BudgetId",
                table: "BudgetPercentagePair");

            migrationBuilder.DropForeignKey(
                name: "FK_CommittedProject_Budget_BudgetId",
                table: "CommittedProject");

            migrationBuilder.DropForeignKey(
                name: "FK_Treatment_Budget_Budget_BudgetId",
                table: "Treatment_Budget");

            migrationBuilder.DropTable(
                name: "BudgetLibrary_Simulation");

            migrationBuilder.RenameColumn(
                name: "BudgetId",
                table: "Treatment_Budget",
                newName: "ScenarioBudgetId");

            migrationBuilder.RenameIndex(
                name: "IX_Treatment_Budget_BudgetId",
                table: "Treatment_Budget",
                newName: "IX_Treatment_Budget_ScenarioBudgetId");

            migrationBuilder.RenameColumn(
                name: "BudgetId",
                table: "CommittedProject",
                newName: "ScenarioBudgetId");

            migrationBuilder.RenameIndex(
                name: "IX_CommittedProject_BudgetId",
                table: "CommittedProject",
                newName: "IX_CommittedProject_ScenarioBudgetId");

            migrationBuilder.RenameColumn(
                name: "BudgetId",
                table: "BudgetPercentagePair",
                newName: "ScenarioBudgetId");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetPercentagePair_BudgetId",
                table: "BudgetPercentagePair",
                newName: "IX_BudgetPercentagePair_ScenarioBudgetId");

            migrationBuilder.CreateTable(
                name: "ScenarioBudget",
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
                    table.PrimaryKey("PK_ScenarioBudget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioBudget_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_ScenarioBudget",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioBudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                    ScenarioBudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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
                name: "IX_ScenarioBudget_SimulationId",
                table: "ScenarioBudget",
                column: "SimulationId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioBudgetAmount_ScenarioBudgetId",
                table: "ScenarioBudgetAmount",
                column: "ScenarioBudgetId");

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
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Treatment_Budget_ScenarioBudget_ScenarioBudgetId",
                table: "Treatment_Budget",
                column: "ScenarioBudgetId",
                principalTable: "ScenarioBudget",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPercentagePair_ScenarioBudget_ScenarioBudgetId",
                table: "BudgetPercentagePair");

            migrationBuilder.DropForeignKey(
                name: "FK_CommittedProject_ScenarioBudget_ScenarioBudgetId",
                table: "CommittedProject");

            migrationBuilder.DropForeignKey(
                name: "FK_Treatment_Budget_ScenarioBudget_ScenarioBudgetId",
                table: "Treatment_Budget");

            migrationBuilder.DropTable(
                name: "CriterionLibrary_ScenarioBudget");

            migrationBuilder.DropTable(
                name: "ScenarioBudgetAmount");

            migrationBuilder.DropTable(
                name: "ScenarioBudget");

            migrationBuilder.RenameColumn(
                name: "ScenarioBudgetId",
                table: "Treatment_Budget",
                newName: "BudgetId");

            migrationBuilder.RenameIndex(
                name: "IX_Treatment_Budget_ScenarioBudgetId",
                table: "Treatment_Budget",
                newName: "IX_Treatment_Budget_BudgetId");

            migrationBuilder.RenameColumn(
                name: "ScenarioBudgetId",
                table: "CommittedProject",
                newName: "BudgetId");

            migrationBuilder.RenameIndex(
                name: "IX_CommittedProject_ScenarioBudgetId",
                table: "CommittedProject",
                newName: "IX_CommittedProject_BudgetId");

            migrationBuilder.RenameColumn(
                name: "ScenarioBudgetId",
                table: "BudgetPercentagePair",
                newName: "BudgetId");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetPercentagePair_ScenarioBudgetId",
                table: "BudgetPercentagePair",
                newName: "IX_BudgetPercentagePair_BudgetId");

            migrationBuilder.CreateTable(
                name: "BudgetLibrary_Simulation",
                columns: table => new
                {
                    BudgetLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_BudgetLibrary_Simulation_BudgetLibraryId",
                table: "BudgetLibrary_Simulation",
                column: "BudgetLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetLibrary_Simulation_SimulationId",
                table: "BudgetLibrary_Simulation",
                column: "SimulationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetPercentagePair_Budget_BudgetId",
                table: "BudgetPercentagePair",
                column: "BudgetId",
                principalTable: "Budget",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommittedProject_Budget_BudgetId",
                table: "CommittedProject",
                column: "BudgetId",
                principalTable: "Budget",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Treatment_Budget_Budget_BudgetId",
                table: "Treatment_Budget",
                column: "BudgetId",
                principalTable: "Budget",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
