using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetLife.Migrations
{
    /// <inheritdoc />
    public partial class AddedPetCategoryInPetFood : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PetType",
                table: "PetFoods");

            migrationBuilder.AddColumn<Guid>(
                name: "PetCategoryId",
                table: "PetFoods",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PetFoods_PetCategoryId",
                table: "PetFoods",
                column: "PetCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_PetFoods_PetCategories_PetCategoryId",
                table: "PetFoods",
                column: "PetCategoryId",
                principalTable: "PetCategories",
                principalColumn: "PetCategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PetFoods_PetCategories_PetCategoryId",
                table: "PetFoods");

            migrationBuilder.DropIndex(
                name: "IX_PetFoods_PetCategoryId",
                table: "PetFoods");

            migrationBuilder.DropColumn(
                name: "PetCategoryId",
                table: "PetFoods");

            migrationBuilder.AddColumn<string>(
                name: "PetType",
                table: "PetFoods",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
