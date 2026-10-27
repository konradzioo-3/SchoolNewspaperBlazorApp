using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolNewspaperBlazorApp.Migrations
{
    /// <inheritdoc />
    public partial class RoleModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthorRole",
                table: "Articles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            // ---- Zachowanie istniejących danych (zanim zniknie stara kolumna Users.Role) ----

            // 1) dotychczasowe stanowiska użytkowników -> wiersze w tabeli Roles
            migrationBuilder.Sql(
                "EXEC(N'INSERT INTO [Roles] ([Name]) " +
                "SELECT DISTINCT LEFT(LTRIM(RTRIM([Role])), 100) FROM [Users] " +
                "WHERE [Role] IS NOT NULL AND LEN(LTRIM(RTRIM([Role]))) > 0')");

            // 2) przypisanie użytkowników do utworzonych ról
            migrationBuilder.Sql(
                "EXEC(N'UPDATE u SET u.[RoleId] = r.[Id] FROM [Users] u " +
                "INNER JOIN [Roles] r ON r.[Name] = LEFT(LTRIM(RTRIM(u.[Role])), 100)')");

            // 3) stare artykuły miały w Author tekst "Imię Nazwisko<enter>Rola" -> rozdziel na Author i AuthorRole
            migrationBuilder.Sql(
                "EXEC(N'UPDATE [Articles] SET " +
                "[AuthorRole] = LEFT(LTRIM(RTRIM(SUBSTRING([Author], CHARINDEX(CHAR(10), [Author]) + 1, LEN([Author])))), 100), " +
                "[Author] = RTRIM(LEFT([Author], CHARINDEX(CHAR(10), [Author]) - 1)) " +
                "WHERE CHARINDEX(CHAR(10), [Author]) > 0')");

            // 4) pusta rola autora -> NULL
            migrationBuilder.Sql(
                "EXEC(N'UPDATE [Articles] SET [AuthorRole] = NULL " +
                "WHERE [AuthorRole] IS NOT NULL AND LEN([AuthorRole]) = 0')");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_RoleId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                "EXEC(N'UPDATE u SET u.[Role] = r.[Name] FROM [Users] u " +
                "INNER JOIN [Roles] r ON r.[Id] = u.[RoleId]')");

            migrationBuilder.Sql(
                "EXEC(N'UPDATE [Articles] SET [Author] = [Author] + CHAR(10) + [AuthorRole] " +
                "WHERE [AuthorRole] IS NOT NULL')");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AuthorRole",
                table: "Articles");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
