using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AssetDetailValueIntId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetDetailValueIntId",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Discriminator = table.Column<string>(type: "char(1)", nullable: false),
                    TextValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumericValue = table.Column<double>(type: "float", nullable: true),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetDetailValueIntId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetDetailValueIntId_AssetDetail_AssetDetailId",
                        column: x => x.AssetDetailId,
                        principalTable: "AssetDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetDetailValueIntId_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AssetSummaryDetailValueIntId",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetSummaryDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Discriminator = table.Column<string>(type: "char(1)", nullable: false),
                    TextValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumericValue = table.Column<double>(type: "float", nullable: true),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetSummaryDetailValueIntId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetSummaryDetailValueIntId_AssetSummaryDetail_AssetSummaryDetailId",
                        column: x => x.AssetSummaryDetailId,
                        principalTable: "AssetSummaryDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetSummaryDetailValueIntId_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetDetailValueIntId_AssetDetailId",
                table: "AssetDetailValueIntId",
                column: "AssetDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDetailValueIntId_AttributeId",
                table: "AssetDetailValueIntId",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDetailValueIntId_Id",
                table: "AssetDetailValueIntId",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetSummaryDetailValueIntId_AssetSummaryDetailId",
                table: "AssetSummaryDetailValueIntId",
                column: "AssetSummaryDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetSummaryDetailValueIntId_AttributeId",
                table: "AssetSummaryDetailValueIntId",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetSummaryDetailValueIntId_Id",
                table: "AssetSummaryDetailValueIntId",
                column: "Id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetDetailValueIntId");

            migrationBuilder.DropTable(
                name: "AssetSummaryDetailValueIntId");
        }
    }
}
