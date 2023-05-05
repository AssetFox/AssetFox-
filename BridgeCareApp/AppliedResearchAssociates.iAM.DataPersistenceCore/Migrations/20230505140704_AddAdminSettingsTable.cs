using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AddAdminSettingsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
               name: "AdminSettings",
               columns: table => new
               {
                   ID = table.Column<int>(type: "int", nullable: false)
                       .Annotation("SqlServer:Identity", "1, 1"),
                   ImplementationName = table.Column<string>(type: "varchar(max)", nullable: true),
                   SiteLogo = table.Column<string>(type: "image", nullable: true),
                   ImplementationLogo = table.Column<string>(type: "image", nullable: true),
                   PrimaryNetwork = table.Column<string>(type: "varchar(max)", nullable: true),
                   KeyFields = table.Column<string>(type: "varchar(max)", nullable: true),
                   InventoryReportNames = table.Column<string>(type: "varchar(max)", nullable: true),
                   SimulationReportNames = table.Column<string>(type: "varchar(max)", nullable: true)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_AdminSiteSettings", x => x.ID);
               });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminSiteSettings");
        }
    }
}
