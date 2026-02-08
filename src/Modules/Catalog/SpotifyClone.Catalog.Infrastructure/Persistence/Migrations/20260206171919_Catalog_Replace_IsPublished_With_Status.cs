using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class Catalog_Replace_IsPublished_With_Status : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "is_published",
            schema: "catalog",
            table: "tracks");

        migrationBuilder.AddColumn<string>(
            name: "status",
            schema: "catalog",
            table: "tracks",
            type: "text",
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "status",
            schema: "catalog",
            table: "tracks");

        migrationBuilder.AddColumn<bool>(
            name: "is_published",
            schema: "catalog",
            table: "tracks",
            type: "boolean",
            nullable: false,
            defaultValue: false);
    }
}
