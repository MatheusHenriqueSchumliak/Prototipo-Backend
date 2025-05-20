namespace PrototipoBackEnd.API.WebApplicationExtensions
{
	public static class MiddlewareExtensions
	{
		public static WebApplication UseCustomMiddlewares(this WebApplication app)
		{
			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			// Configuração do pipeline HTTP
			app.UseCors("PermitirFrontend");
			app.UseHttpsRedirection();
			app.UseAuthorization();

			return app;
		}
	}
}
