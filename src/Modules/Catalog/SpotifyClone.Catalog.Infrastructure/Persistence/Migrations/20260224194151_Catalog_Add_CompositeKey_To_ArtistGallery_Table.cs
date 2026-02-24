using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class Catalog_Add_CompositeKey_To_ArtistGallery_Table : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_artist_gallery_images",
            schema: "catalog",
            table: "artist_gallery_images");

        migrationBuilder.DropColumn(
            name: "Id",
            schema: "catalog",
            table: "artist_gallery_images");

        migrationBuilder.AddPrimaryKey(
            name: "PK_artist_gallery_images",
            schema: "catalog",
            table: "artist_gallery_images",
            columns: new[] { "artist_id", "image_id" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_artist_gallery_images",
            schema: "catalog",
            table: "artist_gallery_images");

        migrationBuilder.AddColumn<int>(
            name: "Id",
            schema: "catalog",
            table: "artist_gallery_images",
            type: "integer",
            nullable: false,
            defaultValue: 0)
            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

        migrationBuilder.AddPrimaryKey(
            name: "PK_artist_gallery_images",
            schema: "catalog",
            table: "artist_gallery_images",
            columns: new[] { "artist_id", "Id" });
    }
}
