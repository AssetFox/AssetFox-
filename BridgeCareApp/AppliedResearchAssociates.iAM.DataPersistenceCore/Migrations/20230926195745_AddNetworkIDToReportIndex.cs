using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AddNetworkIDToReportIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "NetworkID",
                table: "ReportIndex",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NetworkID",
                table: "ReportIndex");
        }
    }
}
