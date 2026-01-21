using Microsoft.Extensions.Logging;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Streaming.Application.Abstractions.Services;
using SpotifyClone.Streaming.Application.Errors;
using Xabe.FFmpeg;

namespace SpotifyClone.Streaming.Infrastructure.Media;

public class FfmpegMediaService(ILogger<FfmpegMediaService> logger) : IMediaService
{
    private readonly ILogger<FfmpegMediaService> _logger = logger;

    public async Task<Result<string>> ConvertToHlsAsync(string sourceFilePath, string outputFolder, Guid songId)
    {
        if (!File.Exists(sourceFilePath))
        {
            return Result.Failure<string>(MediaErrors.SourceFileNotFound);
        }

        try
        {
            string songIdStr = songId.ToString();
            string specificSongFolder = Path.Combine(outputFolder, songIdStr);

            if (!Directory.Exists(specificSongFolder))
            {
                Directory.CreateDirectory(specificSongFolder);
            }

            string outputPath = Path.Combine(specificSongFolder, "playlist.m3u8");

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(sourceFilePath);
            IAudioStream? audioStream = mediaInfo.AudioStreams.FirstOrDefault();

            if (audioStream == null)
            {
                return Result.Failure<string>(MediaErrors.AudioStreamNotFound);
            }

            _logger.LogInformation("Starting HLS conversion for {SongId}...", songId);

            IConversion conversion = FFmpeg.Conversions.New()
                .AddStream(audioStream)
                .SetOutput(outputPath)
                .AddParameter("-c:a aac")         // Кодуємо в AAC (найсумісніший формат)
                .AddParameter("-b:a 192k")        // Бітрейт (якість звуку, як у Spotify High)
                .AddParameter("-vn")              // Вимкнути відео (Video No)
                .AddParameter("-hls_time 10")     // Довжина одного шматочка (сегмента) в секундах
                .AddParameter("-hls_list_size 0") // 0 означає "зберегти всі сегменти в плейлісті" (для VOD)
                .AddParameter("-f hls");          // Формат виходу - HLS

            await conversion.Start();

            _logger.LogInformation("HLS conversion finished for {SongId}", songId);

            string relativePath = $"{songIdStr}/playlist.m3u8";

            return Result.Success(relativePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting song to HLS");

            return Result.Failure<string>(MediaErrors.ConversionFailed);
        }
    }
}
