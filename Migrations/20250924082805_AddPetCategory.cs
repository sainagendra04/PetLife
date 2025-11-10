using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetLife.Migrations
{
    /// <inheritdoc />
    public partial class AddPetCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Pets");

            migrationBuilder.AddColumn<Guid>(
                name: "PetCategoryId",
                table: "Pets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "PetCategories",
                columns: table => new
                {
                    PetCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetCategories", x => x.PetCategoryId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pets_PetCategoryId",
                table: "Pets",
                column: "PetCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_PetCategories_PetCategoryId",
                table: "Pets",
                column: "PetCategoryId",
                principalTable: "PetCategories",
                principalColumn: "PetCategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pets_PetCategories_PetCategoryId",
                table: "Pets");

            migrationBuilder.DropTable(
                name: "PetCategories");

            migrationBuilder.DropIndex(
                name: "IX_Pets_PetCategoryId",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "PetCategoryId",
                table: "Pets");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Pets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
