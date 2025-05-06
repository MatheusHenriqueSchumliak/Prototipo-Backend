using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Infrastructure.Configurations;
using PrototipoBackEnd.Domain.Interfaces.Services;
using PrototipoBackEnd.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;
using PrototipoBackEnd.Infrastructure.Services;
using PrototipoBackEnd.Infrastructure.Context;
using PrototipoBackEnd.Application.Interfaces;
using MongoDB.Bson.Serialization.Serializers;
using PrototipoBackEnd.Application.Services;
using PrototipoBackEnd.Application.Mapping;
using PrototipoBackEnd.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Bson;
using Amazon.Runtime;
using Amazon.S3;
using Amazon;

namespace PrototipoBackEnd.Infrastructure.IoC
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
			// Registra a serialização do Guid
			BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

			#region AutoMapper Configuração

			services.AddAutoMapper(typeof(UsuarioProfile).Assembly);
			services.AddAutoMapper(typeof(ArtesaoProfile).Assembly);

			#endregion

			#region AWS Settings
			services.Configure<AWSSettings>(configuration.GetSection("AWS"));

			services.AddSingleton<IAmazonS3>(resolver =>
			{
				var awsSettings = resolver.GetRequiredService<IOptions<AWSSettings>>().Value;
				var credentials = new BasicAWSCredentials(awsSettings.AccessKey, awsSettings.SecretKey);
				var config = new AmazonS3Config
				{
					RegionEndpoint = RegionEndpoint.GetBySystemName(awsSettings.Region)
				};
				return new AmazonS3Client(credentials, config);
			});

			services.AddScoped<IAmazonS3Service, AmazonS3Service>();
			#endregion

			#region MongoDB Settings
			services.Configure<MongoDbSettings>(configuration.GetSection("MongoDBSettings"));

			services.AddSingleton<IMongoDatabase>(serviceProvider =>
			{
				var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
				var client = new MongoClient(settings.ConnectionString);
				return client.GetDatabase(settings.DatabaseName);
			});

			services.AddScoped<MongoDbContext>();
			#endregion

			#region Repositórios --- Injeção de dependencia  
			// Registro genérico  
			services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

			// Aqui você pode registrar repositórios específicos  
			services.AddScoped<IUsuarioRepository, UsuarioRepository>();
			services.AddScoped<IArtesaoRepository, ArtesaoRepository>();
			#endregion ---------------------------------  

			#region Serviços --- Injeção de dependencia  

			services.AddScoped<IUsuarioService, UsuarioService>();
			services.AddScoped<IArtesaoService, ArtesaoService>();
			services.AddScoped<ISenhaService, SenhaService>();

			services.AddScoped<IAmazonS3Service, AmazonS3Service>();


			#endregion ---------------------------------  

			return services;
		}
	}
}
