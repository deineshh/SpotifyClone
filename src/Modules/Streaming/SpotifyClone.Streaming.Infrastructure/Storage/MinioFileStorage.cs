using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using SpotifyClone.Streaming.Application.Abstractions.Services;

namespace SpotifyClone.Streaming.Infrastructure.Storage;

public class MinioFileStorage : IFileStorage
{
    private const string BucketName = "audio";
    private readonly MinioOptions _options;
    private readonly IMinioClient _minioClient;

    public MinioFileStorage(IOptions<MinioOptions> options)
    {
        _options = options.Value;

        _minioClient = new MinioClient()
            .WithEndpoint(_options.Endpoint, _options.Port)
            .WithCredentials(_options.AccessKey, _options.SecretKey)
            .WithSSL(false)
            .Build();

        InitializeBucket().GetAwaiter().GetResult();
    }

    private async Task InitializeBucket()
    {
        bool found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(BucketName));
        if (!found)
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(BucketName));

            string policyJson = $$"""
                {
                    "Version": "2012-10-17",
                    "Statement": [
                        {
                            "Effect": "Allow",
                            "Principal": {"AWS": ["*"]},
                            "Action": ["s3:GetBucketLocation"],
                            "Resource": ["arn:aws:s3:::{{BucketName}}"]
                        },
                        {
                            "Effect": "Allow",
                            "Principal": {"AWS": ["*"]},
                            "Action": ["s3:ListBucket"],
                            "Resource": ["arn:aws:s3:::{{BucketName}}"]
                        },
                        {
                            "Effect": "Allow",
                            "Principal": {"AWS": ["*"]},
                            "Action": ["s3:GetObject"],
                            "Resource": ["arn:aws:s3:::{{BucketName}}/*"]
                        }
                    ]
                }
                """;
            await _minioClient.SetPolicyAsync(new SetPolicyArgs()
                .WithBucket(BucketName)
                .WithPolicy(policyJson));
        }
    }

    public string GetFullPath(string relativePath)
        => relativePath;

    public string GetAudioRootPath()
    {
        string baseUrl = _options.StorageUrl.TrimEnd('/');
        return $"{baseUrl}/{BucketName}";
    }

    public string GetLocalConversionRootPath()
        => _options.LocalScratchPath;

    public async Task SaveAudioFileAsync(Stream stream, string relativePath)
    {
        if (stream.Position != 0)
        {
            stream.Seek(0, SeekOrigin.Begin);
        }

        long size;
        Stream uploadStream = stream;
        MemoryStream? buffer = null;

        try
        {
            size = stream.Length;
        }
        catch (NotSupportedException)
        {
            buffer = new MemoryStream();
            await stream.CopyToAsync(buffer);
            buffer.Position = 0;
            uploadStream = buffer;
            size = buffer.Length;
        }

        try
        {
            PutObjectArgs putObjectArgs = new PutObjectArgs()
                .WithBucket(BucketName)
                .WithObject(relativePath)
                .WithStreamData(uploadStream)
                .WithObjectSize(size)
                .WithContentType("audio/mpeg");

            await _minioClient.PutObjectAsync(putObjectArgs);
        }
        finally
        {
            buffer?.Dispose();
        }
    }

    public async Task DeleteAudioFileAsync(string relativePath)
    {
        RemoveObjectArgs args = new RemoveObjectArgs()
            .WithBucket(BucketName)
            .WithObject(relativePath);

        await _minioClient.RemoveObjectAsync(args);
    }

    public async Task DownloadAudioToLocalFileAsync(string objectName, string localPath)
    {
        string? directory = Path.GetDirectoryName(localPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        GetObjectArgs args = new GetObjectArgs()
            .WithBucket(BucketName)
            .WithObject(objectName)
            .WithCallbackStream((stream) =>
            {
                using var fileStream = new FileStream(localPath, FileMode.Create);
                stream.CopyTo(fileStream);
            });

        await _minioClient.GetObjectAsync(args);
    }
}
