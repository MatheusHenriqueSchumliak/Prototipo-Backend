namespace PrototipoBackEnd.Application.Interfaces
{
	public interface IAWSSettings
	{
		string AccessKey { get; }
		string SecretKey { get; }
		string Region { get; }
		string BucketName { get; }
	}
}
