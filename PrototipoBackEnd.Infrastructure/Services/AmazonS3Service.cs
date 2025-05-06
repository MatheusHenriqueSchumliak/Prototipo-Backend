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

		public async Task<List<string>> ListFiles(string prefix = "")
		{
			var request = new ListObjectsV2Request
			{
				BucketName = _bucketName,
				Prefix = prefix
			};

			var response = await _amazonS3.ListObjectsV2Async(request);
			return response.S3Objects.Select(o => o.Key).ToList();
		}
		public async Task<Stream> GetFile(string fileName)
		{
			try
			{
				var response = await _amazonS3.GetObjectAsync(_bucketName, fileName);
				return response.ResponseStream;
			}
			catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
			{
				return null; // ou lançar uma exceção customizada
			}
		}
		public async Task<bool> FileExists(string fileName)
		{
			try
			{
				var response = await _amazonS3.GetObjectMetadataAsync(_bucketName, fileName);
				return true;
			}
			catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
			{
				return false;
			}
		}
		public async Task<string> Upload(Stream fileStream, string fileName, string contentType)
		{
			if (fileStream == null || fileStream.Length == 0)
				throw new ArgumentException("Arquivo inválido.");

			var request = new PutObjectRequest
			{
				BucketName = _bucketName,
				Key = fileName,
				InputStream = fileStream,
				ContentType = contentType,
				AutoCloseStream = true
			};

			await _amazonS3.PutObjectAsync(request);
			return $"https://{_bucketName}.s3.amazonaws.com/{fileName}";
		}
		public async Task<bool> Delete(string fileName)
		{
			try
			{
				await _amazonS3.DeleteObjectAsync(_bucketName, fileName);
				return true;
			}
			catch (AmazonS3Exception ex)
			{
				// logar erro, se necessário
				return false;
			}
		}
	}
}
