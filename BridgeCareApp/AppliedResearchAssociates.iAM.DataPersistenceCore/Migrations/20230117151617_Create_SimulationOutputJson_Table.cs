using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class Create_SimulationOutputJson_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SimulationOutputJson",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Output = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OutputType = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimulationOutputJson", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SimulationOutputJson_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SimulationOutputJson_Id",
                table: "SimulationOutputJson",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SimulationOutputJson_SimulationId",
                table: "SimulationOutputJson",
                column: "SimulationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SimulationOutputJson");
        }
    }
}
