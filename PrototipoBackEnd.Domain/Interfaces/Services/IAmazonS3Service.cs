namespace PrototipoBackEnd.Domain.Interfaces.Services
{
	public interface IAmazonS3Service
	{
		Task<string> Upload(Stream fileStream, string fileName, string contentType);
		Task<Stream> GetFile(string fileName);
		Task<bool> Delete(string fileName);
		Task<bool> FileExists(string fileName);
		Task<List<string>> ListFiles(string prefix = "");
	}
}
