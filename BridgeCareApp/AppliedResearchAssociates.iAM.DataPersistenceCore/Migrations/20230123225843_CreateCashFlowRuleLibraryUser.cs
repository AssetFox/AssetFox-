using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class CreateCashFlowRuleLibraryUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CashFlowRuleLibrary_User",
                columns: table => new
                {
                    CashFlowRuleLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessLevel = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashFlowRuleLibrary_User", x => new { x.CashFlowRuleLibraryId, x.UserId });
                    table.ForeignKey(
                        name: "FK_CashFlowRuleLibrary_User_CashFlowRuleLibrary_CashFlowRuleLibraryId",
                        column: x => x.CashFlowRuleLibraryId,
                        principalTable: "CashFlowRuleLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CashFlowRuleLibrary_User_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CashFlowRuleLibrary_User_CashFlowRuleLibraryId",
                table: "CashFlowRuleLibrary_User",
                column: "CashFlowRuleLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CashFlowRuleLibrary_User_UserId",
                table: "CashFlowRuleLibrary_User",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CashFlowRuleLibrary_User");
        }
    }
}
