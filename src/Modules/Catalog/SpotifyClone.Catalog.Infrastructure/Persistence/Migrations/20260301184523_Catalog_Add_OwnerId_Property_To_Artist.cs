using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class Catalog_Add_OwnerId_Property_To_Artist : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
        => migrationBuilder.AddColumn<Guid>(
            name: "owner_id",
            schema: "catalog",
            table: "artists",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty);

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
        => migrationBuilder.DropColumn(
            name: "owner_id",
            schema: "catalog",
            table: "artists");
}
