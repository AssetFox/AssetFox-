using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class ChangesFromMaster20210329 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportIndex",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReportTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportIndex", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportIndex_Simulation_SimulationID",
                        column: x => x.SimulationID,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportIndex_SimulationID",
                table: "ReportIndex",
                column: "SimulationID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportIndex");
        }
    }
}
