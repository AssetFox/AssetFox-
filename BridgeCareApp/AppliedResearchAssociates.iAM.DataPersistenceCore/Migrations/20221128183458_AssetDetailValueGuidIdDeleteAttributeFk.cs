using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AssetDetailValueGuidIdDeleteAttributeFk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetDetailValue_Attribute_AttributeId",
                table: "AssetDetailValue");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetSummaryDetailValue_Attribute_AttributeId",
                table: "AssetSummaryDetailValue");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_AssetDetailValue_Attribute_AttributeId",
                table: "AssetDetailValue",
                column: "AttributeId",
                principalTable: "Attribute",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetSummaryDetailValue_Attribute_AttributeId",
                table: "AssetSummaryDetailValue",
                column: "AttributeId",
                principalTable: "Attribute",
                principalColumn: "Id");
        }
    }
}
