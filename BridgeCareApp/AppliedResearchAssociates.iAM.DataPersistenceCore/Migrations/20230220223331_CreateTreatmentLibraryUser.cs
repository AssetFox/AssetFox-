﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class CreateTreatmentLibraryUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TreatmentLibrary_User",
                columns: table => new
                {
                    LibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentLibrary_User", x => new { x.LibraryId, x.UserId });
                    table.ForeignKey(
                        name: "FK_TreatmentLibrary_User_TreatmentLibrary_LibraryId",
                        column: x => x.LibraryId,
                        principalTable: "TreatmentLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TreatmentLibrary_User_User_UserEntityId",
                        column: x => x.UserEntityId,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TreatmentLibrary_User_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentLibrary_User_LibraryId",
                table: "TreatmentLibrary_User",
                column: "LibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentLibrary_User_UserEntityId",
                table: "TreatmentLibrary_User",
                column: "UserEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentLibrary_User_UserId",
                table: "TreatmentLibrary_User",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
               name: "TreatmentLibrary_User");
        }
    }
}
