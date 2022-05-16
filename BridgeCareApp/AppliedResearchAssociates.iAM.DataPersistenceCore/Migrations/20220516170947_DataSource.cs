using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class DataSource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DataSourceId",
                table: "Attribute",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DataSource",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Secure = table.Column<bool>(type: "bit", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSource", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attribute_DataSourceId",
                table: "Attribute",
                column: "DataSourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attribute_DataSource_DataSourceId",
                table: "Attribute",
                column: "DataSourceId",
                principalTable: "DataSource",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attribute_DataSource_DataSourceId",
                table: "Attribute");

            migrationBuilder.DropTable(
                name: "DataSource");

            migrationBuilder.DropIndex(
                name: "IX_Attribute_DataSourceId",
                table: "Attribute");

            migrationBuilder.DropColumn(
                name: "DataSourceId",
                table: "Attribute");
        }
    }
}
