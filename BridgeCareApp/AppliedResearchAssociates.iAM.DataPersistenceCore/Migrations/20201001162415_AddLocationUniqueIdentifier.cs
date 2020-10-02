using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AddLocationUniqueIdentifier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Routes");

            migrationBuilder.AddColumn<string>(
                name: "UniqueIdentifier",
                table: "Segments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UniqueIdentifier",
                table: "Routes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UniqueIdentifier",
                table: "Locations",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AggregationResultEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SegmentId = table.Column<Guid>(nullable: false),
                    AttributeId = table.Column<Guid>(nullable: false),
                    Year = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    Value = table.Column<double>(nullable: true),
                    TextAggregationResultEntity_Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregationResultEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AggregationResultEntity_Attributes_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AggregationResultEntity_Segments_SegmentId",
                        column: x => x.SegmentId,
                        principalTable: "Segments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AggregationResultEntity_AttributeId",
                table: "AggregationResultEntity",
                column: "AttributeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AggregationResultEntity_SegmentId",
                table: "AggregationResultEntity",
                column: "SegmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AggregationResultEntity");

            migrationBuilder.DropColumn(
                name: "UniqueIdentifier",
                table: "Segments");

            migrationBuilder.DropColumn(
                name: "UniqueIdentifier",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "UniqueIdentifier",
                table: "Locations");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Routes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
