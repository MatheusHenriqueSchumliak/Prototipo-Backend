using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PrototipoBackEnd.Domain.Enumerables;
using Xunit;

namespace PrototipoBackEnd.Tests.Integration
{
    public class ErrosEStatusCodesTests : IClassFixture<ApiFactory>
    {
        private readonly ApiFactory _factory;

        public ErrosEStatusCodesTests(ApiFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("GET", "/Usuario/BuscarPorId/nao-e-guid")]
        [InlineData("GET", "/Artesao/BuscarPorId/nao-e-guid")]
        [InlineData("GET", "/Artesanato/BuscarPorId/nao-e-guid")]
        [InlineData("DELETE", "/Usuario/Excluir/nao-e-guid")]
        [InlineData("DELETE", "/Artesao/Excluir/nao-e-guid")]
        [InlineData("DELETE", "/Artesanato/Excluir/nao-e-guid")]
        public async Task IdInvalido_Retorna400ComEnvelopeDeErro(string metodo, string url)
        {
            var client = _factory.CriarClienteComRole(UsuarioEnum.Administrador);

            var response = await client.SendAsync(new HttpRequestMessage(new HttpMethod(metodo), url));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            await AssertEnvelopeDeErro(response, "ID inválido.");
        }

        [Theory]
        [InlineData("GET", "/Usuario/BuscarPorId/")]
        [InlineData("GET", "/Artesao/BuscarPorId/")]
        [InlineData("GET", "/Artesanato/BuscarPorId/")]
        [InlineData("DELETE", "/Usuario/Excluir/")]
        [InlineData("DELETE", "/Artesao/Excluir/")]
        [InlineData("DELETE", "/Artesanato/Excluir/")]
        public async Task IdInexistente_Retorna404ComEnvelopeDeErro(string metodo, string urlBase)
        {
            var client = _factory.CriarClienteComRole(UsuarioEnum.Administrador);

            var response = await client.SendAsync(new HttpRequestMessage(new HttpMethod(metodo), urlBase + Guid.NewGuid()));

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            await AssertEnvelopeDeErro(response);
        }

        [Fact]
        public async Task PutUsuario_Inexistente_Retorna404()
        {
            var client = _factory.CriarClienteComRole(UsuarioEnum.Administrador);

            var response = await client.PutAsJsonAsync($"/Usuario/Atualizar/{Guid.NewGuid()}",
                new { email = "x@teste.com", role = "Usuario" });

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PostUsuario_Criado_Retorna201ComLocationApontandoParaOGetPorId()
        {
            var client = _factory.CriarClienteComRole(UsuarioEnum.Administrador);

            var response = await client.PostAsJsonAsync("/Usuario/Adicionar", new
            {
                pessoaId = _factory.NovaPessoaId(),
                email = $"loc-{Guid.NewGuid():N}@teste.com",
                senha = "SenhaForte123",
                role = "Usuario"
            });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);
            Assert.Contains("/Usuario/BuscarPorId/", response.Headers.Location!.ToString());
        }

        [Fact]
        public async Task PostUsuario_PessoaIdInvalido_Retorna400_ENaoTrocaPorGuidAleatorio()
        {
            var client = _factory.CriarClienteComRole(UsuarioEnum.Administrador);

            var response = await client.PostAsJsonAsync("/Usuario/Adicionar", new
            {
                pessoaId = "invalido",
                email = $"pid-{Guid.NewGuid():N}@teste.com",
                senha = "SenhaForte123",
                role = "Usuario"
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            await AssertEnvelopeDeErro(response, "PessoaId inválido.");
        }

        [Fact]
        public async Task Erro_NaoVazaStackTrace()
        {
            var client = _factory.CriarClienteComRole(UsuarioEnum.Administrador);

            var response = await client.GetAsync("/Usuario/BuscarPorId/x");
            var corpo = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain("   at ", corpo);
            Assert.DoesNotContain("Exception", corpo);
        }

        private static async Task AssertEnvelopeDeErro(HttpResponseMessage response, string? mensagemEsperada = null)
        {
            Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var message = json.RootElement.GetProperty("message").GetString();
            Assert.False(string.IsNullOrWhiteSpace(message));
            Assert.False(json.RootElement.TryGetProperty("data", out _));
            Assert.False(json.RootElement.TryGetProperty("title", out _)); // não é ProblemDetails

            if (mensagemEsperada != null)
                Assert.Equal(mensagemEsperada, message);
        }
    }
}
