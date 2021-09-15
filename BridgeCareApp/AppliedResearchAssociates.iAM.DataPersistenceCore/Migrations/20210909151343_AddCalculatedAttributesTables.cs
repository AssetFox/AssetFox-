using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AddCalculatedAttributesTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalculatedAttributeLibrary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalculatedAttributeLibrary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioCalculatedAttribute",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CalculationTiming = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioCalculatedAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioCalculatedAttribute_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScenarioCalculatedAttribute_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CalculatedAttribute",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CalculatedAttributeLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CalculationTiming = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalculatedAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalculatedAttribute_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CalculatedAttribute_CalculatedAttributeLibrary_CalculatedAttributeLibraryId",
                        column: x => x.CalculatedAttributeLibraryId,
                        principalTable: "CalculatedAttributeLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioCalculatedAttributePair",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioCalculatedAttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioCalculatedAttributePair", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioCalculatedAttributePair_ScenarioCalculatedAttribute_ScenarioCalculatedAttributeId",
                        column: x => x.ScenarioCalculatedAttributeId,
                        principalTable: "ScenarioCalculatedAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CalculatedAttributePair",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CalculatedAttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalculatedAttributePair", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalculatedAttributePair_CalculatedAttribute_CalculatedAttributeId",
                        column: x => x.CalculatedAttributeId,
                        principalTable: "CalculatedAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioCalculatedAttributePair_Criteria",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioCalculatedAttributePairId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioCalculatedAttributePair_Criteria", x => new { x.ScenarioCalculatedAttributePairId, x.CriterionLibraryId });
                    table.ForeignKey(
                        name: "FK_ScenarioCalculatedAttributePair_Criteria_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScenarioCalculatedAttributePair_Criteria_ScenarioCalculatedAttributePair_ScenarioCalculatedAttributePairId",
                        column: x => x.ScenarioCalculatedAttributePairId,
                        principalTable: "ScenarioCalculatedAttributePair",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioCalculatedAttributePair_Equation",
                columns: table => new
                {
                    EquationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioCalculatedAttributePairId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioCalculatedAttributePair_Equation", x => new { x.ScenarioCalculatedAttributePairId, x.EquationId });
                    table.ForeignKey(
                        name: "FK_ScenarioCalculatedAttributePair_Equation_Equation_EquationId",
                        column: x => x.EquationId,
                        principalTable: "Equation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScenarioCalculatedAttributePair_Equation_ScenarioCalculatedAttributePair_ScenarioCalculatedAttributePairId",
                        column: x => x.ScenarioCalculatedAttributePairId,
                        principalTable: "ScenarioCalculatedAttributePair",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CalculatedAttributePair_Criteria",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CalculatedAttributePairId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalculatedAttributePair_Criteria", x => new { x.CalculatedAttributePairId, x.CriterionLibraryId });
                    table.ForeignKey(
                        name: "FK_CalculatedAttributePair_Criteria_CalculatedAttributePair_CalculatedAttributePairId",
                        column: x => x.CalculatedAttributePairId,
                        principalTable: "CalculatedAttributePair",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CalculatedAttributePair_Criteria_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CalculatedAttributePair_Equation",
                columns: table => new
                {
                    EquationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CalculatedAttributePairId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalculatedAttributePair_Equation", x => new { x.CalculatedAttributePairId, x.EquationId });
                    table.ForeignKey(
                        name: "FK_CalculatedAttributePair_Equation_CalculatedAttributePair_CalculatedAttributePairId",
                        column: x => x.CalculatedAttributePairId,
                        principalTable: "CalculatedAttributePair",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CalculatedAttributePair_Equation_Equation_EquationId",
                        column: x => x.EquationId,
                        principalTable: "Equation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalculatedAttribute_AttributeId",
                table: "CalculatedAttribute",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_CalculatedAttribute_CalculatedAttributeLibraryId",
                table: "CalculatedAttribute",
                column: "CalculatedAttributeLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CalculatedAttributePair_CalculatedAttributeId",
                table: "CalculatedAttributePair",
                column: "CalculatedAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_CalculatedAttributePair_Criteria_CalculatedAttributePairId",
                table: "CalculatedAttributePair_Criteria",
                column: "CalculatedAttributePairId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CalculatedAttributePair_Criteria_CriterionLibraryId",
                table: "CalculatedAttributePair_Criteria",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CalculatedAttributePair_Equation_CalculatedAttributePairId",
                table: "CalculatedAttributePair_Equation",
                column: "CalculatedAttributePairId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CalculatedAttributePair_Equation_EquationId",
                table: "CalculatedAttributePair_Equation",
                column: "EquationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCalculatedAttribute_AttributeId",
                table: "ScenarioCalculatedAttribute",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCalculatedAttribute_SimulationId",
                table: "ScenarioCalculatedAttribute",
                column: "SimulationId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCalculatedAttributePair_ScenarioCalculatedAttributeId",
                table: "ScenarioCalculatedAttributePair",
                column: "ScenarioCalculatedAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCalculatedAttributePair_Criteria_CriterionLibraryId",
                table: "ScenarioCalculatedAttributePair_Criteria",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCalculatedAttributePair_Criteria_ScenarioCalculatedAttributePairId",
                table: "ScenarioCalculatedAttributePair_Criteria",
                column: "ScenarioCalculatedAttributePairId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCalculatedAttributePair_Equation_EquationId",
                table: "ScenarioCalculatedAttributePair_Equation",
                column: "EquationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCalculatedAttributePair_Equation_ScenarioCalculatedAttributePairId",
                table: "ScenarioCalculatedAttributePair_Equation",
                column: "ScenarioCalculatedAttributePairId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalculatedAttributePair_Criteria");

            migrationBuilder.DropTable(
                name: "CalculatedAttributePair_Equation");

            migrationBuilder.DropTable(
                name: "ScenarioCalculatedAttributePair_Criteria");

            migrationBuilder.DropTable(
                name: "ScenarioCalculatedAttributePair_Equation");

            migrationBuilder.DropTable(
                name: "CalculatedAttributePair");

            migrationBuilder.DropTable(
                name: "ScenarioCalculatedAttributePair");

            migrationBuilder.DropTable(
                name: "CalculatedAttribute");

            migrationBuilder.DropTable(
                name: "ScenarioCalculatedAttribute");

            migrationBuilder.DropTable(
                name: "CalculatedAttributeLibrary");
        }
    }
}
