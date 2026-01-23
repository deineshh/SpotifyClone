using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpotifyClone.Streaming.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class Streaming_Create_Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "streaming");

        migrationBuilder.CreateTable(
            name: "audio_assets",
            schema: "streaming",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                format = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                file_size_in_bytes = table.Column<long>(type: "bigint", nullable: false),
                is_ready = table.Column<bool>(type: "boolean", nullable: false),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_audio_assets", x => x.id));
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
        => migrationBuilder.DropTable(
            name: "audio_assets",
            schema: "streaming");
}
