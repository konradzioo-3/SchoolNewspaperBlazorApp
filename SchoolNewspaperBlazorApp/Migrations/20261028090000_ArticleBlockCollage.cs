using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SchoolNewspaperBlazorApp.Data;

#nullable disable

namespace SchoolNewspaperBlazorApp.Migrations
{
    /// <summary>
    /// Zdjęcia w treści artykułu mogą się scalać w kolaż. Flaga InCollage mówi, czy dane zdjęcie
    /// ma się łączyć z sąsiednimi (domyślnie tak, także dla już zapisanych artykułów).
    /// </summary>
    [DbContext(typeof(NewspaperDbContext))]
    [Migration("20261028090000_ArticleBlockCollage")]
    public partial class ArticleBlockCollage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "InCollage",
                table: "ArticleBlocks",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InCollage",
                table: "ArticleBlocks");
        }
    }
}
