using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolNewspaperBlazorApp.Migrations
{
    /// <inheritdoc />
    public partial class Substitutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Stara tabela była niepodłączona do żadnego formularza (zawsze pusta).
            migrationBuilder.DropTable(
                name: "ArticleSubstitutes");

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubstituteLessons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OriginalTeacherId = table.Column<int>(type: "int", nullable: true),
                    SubstituteTeacherId = table.Column<int>(type: "int", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LessonNumber = table.Column<int>(type: "int", nullable: true),
                    OriginalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubstituteDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OldRoom = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Room = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubstituteLessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubstituteLessons_Teachers_OriginalTeacherId",
                        column: x => x.OriginalTeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubstituteLessons_Teachers_SubstituteTeacherId",
                        column: x => x.SubstituteTeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubstituteLessons_OriginalTeacherId",
                table: "SubstituteLessons",
                column: "OriginalTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_SubstituteLessons_SubstituteDate",
                table: "SubstituteLessons",
                column: "SubstituteDate");

            migrationBuilder.CreateIndex(
                name: "IX_SubstituteLessons_SubstituteTeacherId",
                table: "SubstituteLessons",
                column: "SubstituteTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_Surname_Name",
                table: "Teachers",
                columns: new[] { "Surname", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubstituteLessons");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.CreateTable(
                name: "ArticleSubstitutes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    ClassName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldRoom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrginalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OriginalTeacher = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Room = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubstituteDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubstituteTeacher = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleSubstitutes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleSubstitutes_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleSubstitutes_ArticleId",
                table: "ArticleSubstitutes",
                column: "ArticleId",
                unique: true);
        }
    }
}
