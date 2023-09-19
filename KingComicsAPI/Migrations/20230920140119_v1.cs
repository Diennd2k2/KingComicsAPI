using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KingComicsAPI.Migrations
{
    /// <inheritdoc />
    public partial class v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "admin",
                columns: table => new
                {
                    Admin_id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin", x => x.Admin_id);
                });

            migrationBuilder.CreateTable(
                name: "comic",
                columns: table => new
                {
                    Comic_id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Slug = table.Column<string>(type: "text", nullable: true),
                    CoverImage = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comic", x => x.Comic_id);
                });

            migrationBuilder.CreateTable(
                name: "genre",
                columns: table => new
                {
                    Genre_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Genre_Name = table.Column<string>(type: "text", nullable: true),
                    Slug = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_genre", x => x.Genre_id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    User_id = table.Column<Guid>(type: "uuid", nullable: false),
                    NickName = table.Column<string>(type: "text", nullable: true),
                    Avatar = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.User_id);
                });

            migrationBuilder.CreateTable(
                name: "chapter",
                columns: table => new
                {
                    Chapter_id = table.Column<Guid>(type: "uuid", nullable: false),
                    Comic_id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Slug = table.Column<string>(type: "text", nullable: true),
                    Arrange = table.Column<int>(type: "integer", nullable: false),
                    Views = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chapter", x => x.Chapter_id);
                    table.ForeignKey(
                        name: "FK_chapter_comic_Comic_id",
                        column: x => x.Comic_id,
                        principalTable: "comic",
                        principalColumn: "Comic_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comic_genre",
                columns: table => new
                {
                    Comic_id = table.Column<Guid>(type: "uuid", nullable: false),
                    Genre_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comic_genre", x => new { x.Comic_id, x.Genre_id });
                    table.ForeignKey(
                        name: "FK_comic_genre_comic_Comic_id",
                        column: x => x.Comic_id,
                        principalTable: "comic",
                        principalColumn: "Comic_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comic_genre_genre_Genre_id",
                        column: x => x.Genre_id,
                        principalTable: "genre",
                        principalColumn: "Genre_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "follow_comic",
                columns: table => new
                {
                    User_id = table.Column<Guid>(type: "uuid", nullable: false),
                    Comic_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_follow_comic", x => new { x.User_id, x.Comic_id });
                    table.ForeignKey(
                        name: "FK_follow_comic_comic_Comic_id",
                        column: x => x.Comic_id,
                        principalTable: "comic",
                        principalColumn: "Comic_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_follow_comic_user_User_id",
                        column: x => x.User_id,
                        principalTable: "user",
                        principalColumn: "User_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "image",
                columns: table => new
                {
                    Image_id = table.Column<Guid>(type: "uuid", nullable: false),
                    UrlImage = table.Column<string>(type: "text", nullable: true),
                    Arrange = table.Column<int>(type: "integer", nullable: false),
                    Chapter_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_image", x => x.Image_id);
                    table.ForeignKey(
                        name: "FK_image_chapter_Chapter_id",
                        column: x => x.Chapter_id,
                        principalTable: "chapter",
                        principalColumn: "Chapter_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_chapter_Comic_id",
                table: "chapter",
                column: "Comic_id");

            migrationBuilder.CreateIndex(
                name: "IX_comic_Title",
                table: "comic",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_comic_genre_Genre_id",
                table: "comic_genre",
                column: "Genre_id");

            migrationBuilder.CreateIndex(
                name: "IX_follow_comic_Comic_id",
                table: "follow_comic",
                column: "Comic_id");

            migrationBuilder.CreateIndex(
                name: "IX_image_Chapter_id",
                table: "image",
                column: "Chapter_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin");

            migrationBuilder.DropTable(
                name: "comic_genre");

            migrationBuilder.DropTable(
                name: "follow_comic");

            migrationBuilder.DropTable(
                name: "image");

            migrationBuilder.DropTable(
                name: "genre");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "chapter");

            migrationBuilder.DropTable(
                name: "comic");
        }
    }
}
