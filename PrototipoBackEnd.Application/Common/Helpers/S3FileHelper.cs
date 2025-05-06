namespace PrototipoBackEnd.Application.Common.Helpers
{
	public static class S3FileHelper
	{
		public static string GerarCaminhoArquivo(string folder, string originalFileName)
		{
			var extension = Path.GetExtension(originalFileName);
			var uniqueName = $"{Guid.NewGuid()}{extension}";
			var datePrefix = DateTime.UtcNow.ToString("yyyy/MM/dd");

			// Exemplo: uploads/2025/05/06/abc123xyz.png
			return Path.Combine(folder, datePrefix, uniqueName).Replace("\\", "/");
		}

	}

}
