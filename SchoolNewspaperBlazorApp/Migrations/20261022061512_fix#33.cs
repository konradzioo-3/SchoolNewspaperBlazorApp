using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolNewspaperBlazorApp.Migrations
{
    /// <inheritdoc />
    public partial class fix33 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileId",
                table: "Articles",
                newName: "MainImageId");

            migrationBuilder.AddColumn<int>(
                name: "ArticleId",
                table: "Files",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsMainImage",
                table: "Files",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArticleId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "IsMainImage",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "MainImageId",
                table: "Articles",
                newName: "FileId");
        }
    }
}
