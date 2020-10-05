using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class NoAbstractClasses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AggregationResultEntity_Attributes_AttributeId",
                table: "AggregationResultEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AggregationResultEntity_Segments_SegmentId",
                table: "AggregationResultEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeDatumEntity_Attributes_AttributeId",
                table: "AttributeDatumEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeDatumEntity_Locations_LocationId",
                table: "AttributeDatumEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeDatumEntity_Segments_SegmentId",
                table: "AttributeDatumEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttributeDatumEntity",
                table: "AttributeDatumEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AggregationResultEntity",
                table: "AggregationResultEntity");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "AttributeDatumEntity");

            migrationBuilder.DropColumn(
                name: "TextAttributeDatumEntity_Value",
                table: "AttributeDatumEntity");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "AggregationResultEntity");

            migrationBuilder.DropColumn(
                name: "TextAggregationResultEntity_Value",
                table: "AggregationResultEntity");

            migrationBuilder.RenameTable(
                name: "AttributeDatumEntity",
                newName: "AttributeData");

            migrationBuilder.RenameTable(
                name: "AggregationResultEntity",
                newName: "AggregationResults");

            migrationBuilder.RenameIndex(
                name: "IX_AttributeDatumEntity_SegmentId",
                table: "AttributeData",
                newName: "IX_AttributeData_SegmentId");

            migrationBuilder.RenameIndex(
                name: "IX_AttributeDatumEntity_LocationId",
                table: "AttributeData",
                newName: "IX_AttributeData_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_AttributeDatumEntity_AttributeId",
                table: "AttributeData",
                newName: "IX_AttributeData_AttributeId");

            migrationBuilder.RenameIndex(
                name: "IX_AggregationResultEntity_SegmentId",
                table: "AggregationResults",
                newName: "IX_AggregationResults_SegmentId");

            migrationBuilder.RenameIndex(
                name: "IX_AggregationResultEntity_AttributeId",
                table: "AggregationResults",
                newName: "IX_AggregationResults_AttributeId");

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "Routes",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "Locations",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "AttributeData",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<double>(
                name: "NumericValue",
                table: "AttributeData",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextValue",
                table: "AttributeData",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "AggregationResults",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<double>(
                name: "NumericValue",
                table: "AggregationResults",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextValue",
                table: "AggregationResults",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttributeData",
                table: "AttributeData",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AggregationResults",
                table: "AggregationResults",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AggregationResults_Attributes_AttributeId",
                table: "AggregationResults",
                column: "AttributeId",
                principalTable: "Attributes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AggregationResults_Segments_SegmentId",
                table: "AggregationResults",
                column: "SegmentId",
                principalTable: "Segments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeData_Attributes_AttributeId",
                table: "AttributeData",
                column: "AttributeId",
                principalTable: "Attributes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeData_Locations_LocationId",
                table: "AttributeData",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeData_Segments_SegmentId",
                table: "AttributeData",
                column: "SegmentId",
                principalTable: "Segments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AggregationResults_Attributes_AttributeId",
                table: "AggregationResults");

            migrationBuilder.DropForeignKey(
                name: "FK_AggregationResults_Segments_SegmentId",
                table: "AggregationResults");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeData_Attributes_AttributeId",
                table: "AttributeData");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeData_Locations_LocationId",
                table: "AttributeData");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeData_Segments_SegmentId",
                table: "AttributeData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttributeData",
                table: "AttributeData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AggregationResults",
                table: "AggregationResults");

            migrationBuilder.DropColumn(
                name: "NumericValue",
                table: "AttributeData");

            migrationBuilder.DropColumn(
                name: "TextValue",
                table: "AttributeData");

            migrationBuilder.DropColumn(
                name: "NumericValue",
                table: "AggregationResults");

            migrationBuilder.DropColumn(
                name: "TextValue",
                table: "AggregationResults");

            migrationBuilder.RenameTable(
                name: "AttributeData",
                newName: "AttributeDatumEntity");

            migrationBuilder.RenameTable(
                name: "AggregationResults",
                newName: "AggregationResultEntity");

            migrationBuilder.RenameIndex(
                name: "IX_AttributeData_SegmentId",
                table: "AttributeDatumEntity",
                newName: "IX_AttributeDatumEntity_SegmentId");

            migrationBuilder.RenameIndex(
                name: "IX_AttributeData_LocationId",
                table: "AttributeDatumEntity",
                newName: "IX_AttributeDatumEntity_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_AttributeData_AttributeId",
                table: "AttributeDatumEntity",
                newName: "IX_AttributeDatumEntity_AttributeId");

            migrationBuilder.RenameIndex(
                name: "IX_AggregationResults_SegmentId",
                table: "AggregationResultEntity",
                newName: "IX_AggregationResultEntity_SegmentId");

            migrationBuilder.RenameIndex(
                name: "IX_AggregationResults_AttributeId",
                table: "AggregationResultEntity",
                newName: "IX_AggregationResultEntity_AttributeId");

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "Routes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "AttributeDatumEntity",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Value",
                table: "AttributeDatumEntity",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextAttributeDatumEntity_Value",
                table: "AttributeDatumEntity",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "AggregationResultEntity",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Value",
                table: "AggregationResultEntity",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextAggregationResultEntity_Value",
                table: "AggregationResultEntity",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttributeDatumEntity",
                table: "AttributeDatumEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AggregationResultEntity",
                table: "AggregationResultEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AggregationResultEntity_Attributes_AttributeId",
                table: "AggregationResultEntity",
                column: "AttributeId",
                principalTable: "Attributes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AggregationResultEntity_Segments_SegmentId",
                table: "AggregationResultEntity",
                column: "SegmentId",
                principalTable: "Segments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeDatumEntity_Attributes_AttributeId",
                table: "AttributeDatumEntity",
                column: "AttributeId",
                principalTable: "Attributes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeDatumEntity_Locations_LocationId",
                table: "AttributeDatumEntity",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeDatumEntity_Segments_SegmentId",
                table: "AttributeDatumEntity",
                column: "SegmentId",
                principalTable: "Segments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
