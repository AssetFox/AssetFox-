using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class CommittedProjectDTO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommittedProject_MaintainableAsset_MaintainableAssetId",
                table: "CommittedProject");

            migrationBuilder.DropIndex(
                name: "IX_CommittedProject_MaintainableAssetId",
                table: "CommittedProject");

            migrationBuilder.DropColumn(
                name: "MaintainableAssetId",
                table: "CommittedProject");

            migrationBuilder.AlterColumn<Guid>(
                name: "ScenarioBudgetId",
                table: "CommittedProject",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "MaintainableAssetEntityId",
                table: "CommittedProject",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CommittedProjectLocation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommittedProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocationIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Start = table.Column<double>(type: "float", nullable: true),
                    End = table.Column<double>(type: "float", nullable: true),
                    Direction = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommittedProjectLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommittedProjectLocation_CommittedProject_CommittedProjectId",
                        column: x => x.CommittedProjectId,
                        principalTable: "CommittedProject",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommittedProject_MaintainableAssetEntityId",
                table: "CommittedProject",
                column: "MaintainableAssetEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommittedProjectLocation_CommittedProjectId",
                table: "CommittedProjectLocation",
                column: "CommittedProjectId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CommittedProject_MaintainableAsset_MaintainableAssetEntityId",
                table: "CommittedProject",
                column: "MaintainableAssetEntityId",
                principalTable: "MaintainableAsset",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommittedProject_MaintainableAsset_MaintainableAssetEntityId",
                table: "CommittedProject");

            migrationBuilder.DropTable(
                name: "CommittedProjectLocation");

            migrationBuilder.DropIndex(
                name: "IX_CommittedProject_MaintainableAssetEntityId",
                table: "CommittedProject");

            migrationBuilder.DropColumn(
                name: "MaintainableAssetEntityId",
                table: "CommittedProject");

            migrationBuilder.AlterColumn<Guid>(
                name: "ScenarioBudgetId",
                table: "CommittedProject",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MaintainableAssetId",
                table: "CommittedProject",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CommittedProject_MaintainableAssetId",
                table: "CommittedProject",
                column: "MaintainableAssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommittedProject_MaintainableAsset_MaintainableAssetId",
                table: "CommittedProject",
                column: "MaintainableAssetId",
                principalTable: "MaintainableAsset",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
