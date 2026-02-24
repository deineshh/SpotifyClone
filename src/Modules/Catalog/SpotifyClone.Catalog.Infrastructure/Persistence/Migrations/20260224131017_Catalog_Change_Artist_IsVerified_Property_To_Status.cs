using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class Catalog_Change_Artist_IsVerified_Property_To_Status : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "is_verified",
            schema: "catalog",
            table: "artists");

        migrationBuilder.AddColumn<string>(
            name: "status",
            schema: "catalog",
            table: "artists",
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
            table: "artists");

        migrationBuilder.AddColumn<bool>(
            name: "is_verified",
            schema: "catalog",
            table: "artists",
            type: "boolean",
            nullable: false,
            defaultValue: false);
    }
}
