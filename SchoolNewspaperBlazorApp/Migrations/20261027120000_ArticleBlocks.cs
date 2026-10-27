using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SchoolNewspaperBlazorApp.Data;

#nullable disable

namespace SchoolNewspaperBlazorApp.Migrations
{
    /// <summary>
    /// Treść artykułu jako uporządkowane bloki (tekst / zdjęcie), żeby można było pisać w dowolnym miejscu.
    /// </summary>
    [DbContext(typeof(NewspaperDbContext))]
    [Migration("20261027120000_ArticleBlocks")]
    public partial class ArticleBlocks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArticleBlocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    BlockType = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MediaFileId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleBlocks_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleBlocks_ArticleId_Position",
                table: "ArticleBlocks",
                columns: new[] { "ArticleId", "Position" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ArticleBlocks");
        }
    }
}
