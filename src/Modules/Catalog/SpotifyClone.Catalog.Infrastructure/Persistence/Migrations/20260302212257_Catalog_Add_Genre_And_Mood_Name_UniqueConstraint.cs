using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class Catalog_Add_Genre_And_Mood_Name_UniqueConstraint : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_moods_name",
            schema: "catalog",
            table: "moods",
            column: "name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_genres_name",
            schema: "catalog",
            table: "genres",
            column: "name",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_moods_name",
            schema: "catalog",
            table: "moods");

        migrationBuilder.DropIndex(
            name: "IX_genres_name",
            schema: "catalog",
            table: "genres");
    }
}
