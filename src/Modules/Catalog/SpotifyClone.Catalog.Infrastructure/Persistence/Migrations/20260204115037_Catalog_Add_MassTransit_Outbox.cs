using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class Catalog_Add_MassTransit_Outbox : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "catalog");

        migrationBuilder.CreateTable(
            name: "albums",
            schema: "catalog",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                title = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                release_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                is_published = table.Column<bool>(type: "boolean", nullable: false),
                type = table.Column<string>(type: "text", nullable: true),
                cover_metadata_width = table.Column<int>(type: "integer", nullable: true),
                cover_metadata_height = table.Column<int>(type: "integer", nullable: true),
                cover_metadata_file_type = table.Column<string>(type: "text", nullable: true),
                cover_metadata_size_in_bytes = table.Column<long>(type: "bigint", nullable: true),
                cover_image_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_albums", x => x.id));

        migrationBuilder.CreateTable(
            name: "artists",
            schema: "catalog",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                bio = table.Column<string>(type: "character varying(1500)", maxLength: 1500, nullable: true),
                is_verified = table.Column<bool>(type: "boolean", nullable: false),
                avatar_metadata_width = table.Column<int>(type: "integer", nullable: true),
                avatar_metadata_height = table.Column<int>(type: "integer", nullable: true),
                avatar_metadata_file_type = table.Column<string>(type: "text", nullable: true),
                avatar_metadata_size_in_bytes = table.Column<long>(type: "bigint", nullable: true),
                avatar_image_id = table.Column<Guid>(type: "uuid", nullable: true),
                banner_metadata_width = table.Column<int>(type: "integer", nullable: true),
                banner_metadata_height = table.Column<int>(type: "integer", nullable: true),
                banner_metadata_file_type = table.Column<string>(type: "text", nullable: true),
                banner_metadata_size_in_bytes = table.Column<long>(type: "bigint", nullable: true),
                banner_image_id = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_artists", x => x.id));

        migrationBuilder.CreateTable(
            name: "genres",
            schema: "catalog",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                cover_metadata_width = table.Column<int>(type: "integer", nullable: true),
                cover_metadata_height = table.Column<int>(type: "integer", nullable: true),
                cover_metadata_file_type = table.Column<string>(type: "text", nullable: true),
                cover_metadata_size_in_bytes = table.Column<long>(type: "bigint", nullable: true),
                cover_image_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_genres", x => x.id));

        migrationBuilder.CreateTable(
            name: "InboxState",
            schema: "catalog",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                ConsumerId = table.Column<Guid>(type: "uuid", nullable: false),
                LockId = table.Column<Guid>(type: "uuid", nullable: false),
                RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                Received = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ReceiveCount = table.Column<int>(type: "integer", nullable: false),
                ExpirationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                Consumed = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                Delivered = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InboxState", x => x.Id);
                table.UniqueConstraint("AK_InboxState_MessageId_ConsumerId", x => new { x.MessageId, x.ConsumerId });
            });

        migrationBuilder.CreateTable(
            name: "moods",
            schema: "catalog",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                cover_metadata_width = table.Column<int>(type: "integer", nullable: true),
                cover_metadata_height = table.Column<int>(type: "integer", nullable: true),
                cover_metadata_file_type = table.Column<string>(type: "text", nullable: true),
                cover_metadata_size_in_bytes = table.Column<long>(type: "bigint", nullable: true),
                cover_image_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_moods", x => x.id));

        migrationBuilder.CreateTable(
            name: "OutboxMessage",
            schema: "catalog",
            columns: table => new
            {
                SequenceNumber = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                EnqueueTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                SentTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Headers = table.Column<string>(type: "text", nullable: true),
                Properties = table.Column<string>(type: "text", nullable: true),
                InboxMessageId = table.Column<Guid>(type: "uuid", nullable: true),
                InboxConsumerId = table.Column<Guid>(type: "uuid", nullable: true),
                OutboxId = table.Column<Guid>(type: "uuid", nullable: true),
                MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                ContentType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                MessageType = table.Column<string>(type: "text", nullable: false),
                Body = table.Column<string>(type: "text", nullable: false),
                ConversationId = table.Column<Guid>(type: "uuid", nullable: true),
                CorrelationId = table.Column<Guid>(type: "uuid", nullable: true),
                InitiatorId = table.Column<Guid>(type: "uuid", nullable: true),
                RequestId = table.Column<Guid>(type: "uuid", nullable: true),
                SourceAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                DestinationAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                ResponseAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                FaultAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                ExpirationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_OutboxMessage", x => x.SequenceNumber));

        migrationBuilder.CreateTable(
            name: "OutboxState",
            schema: "catalog",
            columns: table => new
            {
                OutboxId = table.Column<Guid>(type: "uuid", nullable: false),
                LockId = table.Column<Guid>(type: "uuid", nullable: false),
                RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Delivered = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_OutboxState", x => x.OutboxId));

        migrationBuilder.CreateTable(
            name: "tracks",
            schema: "catalog",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                title = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                duration = table.Column<TimeSpan>(type: "interval", nullable: true),
                release_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                contains_explicit_content = table.Column<bool>(type: "boolean", nullable: false),
                track_number = table.Column<int>(type: "integer", nullable: false),
                is_published = table.Column<bool>(type: "boolean", nullable: false),
                audio_file_id = table.Column<Guid>(type: "uuid", nullable: true),
                album_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_tracks", x => x.id));

        migrationBuilder.CreateTable(
            name: "album_main_artists",
            schema: "catalog",
            columns: table => new
            {
                artist_id = table.Column<Guid>(type: "uuid", nullable: false),
                album_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_album_main_artists", x => new { x.album_id, x.artist_id });
                table.ForeignKey(
                    name: "FK_album_main_artists_albums_album_id",
                    column: x => x.album_id,
                    principalSchema: "catalog",
                    principalTable: "albums",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "album_tracks",
            schema: "catalog",
            columns: table => new
            {
                track_id = table.Column<Guid>(type: "uuid", nullable: false),
                album_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_album_tracks", x => new { x.album_id, x.track_id });
                table.ForeignKey(
                    name: "FK_album_tracks_albums_album_id",
                    column: x => x.album_id,
                    principalSchema: "catalog",
                    principalTable: "albums",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "artist_gallery_images",
            schema: "catalog",
            columns: table => new
            {
                artist_id = table.Column<Guid>(type: "uuid", nullable: false),
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                metadata_width = table.Column<int>(type: "integer", nullable: true),
                metadata_height = table.Column<int>(type: "integer", nullable: true),
                metadata_file_type = table.Column<string>(type: "text", nullable: true),
                metadata_size_in_bytes = table.Column<long>(type: "bigint", nullable: true),
                image_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_artist_gallery_images", x => new { x.artist_id, x.Id });
                table.ForeignKey(
                    name: "FK_artist_gallery_images_artists_artist_id",
                    column: x => x.artist_id,
                    principalSchema: "catalog",
                    principalTable: "artists",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "track_featured_artists",
            schema: "catalog",
            columns: table => new
            {
                artist_id = table.Column<Guid>(type: "uuid", nullable: false),
                track_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_track_featured_artists", x => new { x.track_id, x.artist_id });
                table.ForeignKey(
                    name: "FK_track_featured_artists_tracks_track_id",
                    column: x => x.track_id,
                    principalSchema: "catalog",
                    principalTable: "tracks",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "track_genres",
            schema: "catalog",
            columns: table => new
            {
                genre_id = table.Column<Guid>(type: "uuid", nullable: false),
                track_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_track_genres", x => new { x.track_id, x.genre_id });
                table.ForeignKey(
                    name: "FK_track_genres_tracks_track_id",
                    column: x => x.track_id,
                    principalSchema: "catalog",
                    principalTable: "tracks",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "track_main_artists",
            schema: "catalog",
            columns: table => new
            {
                artist_id = table.Column<Guid>(type: "uuid", nullable: false),
                track_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_track_main_artists", x => new { x.track_id, x.artist_id });
                table.ForeignKey(
                    name: "FK_track_main_artists_tracks_track_id",
                    column: x => x.track_id,
                    principalSchema: "catalog",
                    principalTable: "tracks",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_albums_release_date",
            schema: "catalog",
            table: "albums",
            column: "release_date");

        migrationBuilder.CreateIndex(
            name: "IX_InboxState_Delivered",
            schema: "catalog",
            table: "InboxState",
            column: "Delivered");

        migrationBuilder.CreateIndex(
            name: "IX_OutboxMessage_EnqueueTime",
            schema: "catalog",
            table: "OutboxMessage",
            column: "EnqueueTime");

        migrationBuilder.CreateIndex(
            name: "IX_OutboxMessage_ExpirationTime",
            schema: "catalog",
            table: "OutboxMessage",
            column: "ExpirationTime");

        migrationBuilder.CreateIndex(
            name: "IX_OutboxMessage_InboxMessageId_InboxConsumerId_SequenceNumber",
            schema: "catalog",
            table: "OutboxMessage",
            columns: new[] { "InboxMessageId", "InboxConsumerId", "SequenceNumber" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_OutboxMessage_OutboxId_SequenceNumber",
            schema: "catalog",
            table: "OutboxMessage",
            columns: new[] { "OutboxId", "SequenceNumber" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_OutboxState_Created",
            schema: "catalog",
            table: "OutboxState",
            column: "Created");

        migrationBuilder.CreateIndex(
            name: "IX_tracks_album_id",
            schema: "catalog",
            table: "tracks",
            column: "album_id");

        migrationBuilder.CreateIndex(
            name: "IX_tracks_audio_file_id",
            schema: "catalog",
            table: "tracks",
            column: "audio_file_id",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "album_main_artists",
            schema: "catalog");

        migrationBuilder.DropTable(
            name: "album_tracks",
            schema: "catalog");

        migrationBuilder.DropTable(
            name: "artist_gallery_images",
            schema: "catalog");

        migrationBuilder.DropTable(
            name: "genres",
            schema: "catalog");

        migrationBuilder.DropTable(
            name: "InboxState",
            schema: "catalog");

        migrationBuilder.DropTable(
            name: "moods",
            schema: "catalog");

        migrationBuilder.DropTable(
            name: "OutboxMessage",
            schema: "catalog");

        migrationBuilder.DropTable(
            name: "OutboxState",
            schema: "catalog");

        migrationBuilder.DropTable(
            name: "track_featured_artists",
            schema: "catalog");

        migrationBuilder.DropTable(
            name: "track_genres",
            schema: "catalog");

        migrationBuilder.DropTable(
            name: "track_main_artists",
            schema: "catalog");

        migrationBuilder.DropTable(
            name: "albums",
            schema: "catalog");

        migrationBuilder.DropTable(
            name: "artists",
            schema: "catalog");

        migrationBuilder.DropTable(
            name: "tracks",
            schema: "catalog");
    }
}
