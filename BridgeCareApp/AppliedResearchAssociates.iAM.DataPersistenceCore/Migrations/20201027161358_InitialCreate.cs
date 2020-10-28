using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Attributes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    DataType = table.Column<string>(nullable: false),
                    AggregationRuleType = table.Column<string>(nullable: false),
                    Command = table.Column<string>(nullable: false),
                    ConnectionType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attributes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Networks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Networks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaintainableAssets",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UniqueIdentifier = table.Column<string>(nullable: false),
                    NetworkId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintainableAssets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintainableAssets_Networks_NetworkId",
                        column: x => x.NetworkId,
                        principalTable: "Networks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AggregatedResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    Year = table.Column<int>(nullable: false),
                    TextValue = table.Column<string>(nullable: true),
                    NumericValue = table.Column<double>(nullable: true),
                    MaintainableAssetId = table.Column<Guid>(nullable: false),
                    AttributeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregatedResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AggregatedResults_Attributes_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AggregatedResults_MaintainableAssets_MaintainableAssetId",
                        column: x => x.MaintainableAssetId,
                        principalTable: "MaintainableAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttributeData",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    NumericValue = table.Column<double>(nullable: true),
                    TextValue = table.Column<string>(nullable: true),
                    AttributeId = table.Column<Guid>(nullable: false),
                    MaintainableAssetId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeData_Attributes_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttributeData_MaintainableAssets_MaintainableAssetId",
                        column: x => x.MaintainableAssetId,
                        principalTable: "MaintainableAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintainableAssetLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    UniqueIdentifier = table.Column<string>(nullable: false),
                    Start = table.Column<double>(nullable: true),
                    End = table.Column<double>(nullable: true),
                    Direction = table.Column<int>(nullable: true),
                    MaintainableAssetId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintainableAssetLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintainableAssetLocations_MaintainableAssets_MaintainableAssetId",
                        column: x => x.MaintainableAssetId,
                        principalTable: "MaintainableAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttributeDatumLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    UniqueIdentifier = table.Column<string>(nullable: false),
                    Start = table.Column<double>(nullable: true),
                    End = table.Column<double>(nullable: true),
                    Direction = table.Column<int>(nullable: true),
                    AttributeDatumId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeDatumLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeDatumLocations_AttributeData_AttributeDatumId",
                        column: x => x.AttributeDatumId,
                        principalTable: "AttributeData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedResults_AttributeId",
                table: "AggregatedResults",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedResults_MaintainableAssetId",
                table: "AggregatedResults",
                column: "MaintainableAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeData_AttributeId",
                table: "AttributeData",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeData_MaintainableAssetId",
                table: "AttributeData",
                column: "MaintainableAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDatumLocations_AttributeDatumId",
                table: "AttributeDatumLocations",
                column: "AttributeDatumId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintainableAssetLocations_MaintainableAssetId",
                table: "MaintainableAssetLocations",
                column: "MaintainableAssetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintainableAssets_NetworkId",
                table: "MaintainableAssets",
                column: "NetworkId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AggregatedResults");

            migrationBuilder.DropTable(
                name: "AttributeDatumLocations");

            migrationBuilder.DropTable(
                name: "MaintainableAssetLocations");

            migrationBuilder.DropTable(
                name: "AttributeData");

            migrationBuilder.DropTable(
                name: "Attributes");

            migrationBuilder.DropTable(
                name: "MaintainableAssets");

            migrationBuilder.DropTable(
                name: "Networks");
        }
    }
}
