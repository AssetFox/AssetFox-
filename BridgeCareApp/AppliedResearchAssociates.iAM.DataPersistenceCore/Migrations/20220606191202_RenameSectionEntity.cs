using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class RenameSectionEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommittedProject_Section_SectionEntityId",
                table: "CommittedProject");

            migrationBuilder.DropForeignKey(
                name: "FK_NumericAttributeValueHistory_Section_SectionId",
                table: "NumericAttributeValueHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_TextAttributeValueHistory_Section_SectionId",
                table: "TextAttributeValueHistory");

            migrationBuilder.DropTable(
                name: "Section");

            migrationBuilder.RenameColumn(
                name: "SectionEntityId",
                table: "CommittedProject",
                newName: "AnalysisMaintainableAssetEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_CommittedProject_SectionEntityId",
                table: "CommittedProject",
                newName: "IX_CommittedProject_AnalysisMaintainableAssetEntityId");

            migrationBuilder.CreateTable(
                name: "AnalysisMaintainableAsset",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpatialWeightingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NetworkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisMaintainableAsset", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalysisMaintainableAsset_Equation_SpatialWeightingId",
                        column: x => x.SpatialWeightingId,
                        principalTable: "Equation",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AnalysisMaintainableAsset_Network_NetworkId",
                        column: x => x.NetworkId,
                        principalTable: "Network",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisMaintainableAsset_NetworkId",
                table: "AnalysisMaintainableAsset",
                column: "NetworkId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisMaintainableAsset_SpatialWeightingId",
                table: "AnalysisMaintainableAsset",
                column: "SpatialWeightingId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommittedProject_AnalysisMaintainableAsset_AnalysisMaintainableAssetEntityId",
                table: "CommittedProject",
                column: "AnalysisMaintainableAssetEntityId",
                principalTable: "AnalysisMaintainableAsset",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NumericAttributeValueHistory_AnalysisMaintainableAsset_SectionId",
                table: "NumericAttributeValueHistory",
                column: "SectionId",
                principalTable: "AnalysisMaintainableAsset",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TextAttributeValueHistory_AnalysisMaintainableAsset_SectionId",
                table: "TextAttributeValueHistory",
                column: "SectionId",
                principalTable: "AnalysisMaintainableAsset",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommittedProject_AnalysisMaintainableAsset_AnalysisMaintainableAssetEntityId",
                table: "CommittedProject");

            migrationBuilder.DropForeignKey(
                name: "FK_NumericAttributeValueHistory_AnalysisMaintainableAsset_SectionId",
                table: "NumericAttributeValueHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_TextAttributeValueHistory_AnalysisMaintainableAsset_SectionId",
                table: "TextAttributeValueHistory");

            migrationBuilder.DropTable(
                name: "AnalysisMaintainableAsset");

            migrationBuilder.RenameColumn(
                name: "AnalysisMaintainableAssetEntityId",
                table: "CommittedProject",
                newName: "SectionEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_CommittedProject_AnalysisMaintainableAssetEntityId",
                table: "CommittedProject",
                newName: "IX_CommittedProject_SectionEntityId");

            migrationBuilder.CreateTable(
                name: "Section",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NetworkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SpatialWeightingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Section", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Section_Equation_SpatialWeightingId",
                        column: x => x.SpatialWeightingId,
                        principalTable: "Equation",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Section_Network_NetworkId",
                        column: x => x.NetworkId,
                        principalTable: "Network",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Section_NetworkId",
                table: "Section",
                column: "NetworkId");

            migrationBuilder.CreateIndex(
                name: "IX_Section_SpatialWeightingId",
                table: "Section",
                column: "SpatialWeightingId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommittedProject_Section_SectionEntityId",
                table: "CommittedProject",
                column: "SectionEntityId",
                principalTable: "Section",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NumericAttributeValueHistory_Section_SectionId",
                table: "NumericAttributeValueHistory",
                column: "SectionId",
                principalTable: "Section",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TextAttributeValueHistory_Section_SectionId",
                table: "TextAttributeValueHistory",
                column: "SectionId",
                principalTable: "Section",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
