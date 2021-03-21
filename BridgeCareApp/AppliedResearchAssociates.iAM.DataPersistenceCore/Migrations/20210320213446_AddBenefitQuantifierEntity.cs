using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AddBenefitQuantifierEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BenefitQuantifier",
                columns: table => new
                {
                    NetworkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenefitQuantifier", x => x.NetworkId);
                    table.ForeignKey(
                        name: "FK_BenefitQuantifier_Equation_EquationId",
                        column: x => x.EquationId,
                        principalTable: "Equation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BenefitQuantifier_Network_NetworkId",
                        column: x => x.NetworkId,
                        principalTable: "Network",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BenefitQuantifier_EquationId",
                table: "BenefitQuantifier",
                column: "EquationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BenefitQuantifier_NetworkId",
                table: "BenefitQuantifier",
                column: "NetworkId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BenefitQuantifier");
        }
    }
}
