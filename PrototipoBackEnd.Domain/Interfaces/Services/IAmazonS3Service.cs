namespace PrototipoBackEnd.Domain.Interfaces.Services
{
	public interface IAmazonS3Service
	{
		Task<string> Upload(Stream fileStream, string fileName, string contentType);
		Task<Stream> GetFile(string fileName);
		Task Delete(string fileName);
	}
}
