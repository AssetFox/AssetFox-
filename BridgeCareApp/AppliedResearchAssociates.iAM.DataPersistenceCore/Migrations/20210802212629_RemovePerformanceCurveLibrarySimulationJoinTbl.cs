using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class RemovePerformanceCurveLibrarySimulationJoinTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PerformanceCurveLibrary_Simulation");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PerformanceCurveLibrary_Simulation",
                columns: table => new
                {
                    PerformanceCurveLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerformanceCurveLibrary_Simulation", x => new { x.PerformanceCurveLibraryId, x.SimulationId });
                    table.ForeignKey(
                        name: "FK_PerformanceCurveLibrary_Simulation_PerformanceCurveLibrary_PerformanceCurveLibraryId",
                        column: x => x.PerformanceCurveLibraryId,
                        principalTable: "PerformanceCurveLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerformanceCurveLibrary_Simulation_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceCurveLibrary_Simulation_PerformanceCurveLibraryId",
                table: "PerformanceCurveLibrary_Simulation",
                column: "PerformanceCurveLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceCurveLibrary_Simulation_SimulationId",
                table: "PerformanceCurveLibrary_Simulation",
                column: "SimulationId",
                unique: true);
        }
    }
}
