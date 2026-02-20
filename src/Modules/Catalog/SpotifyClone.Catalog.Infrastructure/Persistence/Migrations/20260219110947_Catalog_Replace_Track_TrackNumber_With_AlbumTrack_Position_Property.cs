using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class Catalog_Replace_Track_TrackNumber_With_AlbumTrack_Position_Property : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "track_number",
            schema: "catalog",
            table: "tracks");

        migrationBuilder.AddColumn<int>(
            name: "position",
            schema: "catalog",
            table: "album_tracks",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateIndex(
            name: "IX_album_tracks_album_id_position",
            schema: "catalog",
            table: "album_tracks",
            columns: new[] { "album_id", "position" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_album_tracks_album_id_position",
            schema: "catalog",
            table: "album_tracks");

        migrationBuilder.DropColumn(
            name: "position",
            schema: "catalog",
            table: "album_tracks");

        migrationBuilder.AddColumn<int>(
            name: "track_number",
            schema: "catalog",
            table: "tracks",
            type: "integer",
            nullable: false,
            defaultValue: 0);
    }
}
