using System.Text.Json.Serialization;

namespace PrototipoBackEnd.API.Extensions
{
	public static class ControllerExtensions
	{
		public static IServiceCollection AddConfiguredControllers(this IServiceCollection services)
		{
			services.AddControllers()
					.AddJsonOptions(options =>
					{
						options.JsonSerializerOptions.PropertyNamingPolicy = null;
						options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
					});

			return services;
		}
	}
}
