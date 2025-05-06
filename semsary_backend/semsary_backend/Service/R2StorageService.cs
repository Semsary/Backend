using Amazon.S3.Model;
using Amazon.S3;
using Microsoft.Extensions.Options;

namespace semsary_backend.Service
{
    public class R2StorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly CloudflareR2Settings _settings;

        public R2StorageService(IAmazonS3 s3Client, IOptions<CloudflareR2Settings> settings)
        {
            _s3Client = s3Client;
            _settings = settings.Value;
        }

        
        public async Task<string> UploadFileAsync(IFormFile file)
        {

            var key = Ulid.NewUlid().ToString() + Path.GetExtension(file.FileName);
            // Read the entire file into memory first
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0; // Reset position

            var request = new PutObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = key,
                InputStream = memoryStream,
                ContentType = file.ContentType,
                AutoCloseStream = false,
                // Disable MD5 checksum (optional but recommended for R2)
                UseChunkEncoding = false // Critical for R2 compatibility
            };

            await _s3Client.PutObjectAsync(request);

            return key;
        }
        public async Task<Stream> GetFileAsync(string key)
        {
            var response = await _s3Client.GetObjectAsync(_settings.BucketName, key);
              
            return response.ResponseStream;
        }

        public async Task DeleteFileAsync(string key)
        {
            await _s3Client.DeleteObjectAsync(_settings.BucketName, key);
        }
    }

}
