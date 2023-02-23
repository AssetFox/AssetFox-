using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class CreateRemainingLifeLimitLibraryUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RemainingLifeLimitLibrary_User",
                columns: table => new
                {
                    RemainingLifeLimitLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessLevel = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemainingLifeLimitLibrary_User", x => new { x.RemainingLifeLimitLibraryId, x.UserId });
                    table.ForeignKey(
                        name: "FK_RemainingLifeLimitLibrary_User_RemainingLifeLimitLibrary_RemainingLifeLimitLibraryId",
                        column: x => x.RemainingLifeLimitLibraryId,
                        principalTable: "RemainingLifeLimitLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RemainingLifeLimitLibrary_User_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RemainingLifeLimitLibrary_User_RemainingLifeLimitLibraryId",
                table: "RemainingLifeLimitLibrary_User",
                column: "RemainingLifeLimitLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_RemainingLifeLimitLibrary_User_UserId",
                table: "RemainingLifeLimitLibrary_User",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RemainingLifeLimitLibrary_User");
        }
    }
}
