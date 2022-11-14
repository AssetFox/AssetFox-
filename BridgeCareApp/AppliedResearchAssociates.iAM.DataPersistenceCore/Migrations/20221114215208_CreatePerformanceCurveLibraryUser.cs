using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class CreatePerformanceCurveLibraryUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PerformanceCurveLibrary_User",
                columns: table => new
                {
                    PerformanceCurveLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessLevel = table.Column<int>(type: "int", nullable: false),
                    UserEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerformanceCurveLibrary_User", x => new { x.PerformanceCurveLibraryId, x.UserId });
                    table.ForeignKey(
                        name: "FK_PerformanceCurveLibrary_User_PerformanceCurveLibrary_PerformanceCurveLibraryId",
                        column: x => x.PerformanceCurveLibraryId,
                        principalTable: "PerformanceCurveLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerformanceCurveLibrary_User_User_UserEntityId",
                        column: x => x.UserEntityId,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PerformanceCurveLibrary_User_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceCurveLibrary_User_PerformanceCurveLibraryId",
                table: "PerformanceCurveLibrary_User",
                column: "PerformanceCurveLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceCurveLibrary_User_UserEntityId",
                table: "PerformanceCurveLibrary_User",
                column: "UserEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceCurveLibrary_User_UserId",
                table: "PerformanceCurveLibrary_User",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PerformanceCurveLibrary_User");
        }
    }
}
