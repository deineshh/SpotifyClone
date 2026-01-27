using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using SpotifyClone.Streaming.Application.Abstractions.Services;

namespace SpotifyClone.Streaming.Infrastructure.Storage;

public class MinioFileStorage : IFileStorage
{
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

        InitializeAudioBucket().GetAwaiter().GetResult();
        InitializeImagesBucket().GetAwaiter().GetResult();
    }

    public string GetFullPath(string relativePath)
        => relativePath;

    public string GetImageRootPath()
    {
        string baseUrl = _options.StorageUrl.TrimEnd('/');
        return $"{baseUrl}/{_options.ImageBucketName}";
    }

    public string GetAudioRootPath()
    {
        string baseUrl = _options.StorageUrl.TrimEnd('/');
        return $"{baseUrl}/{_options.AudioBucketName}";
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
                .WithBucket(_options.AudioBucketName)
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

    public async Task SaveImageFileAsync(Stream stream, string relativePath)
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
                .WithBucket(_options.ImageBucketName)
                .WithObject(relativePath)
                .WithStreamData(uploadStream)
                .WithObjectSize(size)
                .WithContentType("image/jpeg");
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
            .WithBucket(_options.AudioBucketName)
            .WithObject(relativePath);

        await _minioClient.RemoveObjectAsync(args);
    }

    public async Task DeleteImageFileAsync(string relativePath)
    {
        RemoveObjectArgs args = new RemoveObjectArgs()
            .WithBucket(_options.ImageBucketName)
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
            .WithBucket(_options.AudioBucketName)
            .WithObject(objectName)
            .WithCallbackStream((stream) =>
            {
                using var fileStream = new FileStream(localPath, FileMode.Create);
                stream.CopyTo(fileStream);
            });

        await _minioClient.GetObjectAsync(args);
    }

    public async Task DownloadImageToLocalFileAsync(string objectName, string localPath)
    {
        string? directory = Path.GetDirectoryName(localPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        GetObjectArgs args = new GetObjectArgs()
            .WithBucket(_options.ImageBucketName)
            .WithObject(objectName)
            .WithCallbackStream((stream) =>
            {
                using var fileStream = new FileStream(localPath, FileMode.Create);
                stream.CopyTo(fileStream);
            });
        await _minioClient.GetObjectAsync(args);
    }

    private async Task InitializeAudioBucket()
    {
        bool found = await _minioClient.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_options.AudioBucketName));
        if (!found)
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_options.AudioBucketName));

            string policyJson = $$"""
                {
                    "Version": "2012-10-17",
                    "Statement": [
                        {
                            "Effect": "Allow",
                            "Principal": {"AWS": ["*"]},
                            "Action": ["s3:GetBucketLocation"],
                            "Resource": ["arn:aws:s3:::{{_options.AudioBucketName}}"]
                        },
                        {
                            "Effect": "Allow",
                            "Principal": {"AWS": ["*"]},
                            "Action": ["s3:ListBucket"],
                            "Resource": ["arn:aws:s3:::{{_options.AudioBucketName}}"]
                        },
                        {
                            "Effect": "Allow",
                            "Principal": {"AWS": ["*"]},
                            "Action": ["s3:GetObject"],
                            "Resource": ["arn:aws:s3:::{{_options.AudioBucketName}}/*"]
                        }
                    ]
                }
                """;
            await _minioClient.SetPolicyAsync(new SetPolicyArgs()
                .WithBucket(_options.AudioBucketName)
                .WithPolicy(policyJson));
        }
    }

    private async Task InitializeImagesBucket()
    {
        bool found = await _minioClient.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_options.ImageBucketName));
        if (!found)
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_options.ImageBucketName));

            string policyJson = $$"""
                {
                    "Version": "2012-10-17",
                    "Statement": [
                        {
                            "Effect": "Allow",
                            "Principal": {"AWS": ["*"]},
                            "Action": ["s3:GetBucketLocation"],
                            "Resource": ["arn:aws:s3:::{{_options.ImageBucketName}}"]
                        },
                        {
                            "Effect": "Allow",
                            "Principal": {"AWS": ["*"]},
                            "Action": ["s3:ListBucket"],
                            "Resource": ["arn:aws:s3:::{{_options.ImageBucketName}}"]
                        },
                        {
                            "Effect": "Allow",
                            "Principal": {"AWS": ["*"]},
                            "Action": ["s3:GetObject"],
                            "Resource": ["arn:aws:s3:::{{_options.ImageBucketName}}/*"]
                        }
                    ]
                }
                """;
            await _minioClient.SetPolicyAsync(new SetPolicyArgs()
                .WithBucket(_options.ImageBucketName)
                .WithPolicy(policyJson));
        }
    }
}
