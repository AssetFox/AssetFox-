using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class AddTreatmentLibraryUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentLibrary_User_TreatmentLibrary_TreatmentId",
                table: "TreatmentLibrary_User");

            migrationBuilder.DropColumn(
                name: "CanModify",
                table: "TreatmentLibrary_User");

            migrationBuilder.DropColumn(
                name: "IsOwner",
                table: "TreatmentLibrary_User");

            migrationBuilder.RenameColumn(
                name: "TreatmentId",
                table: "TreatmentLibrary_User",
                newName: "TreatmentLibraryId");

            migrationBuilder.RenameIndex(
                name: "IX_TreatmentLibrary_User_TreatmentId",
                table: "TreatmentLibrary_User",
                newName: "IX_TreatmentLibrary_User_TreatmentLibraryId");

            migrationBuilder.AddColumn<int>(
                name: "AccessLevel",
                table: "TreatmentLibrary_User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TreatmentLibraryEntityId",
                table: "TreatmentLibrary_User",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentLibrary_User_TreatmentLibraryEntityId",
                table: "TreatmentLibrary_User",
                column: "TreatmentLibraryEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentLibrary_User_TreatmentLibrary_TreatmentLibraryEntityId",
                table: "TreatmentLibrary_User",
                column: "TreatmentLibraryEntityId",
                principalTable: "TreatmentLibrary",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentLibrary_User_TreatmentLibrary_TreatmentLibraryId",
                table: "TreatmentLibrary_User",
                column: "TreatmentLibraryId",
                principalTable: "TreatmentLibrary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentLibrary_User_TreatmentLibrary_TreatmentLibraryEntityId",
                table: "TreatmentLibrary_User");

            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentLibrary_User_TreatmentLibrary_TreatmentLibraryId",
                table: "TreatmentLibrary_User");

            migrationBuilder.DropIndex(
                name: "IX_TreatmentLibrary_User_TreatmentLibraryEntityId",
                table: "TreatmentLibrary_User");

            migrationBuilder.DropColumn(
                name: "AccessLevel",
                table: "TreatmentLibrary_User");

            migrationBuilder.DropColumn(
                name: "TreatmentLibraryEntityId",
                table: "TreatmentLibrary_User");

            migrationBuilder.RenameColumn(
                name: "TreatmentLibraryId",
                table: "TreatmentLibrary_User",
                newName: "TreatmentId");

            migrationBuilder.RenameIndex(
                name: "IX_TreatmentLibrary_User_TreatmentLibraryId",
                table: "TreatmentLibrary_User",
                newName: "IX_TreatmentLibrary_User_TreatmentId");

            migrationBuilder.AddColumn<bool>(
                name: "CanModify",
                table: "TreatmentLibrary_User",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOwner",
                table: "TreatmentLibrary_User",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentLibrary_User_TreatmentLibrary_TreatmentId",
                table: "TreatmentLibrary_User",
                column: "TreatmentId",
                principalTable: "TreatmentLibrary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
