using SpotifyClone.Shared.BuildingBlocks.Application.Errors;

namespace SpotifyClone.Streaming.Application.Errors;

public static class MediaErrors
{
    public static readonly Error SourceFileNotFound = new(
        "Media.SourceFileNotFound",
        "Source file was not found.");

    public static readonly Error AudioStreamNotFound = new(
        "Media.AudioStreamNotFound",
        "No audio stream found in file.");

    public static readonly Error AudioFileNotFound = new(
        "Media.AudioFileNotFound",
        "The audio file was not found or the upload is not yet complete.");

    public static readonly Error ConversionFailed = new(
        "Media.ConversionFailed",
        "Audio file conversion failed.");

    public static readonly Error UploadFailed = new(
        "Media.UploadFailed",
        "Audio file upload failed.");
}
