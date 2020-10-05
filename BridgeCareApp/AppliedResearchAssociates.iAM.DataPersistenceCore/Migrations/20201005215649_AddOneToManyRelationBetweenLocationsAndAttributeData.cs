using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AddOneToManyRelationBetweenLocationsAndAttributeData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttributeData_Locations_LocationId",
                table: "AttributeData");

            migrationBuilder.DropIndex(
                name: "IX_AttributeData_LocationId",
                table: "AttributeData");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeData_LocationId",
                table: "AttributeData",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeData_Locations_LocationId",
                table: "AttributeData",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttributeData_Locations_LocationId",
                table: "AttributeData");

            migrationBuilder.DropIndex(
                name: "IX_AttributeData_LocationId",
                table: "AttributeData");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeData_LocationId",
                table: "AttributeData",
                column: "LocationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeData_Locations_LocationId",
                table: "AttributeData",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
