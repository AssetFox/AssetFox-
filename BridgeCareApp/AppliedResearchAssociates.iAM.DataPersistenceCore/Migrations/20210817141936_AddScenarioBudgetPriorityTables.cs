using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AddScenarioBudgetPriorityTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetPriorityLibrary_Simulation");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPercentagePair_BudgetPriority_BudgetPriorityId",
                table: "BudgetPercentagePair");

            migrationBuilder.DropIndex(
                name: "IX_BudgetPercentagePair_BudgetPriorityId",
                table: "BudgetPercentagePair");

            migrationBuilder.DropColumn(
                name: "BudgetPriorityId",
                table: "BudgetPercentagePair");

            migrationBuilder.AddColumn<Guid>(
                name: "ScenarioBudgetPriorityId",
                table: "BudgetPercentagePair",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ScenarioBudgetPriority",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriorityLevel = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioBudgetPriority", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioBudgetPriority_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_ScenarioBudgetPriority",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioBudgetPriorityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_ScenarioBudgetPriority", x => new { x.CriterionLibraryId, x.ScenarioBudgetPriorityId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioBudgetPriority_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioBudgetPriority_ScenarioBudgetPriority_ScenarioBudgetPriorityId",
                        column: x => x.ScenarioBudgetPriorityId,
                        principalTable: "ScenarioBudgetPriority",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetPercentagePair_ScenarioBudgetPriority_ScenarioBudgetPriorityId",
                table: "BudgetPercentagePair",
                column: "ScenarioBudgetPriorityId",
                principalTable: "ScenarioBudgetPriority",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPercentagePair_ScenarioBudgetPriorityId",
                table: "BudgetPercentagePair",
                column: "ScenarioBudgetPriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioBudgetPriority_CriterionLibraryId",
                table: "CriterionLibrary_ScenarioBudgetPriority",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioBudgetPriority_ScenarioBudgetPriorityId",
                table: "CriterionLibrary_ScenarioBudgetPriority",
                column: "ScenarioBudgetPriorityId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioBudgetPriority_SimulationId",
                table: "ScenarioBudgetPriority",
                column: "SimulationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CriterionLibrary_ScenarioBudgetPriority");

            migrationBuilder.DropTable(
                name: "ScenarioBudgetPriority");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPercentagePair_ScenarioBudgetPriority_ScenarioBudgetPriorityId",
                table: "BudgetPercentagePair");

            migrationBuilder.DropIndex(
                name: "IX_BudgetPercentagePair_ScenarioBudgetPriorityId",
                table: "BudgetPercentagePair");

            migrationBuilder.DropColumn(
                name: "ScenarioBudgetPriorityId",
                table: "BudgetPercentagePair");

            migrationBuilder.AddColumn<Guid>(
                name: "BudgetPriorityId",
                table: "BudgetPercentagePair",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "BudgetPriorityLibrary_Simulation",
                columns: table => new
                {
                    BudgetPriorityLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetPriorityLibrary_Simulation", x => new { x.BudgetPriorityLibraryId, x.SimulationId });
                    table.ForeignKey(
                        name: "FK_BudgetPriorityLibrary_Simulation_BudgetPriorityLibrary_BudgetPriorityLibraryId",
                        column: x => x.BudgetPriorityLibraryId,
                        principalTable: "BudgetPriorityLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetPriorityLibrary_Simulation_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPercentagePair_BudgetPriorityId",
                table: "BudgetPercentagePair",
                column: "BudgetPriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPriorityLibrary_Simulation_BudgetPriorityLibraryId",
                table: "BudgetPriorityLibrary_Simulation",
                column: "BudgetPriorityLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPriorityLibrary_Simulation_SimulationId",
                table: "BudgetPriorityLibrary_Simulation",
                column: "SimulationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetPercentagePair_BudgetPriority_BudgetPriorityId",
                table: "BudgetPercentagePair",
                column: "BudgetPriorityId",
                principalTable: "BudgetPriority",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
