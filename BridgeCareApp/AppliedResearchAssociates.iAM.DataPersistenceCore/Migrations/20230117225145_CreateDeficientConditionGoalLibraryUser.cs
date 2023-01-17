using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class CreateDeficientConditionGoalLibraryUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeficientConditionGoalLibrary_User",
                columns: table => new
                {
                    DeficientConditionGoalLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessLevel = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeficientConditionGoalLibrary_User", x => new { x.DeficientConditionGoalLibraryId, x.UserId });
                    table.ForeignKey(
                        name: "FK_DeficientConditionGoalLibrary_User_DeficientConditionGoalLibrary_DeficientConditionGoalLibraryId",
                        column: x => x.DeficientConditionGoalLibraryId,
                        principalTable: "DeficientConditionGoalLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeficientConditionGoalLibrary_User_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeficientConditionGoalLibrary_User_DeficientConditionGoalLibraryId",
                table: "DeficientConditionGoalLibrary_User",
                column: "DeficientConditionGoalLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_DeficientConditionGoalLibrary_User_UserId",
                table: "DeficientConditionGoalLibrary_User",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeficientConditionGoalLibrary_User");
        }
    }
}
