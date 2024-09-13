using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class Add_ReportType_Key_To_SImulationReportDetailEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SimulationReportDetail",
                table: "SimulationReportDetail");

            migrationBuilder.AlterColumn<string>(
                name: "ReportType",
                table: "SimulationReportDetail",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SimulationReportDetail",
                table: "SimulationReportDetail",
                columns: new[] { "SimulationId", "ReportType" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SimulationReportDetail",
                table: "SimulationReportDetail");

            migrationBuilder.AlterColumn<string>(
                name: "ReportType",
                table: "SimulationReportDetail",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SimulationReportDetail",
                table: "SimulationReportDetail",
                column: "SimulationId");
        }
    }
}
