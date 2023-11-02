using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AddUpdateTreatmentSupersedeRules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CriterionLibrary_TreatmentSupersession");

            migrationBuilder.DropTable(
                name: "TreatmentSupersession");

            migrationBuilder.DropTable(
                name: "CriterionLibrary_ScenarioTreatmentSupersession");

            migrationBuilder.DropTable(
                name: "ScenarioTreatmentSupersession");

            migrationBuilder.CreateTable(
                name: "ScenarioTreatmentSupersedeRule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreventTreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioTreatmentSupersedeRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioTreatmentSupersedeRule_ScenarioSelectableTreatment_TreatmentId",
                        column: x => x.TreatmentId,
                        principalTable: "ScenarioSelectableTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreatmentSupersedeRule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreventTreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentSupersedeRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreatmentSupersedeRule_SelectableTreatment_TreatmentId",
                        column: x => x.TreatmentId,
                        principalTable: "SelectableTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_ScenarioTreatmentSupersedeRule",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioTreatmentSupersedeRuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_ScenarioTreatmentSupersedeRule", x => new { x.CriterionLibraryId, x.ScenarioTreatmentSupersedeRuleId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioTreatmentSupersedeRule_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioTreatmentSupersedeRule_ScenarioTreatmentSupersedeRule_ScenarioTreatmentSupersedeRuleId",
                        column: x => x.ScenarioTreatmentSupersedeRuleId,
                        principalTable: "ScenarioTreatmentSupersedeRule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_TreatmentSupersedeRule",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TreatmentSupersedeRuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_TreatmentSupersedeRule", x => new { x.CriterionLibraryId, x.TreatmentSupersedeRuleId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_TreatmentSupersedeRule_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_TreatmentSupersedeRule_TreatmentSupersedeRule_TreatmentSupersedeRuleId",
                        column: x => x.TreatmentSupersedeRuleId,
                        principalTable: "TreatmentSupersedeRule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioTreatmentSupersedeRule_CriterionLibraryId",
                table: "CriterionLibrary_ScenarioTreatmentSupersedeRule",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioTreatmentSupersedeRule_ScenarioTreatmentSupersedeRuleId",
                table: "CriterionLibrary_ScenarioTreatmentSupersedeRule",
                column: "ScenarioTreatmentSupersedeRuleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_TreatmentSupersedeRule_CriterionLibraryId",
                table: "CriterionLibrary_TreatmentSupersedeRule",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_TreatmentSupersedeRule_TreatmentSupersedeRuleId",
                table: "CriterionLibrary_TreatmentSupersedeRule",
                column: "TreatmentSupersedeRuleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioTreatmentSupersedeRule_TreatmentId",
                table: "ScenarioTreatmentSupersedeRule",
                column: "TreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentSupersedeRule_TreatmentId",
                table: "TreatmentSupersedeRule",
                column: "TreatmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CriterionLibrary_ScenarioTreatmentSupersedeRule");

            migrationBuilder.DropTable(
                name: "CriterionLibrary_TreatmentSupersedeRule");

            migrationBuilder.DropTable(
                name: "ScenarioTreatmentSupersedeRule");

            migrationBuilder.DropTable(
                name: "TreatmentSupersedeRule");

            migrationBuilder.CreateTable(
                name: "ScenarioTreatmentSupersession",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioTreatmentSupersession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioTreatmentSupersession_ScenarioSelectableTreatment_TreatmentId",
                        column: x => x.TreatmentId,
                        principalTable: "ScenarioSelectableTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_ScenarioTreatmentSupersession",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TreatmentSupersessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_ScenarioTreatmentSupersession", x => new { x.CriterionLibraryId, x.TreatmentSupersessionId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioTreatmentSupersession_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioTreatmentSupersession_ScenarioTreatmentSupersession_TreatmentSupersessionId",
                        column: x => x.TreatmentSupersessionId,
                        principalTable: "ScenarioTreatmentSupersession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreatmentSupersession",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriterionLibraryScenarioTreatmentSupersessionJoinCriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriterionLibraryScenarioTreatmentSupersessionJoinTreatmentSupersessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentSupersession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreatmentSupersession_CriterionLibrary_ScenarioTreatmentSupersession_CriterionLibraryScenarioTreatmentSupersessionJoinCriter~",
                        columns: x => new { x.CriterionLibraryScenarioTreatmentSupersessionJoinCriterionLibraryId, x.CriterionLibraryScenarioTreatmentSupersessionJoinTreatmentSupersessionId },
                        principalTable: "CriterionLibrary_ScenarioTreatmentSupersession",
                        principalColumns: new[] { "CriterionLibraryId", "TreatmentSupersessionId" });
                    table.ForeignKey(
                        name: "FK_TreatmentSupersession_SelectableTreatment_TreatmentId",
                        column: x => x.TreatmentId,
                        principalTable: "SelectableTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_TreatmentSupersession",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TreatmentSupersessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_TreatmentSupersession", x => new { x.CriterionLibraryId, x.TreatmentSupersessionId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_TreatmentSupersession_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_TreatmentSupersession_TreatmentSupersession_TreatmentSupersessionId",
                        column: x => x.TreatmentSupersessionId,
                        principalTable: "TreatmentSupersession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioTreatmentSupersession_CriterionLibraryId",
                table: "CriterionLibrary_ScenarioTreatmentSupersession",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioTreatmentSupersession_TreatmentSupersessionId",
                table: "CriterionLibrary_ScenarioTreatmentSupersession",
                column: "TreatmentSupersessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_TreatmentSupersession_CriterionLibraryId",
                table: "CriterionLibrary_TreatmentSupersession",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_TreatmentSupersession_TreatmentSupersessionId",
                table: "CriterionLibrary_TreatmentSupersession",
                column: "TreatmentSupersessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioTreatmentSupersession_TreatmentId",
                table: "ScenarioTreatmentSupersession",
                column: "TreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentSupersession_CriterionLibraryScenarioTreatmentSupersessionJoinCriterionLibraryId_CriterionLibraryScenarioTreatmentS~",
                table: "TreatmentSupersession",
                columns: new[] { "CriterionLibraryScenarioTreatmentSupersessionJoinCriterionLibraryId", "CriterionLibraryScenarioTreatmentSupersessionJoinTreatmentSupersessionId" });

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentSupersession_TreatmentId",
                table: "TreatmentSupersession",
                column: "TreatmentId");
        }
    }
}
