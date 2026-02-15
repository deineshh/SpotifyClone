using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class Catalog_Make_ImageMetadata_Properties_NotNullable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<Guid>(
            name: "cover_image_id",
            schema: "catalog",
            table: "moods",
            type: "uuid",
            nullable: true,
            oldClrType: typeof(Guid),
            oldType: "uuid");

        migrationBuilder.AlterColumn<Guid>(
            name: "cover_image_id",
            schema: "catalog",
            table: "genres",
            type: "uuid",
            nullable: true,
            oldClrType: typeof(Guid),
            oldType: "uuid");

        migrationBuilder.AlterColumn<int>(
            name: "metadata_width",
            schema: "catalog",
            table: "artist_gallery_images",
            type: "integer",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "integer",
            oldNullable: true);

        migrationBuilder.AlterColumn<long>(
            name: "metadata_size_in_bytes",
            schema: "catalog",
            table: "artist_gallery_images",
            type: "bigint",
            nullable: false,
            defaultValue: 0L,
            oldClrType: typeof(long),
            oldType: "bigint",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "metadata_height",
            schema: "catalog",
            table: "artist_gallery_images",
            type: "integer",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "integer",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "metadata_file_type",
            schema: "catalog",
            table: "artist_gallery_images",
            type: "text",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        migrationBuilder.AlterColumn<Guid>(
            name: "cover_image_id",
            schema: "catalog",
            table: "albums",
            type: "uuid",
            nullable: true,
            oldClrType: typeof(Guid),
            oldType: "uuid");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<Guid>(
            name: "cover_image_id",
            schema: "catalog",
            table: "moods",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty,
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldNullable: true);

        migrationBuilder.AlterColumn<Guid>(
            name: "cover_image_id",
            schema: "catalog",
            table: "genres",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty,
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "metadata_width",
            schema: "catalog",
            table: "artist_gallery_images",
            type: "integer",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "integer");

        migrationBuilder.AlterColumn<long>(
            name: "metadata_size_in_bytes",
            schema: "catalog",
            table: "artist_gallery_images",
            type: "bigint",
            nullable: true,
            oldClrType: typeof(long),
            oldType: "bigint");

        migrationBuilder.AlterColumn<int>(
            name: "metadata_height",
            schema: "catalog",
            table: "artist_gallery_images",
            type: "integer",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "integer");

        migrationBuilder.AlterColumn<string>(
            name: "metadata_file_type",
            schema: "catalog",
            table: "artist_gallery_images",
            type: "text",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "text");

        migrationBuilder.AlterColumn<Guid>(
            name: "cover_image_id",
            schema: "catalog",
            table: "albums",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty,
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldNullable: true);
    }
}
