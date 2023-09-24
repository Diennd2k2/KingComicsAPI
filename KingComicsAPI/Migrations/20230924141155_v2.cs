using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KingComicsAPI.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "reading_history",
                columns: table => new
                {
                    User_id = table.Column<Guid>(type: "uuid", nullable: false),
                    Comic_id = table.Column<Guid>(type: "uuid", nullable: false),
                    Chapter_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reading_history", x => new { x.User_id, x.Comic_id, x.Chapter_id });
                    table.ForeignKey(
                        name: "FK_reading_history_chapter_Chapter_id",
                        column: x => x.Chapter_id,
                        principalTable: "chapter",
                        principalColumn: "Chapter_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reading_history_comic_Comic_id",
                        column: x => x.Comic_id,
                        principalTable: "comic",
                        principalColumn: "Comic_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reading_history_user_User_id",
                        column: x => x.User_id,
                        principalTable: "user",
                        principalColumn: "User_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_reading_history_Chapter_id",
                table: "reading_history",
                column: "Chapter_id");

            migrationBuilder.CreateIndex(
                name: "IX_reading_history_Comic_id",
                table: "reading_history",
                column: "Comic_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reading_history");
        }
    }
}
