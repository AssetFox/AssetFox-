using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class CreateCalculatedAttributeLibraryUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalculatedAttributeLibrary_User",
                columns: table => new
                {
                    CalculatedAttributeLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessLevel = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalculatedAttributeLibrary_User", x => new { x.CalculatedAttributeLibraryId, x.UserId });
                    table.ForeignKey(
                        name: "FK_CalculatedAttributeLibrary_User_CalculatedAttributeLibrary_CalculatedAttributeLibraryId",
                        column: x => x.CalculatedAttributeLibraryId,
                        principalTable: "CalculatedAttributeLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CalculatedAttributeLibrary_User_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalculatedAttributeLibrary_User_CalculatedAttributeLibraryId",
                table: "CalculatedAttributeLibrary_User",
                column: "CalculatedAttributeLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CalculatedAttributeLibrary_User_UserId",
                table: "CalculatedAttributeLibrary_User",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalculatedAttributeLibrary_User");
        }
    }
}
