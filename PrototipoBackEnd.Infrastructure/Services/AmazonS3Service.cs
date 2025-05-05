using PrototipoBackEnd.Infrastructure.Configurations;
using PrototipoBackEnd.Domain.Interfaces.Services;
using Microsoft.Extensions.Options;
using Amazon.S3.Model;
using Amazon.S3;

namespace PrototipoBackEnd.Infrastructure.Services
{
	public class AmazonS3Service : IAmazonS3Service
	{
		private readonly IAmazonS3 _amazonS3;
		private readonly string _bucketName;
		public AmazonS3Service(IAmazonS3 amazonS3, IOptions<AWSSettings> awsSettings)
		{
			_amazonS3 = amazonS3;
			_bucketName = awsSettings.Value.BucketName;
		}

		public async Task<Stream> GetFile(string fileName)
		{
			var response = await _amazonS3.GetObjectAsync(_bucketName, fileName);
			return response.ResponseStream;
		}

		public async Task<string> Upload(Stream fileStream, string fileName, string contentType)
		{
			var request = new PutObjectRequest
			{
				BucketName = _bucketName,
				Key = fileName,
				InputStream = fileStream,
				ContentType = contentType
			};

			await _amazonS3.PutObjectAsync(request);
			return $"https://{_bucketName}.s3.amazonaws.com/{fileName}";
		}

		public async Task Delete(string fileName)
		{
			await _amazonS3.DeleteObjectAsync(_bucketName, fileName);
		}
	}
}
