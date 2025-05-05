using PrototipoBackEnd.Application.Interfaces;

namespace PrototipoBackEnd.Infrastructure.Configurations
{
	public class AWSSettings : IAWSSettings
	{
		public string AccessKey { get; set; } = string.Empty;
		public string SecretKey { get; set; } = string.Empty;
		public string Region { get; set; } = string.Empty;
		public string BucketName { get; set; } = string.Empty;
	}
}
