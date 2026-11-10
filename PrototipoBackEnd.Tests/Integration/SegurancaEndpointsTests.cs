using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PrototipoBackEnd.Domain.Entities;
using PrototipoBackEnd.Domain.Enumerables;
using Xunit;

namespace PrototipoBackEnd.Tests.Integration
{
    public class SegurancaEndpointsTests : IClassFixture<ApiFactory>
    {
        private const string GuidQualquer = "11111111-1111-1111-1111-111111111111";
        private readonly ApiFactory _factory;

        public SegurancaEndpointsTests(ApiFactory factory)
        {
            _factory = factory;
        }

        private object NovoUsuario(string email, string role = "Usuario") => new
        {
            pessoaId = _factory.NovaPessoaId(),
            email,
            senha = "SenhaForte123",
            role
        };

        private static HttpContent CorpoJson(object corpo) => JsonContent.Create(corpo);

        private static HttpContent CorpoMultipart() => new MultipartFormDataContent();

        public static IEnumerable<object[]> EscritasProtegidas()
        {
            // método, url, multipart?
            yield return new object[] { "POST", "/Usuario/Adicionar", false };
            yield return new object[] { "PUT", $"/Usuario/Atualizar/{GuidQualquer}", false };
            yield return new object[] { "DELETE", $"/Usuario/Excluir/{GuidQualquer}", false };
            yield return new object[] { "POST", "/Artesao/Adicionar", true };
            yield return new object[] { "PUT", $"/Artesao/Atualizar/{GuidQualquer}", true };
            yield return new object[] { "DELETE", $"/Artesao/Excluir/{GuidQualquer}", false };
            yield return new object[] { "POST", "/Artesanato/Adicionar", true };
            yield return new object[] { "PUT", $"/Artesanato/Atualizar/{GuidQualquer}", true };
            yield return new object[] { "DELETE", $"/Artesanato/Excluir/{GuidQualquer}", false };
        }

        // DELETE de Artesão/Artesanato inexistente ainda propaga exceção no controller (será tratado na Etapa 2).
        public static IEnumerable<object[]> EscritasParaAdmin()
            => EscritasProtegidas().Where(e => !((string)e[0] == "DELETE" && ((string)e[1]).StartsWith("/Artesa")));

        private static HttpRequestMessage Requisicao(string metodo, string url, bool multipart)
        {
            var request = new HttpRequestMessage(new HttpMethod(metodo), url);
            if (metodo != "DELETE")
                request.Content = multipart ? CorpoMultipart() : CorpoJson(new { });
            return request;
        }

        [Theory]
        [MemberData(nameof(EscritasProtegidas))]
        public async Task Escrita_Anonimo_Retorna401(string metodo, string url, bool multipart)
        {
            var response = await _factory.CreateClient().SendAsync(Requisicao(metodo, url, multipart));

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(EscritasProtegidas))]
        public async Task Escrita_UsuarioComum_Retorna403(string metodo, string url, bool multipart)
        {
            var client = _factory.CriarClienteComRole(UsuarioEnum.Usuario);

            var response = await client.SendAsync(Requisicao(metodo, url, multipart));

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(EscritasParaAdmin))]
        public async Task Escrita_Admin_NaoERecusadaPorAutorizacao(string metodo, string url, bool multipart)
        {
            var client = _factory.CriarClienteComRole(UsuarioEnum.Administrador);

            var response = await client.SendAsync(Requisicao(metodo, url, multipart));

            Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData("/Artesao/BuscarTodos")]
        [InlineData("/Artesanato/BuscarTodos")]
        public async Task LeituraPublica_Anonimo_Retorna200(string url)
        {
            var response = await _factory.CreateClient().GetAsync(url);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("/Usuario/BuscarTodos")]
        [InlineData("/Usuario/BuscarPorId/" + GuidQualquer)]
        public async Task LeituraDeUsuario_Anonimo_Retorna401(string url)
        {
            var response = await _factory.CreateClient().GetAsync(url);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData("/Usuario/BuscarTodos")]
        [InlineData("/Usuario/BuscarPorId/" + GuidQualquer)]
        public async Task LeituraDeUsuario_UsuarioComum_Retorna403(string url)
        {
            var response = await _factory.CriarClienteComRole(UsuarioEnum.Usuario).GetAsync(url);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task PostUsuario_Admin_Cria_RespeitaRoleDoRequest_ENaoVazaSenhaNemHash()
        {
            var client = _factory.CriarClienteComRole(UsuarioEnum.Administrador);
            var email = $"Novo-{Guid.NewGuid():N}@Teste.com";

            var response = await client.PostAsync("/Usuario/Adicionar", CorpoJson(NovoUsuario(email, "Usuario")));
            var corpo = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.DoesNotContain("senha", corpo, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("$2", corpo);

            using var json = JsonDocument.Parse(corpo);
            var data = json.RootElement.GetProperty("data");
            Assert.Equal("Usuario", data.GetProperty("role").GetString());
            Assert.Equal(email.ToLowerInvariant(), data.GetProperty("email").GetString());
        }

        [Fact]
        public async Task ListagemDeUsuarios_Admin_NaoContemSenhaNemHash()
        {
            var client = _factory.CriarClienteComRole(UsuarioEnum.Administrador);
            await client.PostAsync("/Usuario/Adicionar", CorpoJson(NovoUsuario($"lista-{Guid.NewGuid():N}@teste.com", "Administrador")));

            var response = await client.GetAsync("/Usuario/BuscarTodos");
            var corpo = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"Administrador\"", corpo);
            Assert.DoesNotContain("senha", corpo, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("$2", corpo);
        }

        [Fact]
        public async Task PostUsuario_EmailDuplicado_Retorna409()
        {
            var client = _factory.CriarClienteComRole(UsuarioEnum.Administrador);
            var email = $"dup-{Guid.NewGuid():N}@teste.com";

            var primeiro = await client.PostAsync("/Usuario/Adicionar", CorpoJson(NovoUsuario(email)));
            var segundo = await client.PostAsync("/Usuario/Adicionar", CorpoJson(NovoUsuario(email.ToUpperInvariant())));

            Assert.Equal(HttpStatusCode.Created, primeiro.StatusCode);
            Assert.Equal(HttpStatusCode.Conflict, segundo.StatusCode);
        }

        [Fact]
        public async Task PostUsuario_RoleInvalido_Retorna400ComEnvelopeDeErro()
        {
            var client = _factory.CriarClienteComRole(UsuarioEnum.Administrador);

            var response = await client.PostAsync("/Usuario/Adicionar", CorpoJson(NovoUsuario($"r-{Guid.NewGuid():N}@teste.com", "SuperAdmin")));
            var corpo = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            using var json = JsonDocument.Parse(corpo);
            Assert.True(json.RootElement.TryGetProperty("message", out _));
            Assert.True(json.RootElement.TryGetProperty("errors", out _));
        }

        [Fact]
        public async Task PutUsuario_Admin_AtualizaEmailESemSenhaMantemLogin()
        {
            var client = _factory.CriarClienteComRole(UsuarioEnum.Administrador);
            var email = $"put-{Guid.NewGuid():N}@teste.com";
            var criado = await client.PostAsync("/Usuario/Adicionar", CorpoJson(NovoUsuario(email)));
            using var json = JsonDocument.Parse(await criado.Content.ReadAsStringAsync());
            var id = json.RootElement.GetProperty("data").GetProperty("id").GetString();
            var novoEmail = $"novo-{Guid.NewGuid():N}@teste.com";

            var response = await client.PutAsync($"/Usuario/Atualizar/{id}", CorpoJson(new { email = novoEmail, role = "Usuario" }));
            var login = await _factory.CreateClient().PostAsJsonAsync("/Auth/login", new { email = novoEmail, senha = "SenhaForte123" });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        }

        [Fact]
        public async Task Login_SemEmailOuSenha_Retorna400()
        {
            var client = _factory.CreateClient();

            var semCampos = await client.PostAsJsonAsync("/Auth/login", new { });
            var semSenha = await client.PostAsJsonAsync("/Auth/login", new { email = "a@b.com" });

            Assert.Equal(HttpStatusCode.BadRequest, semCampos.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, semSenha.StatusCode);
        }

        [Fact]
        public async Task Login_CredenciaisInvalidas_Retorna401ComMensagemGenerica()
        {
            var client = _factory.CriarClienteComRole(UsuarioEnum.Administrador);
            var email = $"login-{Guid.NewGuid():N}@teste.com";
            await client.PostAsync("/Usuario/Adicionar", CorpoJson(NovoUsuario(email)));
            var anonimo = _factory.CreateClient();

            var emailInexistente = await anonimo.PostAsJsonAsync("/Auth/login", new { email = "nao-existe@teste.com", senha = "qualquer" });
            var senhaErrada = await anonimo.PostAsJsonAsync("/Auth/login", new { email, senha = "errada" });

            Assert.Equal(HttpStatusCode.Unauthorized, emailInexistente.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, senhaErrada.StatusCode);
            Assert.Equal(await emailInexistente.Content.ReadAsStringAsync(), await senhaErrada.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Login_Valido_RetornaTokenSemCookieDeRefresh()
        {
            var admin = _factory.CriarClienteComRole(UsuarioEnum.Administrador);
            var email = $"ok-{Guid.NewGuid():N}@teste.com";
            await admin.PostAsync("/Usuario/Adicionar", CorpoJson(NovoUsuario(email, "Administrador")));

            var response = await _factory.CreateClient().PostAsJsonAsync("/Auth/login", new { email, senha = "SenhaForte123" });
            var corpo = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(response.Headers.Contains("Set-Cookie"));
            using var json = JsonDocument.Parse(corpo);
            var data = json.RootElement.GetProperty("data");
            Assert.False(string.IsNullOrWhiteSpace(data.GetProperty("token").GetString()));
            Assert.Equal("Administrador", data.GetProperty("role").GetString());

            // O token emitido no login deve dar acesso de admin
            var autenticado = _factory.CreateClient();
            autenticado.DefaultRequestHeaders.Authorization = new("Bearer", data.GetProperty("token").GetString());
            Assert.Equal(HttpStatusCode.OK, (await autenticado.GetAsync("/Usuario/BuscarTodos")).StatusCode);
        }
    }
}

