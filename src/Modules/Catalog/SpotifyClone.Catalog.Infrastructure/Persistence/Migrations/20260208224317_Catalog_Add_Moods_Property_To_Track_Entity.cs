using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class Catalog_Add_Moods_Property_To_Track_Entity : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
        => migrationBuilder.CreateTable(
            name: "track_moods",
            schema: "catalog",
            columns: table => new
            {
                mood_id = table.Column<Guid>(type: "uuid", nullable: false),
                track_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_track_moods", x => new { x.track_id, x.mood_id });
                table.ForeignKey(
                    name: "FK_track_moods_tracks_track_id",
                    column: x => x.track_id,
                    principalSchema: "catalog",
                    principalTable: "tracks",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
        => migrationBuilder.DropTable(
            name: "track_moods",
            schema: "catalog");
}
