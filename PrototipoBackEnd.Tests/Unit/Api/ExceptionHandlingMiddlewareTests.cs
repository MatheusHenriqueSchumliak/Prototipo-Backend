using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using PrototipoBackEnd.API.Middlewares;
using PrototipoBackEnd.Application.Exceptions;
using Xunit;

namespace PrototipoBackEnd.Tests.Unit.Api
{
    public class ExceptionHandlingMiddlewareTests
    {
        private static async Task<(int Status, JsonElement Corpo, string Texto)> Executar(Exception ex)
        {
            var middleware = new ExceptionHandlingMiddleware(_ => throw ex, NullLogger<ExceptionHandlingMiddleware>.Instance);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await middleware.InvokeAsync(context);

            context.Response.Body.Position = 0;
            var texto = await new StreamReader(context.Response.Body).ReadToEndAsync();
            return (context.Response.StatusCode, JsonDocument.Parse(texto).RootElement.Clone(), texto);
        }

        [Theory]
        [MemberData(nameof(Mapeamentos))]
        public async Task ConverteExcecaoEmStatusEEnvelope(Exception ex, int statusEsperado)
        {
            var (status, corpo, _) = await Executar(ex);

            Assert.Equal(statusEsperado, status);
            Assert.True(corpo.TryGetProperty("message", out _));
        }

        public static IEnumerable<object[]> Mapeamentos()
        {
            yield return new object[] { new NotFoundException("x"), 404 };
            yield return new object[] { new ConflictException("x"), 409 };
            yield return new object[] { new ValidationException("x"), 400 };
            yield return new object[] { new ArgumentException("x"), 400 };
            yield return new object[] { new UnauthorizedAccessException("segredo"), 401 };
            yield return new object[] { new InvalidOperationException("x"), 500 };
        }

        [Fact]
        public async Task ErroInesperado_Retorna500SemVazarDetalhes()
        {
            var (status, _, texto) = await Executar(new InvalidOperationException("string de conexão mongodb://user:pass@host"));

            Assert.Equal(500, status);
            Assert.DoesNotContain("mongodb", texto);
            Assert.DoesNotContain("InvalidOperationException", texto);
            Assert.DoesNotContain("   at ", texto);
        }

        [Fact]
        public async Task ErroDeValidacao_UsaMensagemDaExcecao()
        {
            var (_, corpo, _) = await Executar(new ValidationException("ID inválido."));

            Assert.Equal("ID inválido.", corpo.GetProperty("message").GetString());
            Assert.False(corpo.TryGetProperty("errors", out _));
        }

        [Fact]
        public async Task SemErro_NaoAlteraAResposta()
        {
            var middleware = new ExceptionHandlingMiddleware(ctx => { ctx.Response.StatusCode = 204; return Task.CompletedTask; }, NullLogger<ExceptionHandlingMiddleware>.Instance);
            var context = new DefaultHttpContext();

            await middleware.InvokeAsync(context);

            Assert.Equal(204, context.Response.StatusCode);
        }
    }
}
