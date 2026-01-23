using Microsoft.Extensions.Logging;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Streaming.Application.Abstractions.Services;
using SpotifyClone.Streaming.Application.Errors;
using Xabe.FFmpeg;

namespace SpotifyClone.Streaming.Infrastructure.Media;

public class FfmpegMediaService(ILogger<FfmpegMediaService> logger) : IMediaService
{
    private readonly ILogger<FfmpegMediaService> _logger = logger;

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

            string outputPath = Path.Combine(specificAudioFolder, "manifest.mpd");

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(sourceFilePath);
            IAudioStream? audioStream = mediaInfo.AudioStreams.FirstOrDefault();

            if (audioStream == null)
            {
                return Result.Failure(MediaErrors.AudioStreamNotFound);
            }

            _logger.LogInformation("Starting Multi-Bitrate CMAF conversion for {SongId}...", audioId);

            // Формуємо команду
            IConversion conversion = FFmpeg.Conversions.New()
                .AddStream(audioStream) // Це додає перший потік (index 0)
                .SetOutput(outputPath)
                // Додаємо ДРУГИЙ потік вручну через map
                .AddParameter("-map 0:a:0")

                // Налаштування для потоку 0 (128k)
                .AddParameter("-c:a:0 aac")
                .AddParameter("-b:a:0 128k")

                // Налаштування для потоку 1 (192k)
                .AddParameter("-c:a:1 aac")
                .AddParameter("-b:a:1 192k")

                .AddParameter("-vn")
                .AddParameter("-f dash")
                .AddParameter("-hls_playlist 1")
                .AddParameter("-hls_master_name master.m3u8")
                .AddParameter("-seg_duration 10")
                .AddParameter("-use_template 1")
                .AddParameter("-use_timeline 1")
                .AddParameter("-adaptation_sets \"id=0,streams=a\"")
                // FFmpeg сам підставить 0 та 1 замість $RepresentationID$
                .AddParameter("-init_seg_name \"$RepresentationID$/init.m4s\"")
                .AddParameter("-media_seg_name \"$RepresentationID$/chunk_$Number$.m4s\"");

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
