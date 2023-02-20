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
                name: "PerformanceCurveLibrary_User",
                columns: table => new
                {
                    PerformanceCurveLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessLevel = table.Column<int>(type: "int", nullable: false),
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
                        name: "FK_PerformanceCurveLibrary_User_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TargetConditionGoalLibrary_User",
                columns: table => new
                {
                    LibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetConditionGoalLibrary_User", x => new { x.LibraryId, x.UserId });
                    table.ForeignKey(
                        name: "FK_TargetConditionGoalLibrary_User_TargetConditionGoalLibrary_LibraryId",
                        column: x => x.LibraryId,
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

            migrationBuilder.CreateTable(
                name: "TreatmentLibrary_User",
                columns: table => new
                {
                    TreatmentLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessLevel = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentLibrary_User", x => new { x.TreatmentLibraryId, x.UserId });
                    table.ForeignKey(
                        name: "FK_TreatmentLibrary_User_TreatmentLibrary_TreatmentLibraryId",
                        column: x => x.TreatmentLibraryId,
                        principalTable: "TreatmentLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TreatmentLibrary_User_User_UserId",
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
                name: "IX_PerformanceCurveLibrary_User_UserId",
                table: "PerformanceCurveLibrary_User",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetConditionGoalLibrary_User_LibraryId",
                table: "TargetConditionGoalLibrary_User",
                column: "LibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetConditionGoalLibrary_User_UserId",
                table: "TargetConditionGoalLibrary_User",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentLibrary_User_TreatmentLibraryId",
                table: "TreatmentLibrary_User",
                column: "TreatmentLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentLibrary_User_UserId",
                table: "TreatmentLibrary_User",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PerformanceCurveLibrary_User");

            migrationBuilder.DropTable(
                name: "TargetConditionGoalLibrary_User");

            migrationBuilder.DropTable(
                name: "TreatmentLibrary_User");
        }
    }
}
