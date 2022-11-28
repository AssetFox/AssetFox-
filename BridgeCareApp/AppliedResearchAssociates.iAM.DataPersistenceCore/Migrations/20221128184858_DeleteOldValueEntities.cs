using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class DeleteOldValueEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetDetailValue");

            migrationBuilder.DropTable(
                name: "AssetSummaryDetailValue");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetDetailValue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Discriminator = table.Column<string>(type: "char(1)", nullable: false),
                    NumericValue = table.Column<double>(type: "float", nullable: true),
                    TextValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetDetailValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetDetailValue_AssetDetail_AssetDetailId",
                        column: x => x.AssetDetailId,
                        principalTable: "AssetDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetSummaryDetailValue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetSummaryDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Discriminator = table.Column<string>(type: "char(1)", nullable: false),
                    NumericValue = table.Column<double>(type: "float", nullable: true),
                    TextValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetSummaryDetailValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetSummaryDetailValue_AssetSummaryDetail_AssetSummaryDetailId",
                        column: x => x.AssetSummaryDetailId,
                        principalTable: "AssetSummaryDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetDetailValue_AssetDetailId",
                table: "AssetDetailValue",
                column: "AssetDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDetailValue_AttributeId",
                table: "AssetDetailValue",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDetailValue_Id",
                table: "AssetDetailValue",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetSummaryDetailValue_AssetSummaryDetailId",
                table: "AssetSummaryDetailValue",
                column: "AssetSummaryDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetSummaryDetailValue_AttributeId",
                table: "AssetSummaryDetailValue",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetSummaryDetailValue_Id",
                table: "AssetSummaryDetailValue",
                column: "Id",
                unique: true);
        }
    }
}
