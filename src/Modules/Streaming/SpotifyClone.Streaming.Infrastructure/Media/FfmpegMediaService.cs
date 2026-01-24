using Microsoft.Extensions.Logging;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Streaming.Application.Abstractions.Services;
using SpotifyClone.Streaming.Application.Abstractions.Services.Models;
using SpotifyClone.Streaming.Application.Errors;
using Xabe.FFmpeg;

namespace SpotifyClone.Streaming.Infrastructure.Media;

public class FfmpegMediaService(ILogger<FfmpegMediaService> logger) : IMediaService
{
    private readonly ILogger<FfmpegMediaService> _logger = logger;

    public async Task<AudioMetadata> GetAudioMetadataAsync(string filePath)
    {
        IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(filePath);
        IAudioStream? audioStream = mediaInfo.AudioStreams.FirstOrDefault();
        var fileInfo = new FileInfo(filePath);

        return new AudioMetadata(
            audioStream?.Duration ?? TimeSpan.Zero,
            Path.GetExtension(filePath).Replace(".", ""),
            fileInfo.Length,
            audioStream?.Bitrate ?? 0
        );
    }

    public async Task<Result> ConvertToHlsAsync(string sourceFilePath, string outputFolder, Guid audioId)
    {
        if (!File.Exists(sourceFilePath))
        {
            return Result.Failure(MediaErrors.SourceFileNotFound);
        }

        string specificAudioFolder = Path.Combine(outputFolder, audioId.ToString());

        try
        {
            // 1. Створюємо головну папку
            if (!Directory.Exists(specificAudioFolder))
            {
                Directory.CreateDirectory(specificAudioFolder);
            }

            // 2. Створюємо папки 0 та 1 вручну (FFmpeg буде використовувати їх як Representation ID)
            string[] representationIds = ["0", "1"];
            foreach (string id in representationIds)
            {
                string subFolder = Path.Combine(specificAudioFolder, id);
                if (!Directory.Exists(subFolder))
                {
                    Directory.CreateDirectory(subFolder);
                }
            }

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(sourceFilePath);
            IAudioStream? audioStream = mediaInfo.AudioStreams.FirstOrDefault();

            if (audioStream == null)
            {
                return Result.Failure(MediaErrors.AudioStreamNotFound);
            }

            _logger.LogInformation("Starting Multi-Bitrate HLS conversion for {AudioId}...", audioId);

            // Використовуємо прямі слеші для FFmpeg
            string normalizedPath = specificAudioFolder.Replace("\\", "/");

            IConversion conversion = FFmpeg.Conversions.New()
                .AddStream(audioStream) // Це автоматично створює перший потік (a:0)
                .AddParameter("-map 0:a:0") // Створюємо ДРУГИЙ потік (a:1) вручну

                // ТУТ ВАЖЛИВО: тільки два потоки!
                .AddParameter("-c:a:0 aac -b:a:0 128k")
                .AddParameter("-c:a:1 aac -b:a:1 192k")
                .AddParameter("-vn")

                .AddParameter("-f hls")
                .AddParameter("-hls_segment_type fmp4")
                .AddParameter("-hls_time 10")
                .AddParameter("-hls_list_size 0")

                // Мапимо два потоки на дві папки: 0 та 1
                .AddParameter("-var_stream_map \"a:0,name:0 a:1,name:1\"")

                .AddParameter("-hls_playlist_type vod")
                .AddParameter("-master_pl_name master.m3u8")

                // Використовуємо шаблони для імен файлів
                .AddParameter($"-hls_segment_filename \"{normalizedPath}/%v/chunk_%03d.m4s\"")

                // ВАЖЛИВО: SetOutput має бути таким, щоб FFmpeg міг підставити %v
                // Не використовувати Path.Combine для частини з %v, бо він може додати зайві слеші
                .SetOutput($"{normalizedPath}/%v/index.m3u8");

            await conversion.Start();

            _logger.LogInformation("Multi-Bitrate conversion finished for {AudioId}", audioId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting song. Cleaning up...");

            if (Directory.Exists(specificAudioFolder))
            {
                Directory.Delete(specificAudioFolder, true);
            }

            return Result.Failure(MediaErrors.ConversionFailed);
        }
    }
}
