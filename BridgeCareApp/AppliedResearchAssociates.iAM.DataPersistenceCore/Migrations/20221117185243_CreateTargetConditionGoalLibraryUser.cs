using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class CreateTargetConditionGoalLibraryUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TargetConditionGoalLibrary_User",
                columns: table => new
                {
                    TargetConditionGoalLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessLevel = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetConditionGoalLibrary_User", x => new { x.TargetConditionGoalLibraryId, x.UserId });
                    table.ForeignKey(
                        name: "FK_TargetConditionGoalLibrary_User_TargetConditionGoalLibrary_TargetConditionGoalLibraryId",
                        column: x => x.TargetConditionGoalLibraryId,
                        principalTable: "TargetConditionGoalLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TargetConditionGoalLibrary_User_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TargetConditionGoalLibrary_User_TargetConditionGoalLibraryId",
                table: "TargetConditionGoalLibrary_User",
                column: "TargetConditionGoalLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetConditionGoalLibrary_User_UserId",
                table: "TargetConditionGoalLibrary_User",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TargetConditionGoalLibrary_User");
        }
    }
}
