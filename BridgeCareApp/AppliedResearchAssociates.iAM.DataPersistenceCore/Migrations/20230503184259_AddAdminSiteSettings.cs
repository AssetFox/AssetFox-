using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AddAdminSiteSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
               name: "AdminSiteSettings",
               columns: table => new
               {
                   ID = table.Column<int>(type: "int", nullable: false)
                       .Annotation("SqlServer:Identity", "1, 1"),
                   ImplementationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                   SiteLogo = table.Column<string>(type: "image", nullable: true),
                   ImplementationLogo = table.Column<string>(type: "image", nullable: true)
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
