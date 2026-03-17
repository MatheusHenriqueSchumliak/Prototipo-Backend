using FluentAssertions;
using PrototipoBackEnd.Domain.Entities;
using PrototipoBackEnd.Domain.Enumerables;
using Xunit;

namespace PrototipoBackEnd.UnitTests.Domain;

public class ArtesaoTest
{
	// Valores válidos padrão usados nos testes
	private readonly string ValidUsuarioId = Guid.NewGuid().ToString();
	private const string ValidNomeArtesao = "Ateliê Exemplo";
	private const int ValidIdade = 30;
	private const string ValidNomeCompleto = "Maria Exemplo";
	private const string ValidTelefone = "49 99999-9999";
	private const string ValidWhatsapp = "(49) 99999-9999";
	private const string ValidEmail = "maria@example.com";
	private const string ValidInstagram = "maria_inst";
	private const string ValidFacebook = "maria.fb";
	private const string ValidDescricao = "Peças artesanais em madeira";
	private const bool ValidReceberEncomendas = true;
	private const bool ValidEnviaEncomendas = false;
	private const string ValidFotoUrl = "https://exemplo/foto.jpg";
	private const string ValidNichoAtuacao = "Marcenaria";
	private const bool ValidLocalFisico = true;
	private const bool ValidFeiraMunicipal = false;
	private const string ValidCEP = "12345-678";
	private const string ValidEstado = "PR";
	private const string ValidCidade = "Pato Branco";
	private const string ValidRua = "Rua das Flores";
	private const string ValidBairro = "Centro";
	private const string ValidComplemento = "Apto 1";
	private const string ValidNumero = "100";
	private const bool ValidSemNumero = false;

	// Sentinel usado para diferenciar "usar valor padrão" de "o teste passou explicitamente null"
	private const string UseDefault = "__USE_DEFAULT__";

	private Artesao CreateDefault(
		string? usuarioId = UseDefault,
		string? nomeArtesao = UseDefault,
		int? idade = null,
		string? nomeCompleto = UseDefault,
		string? telefone = UseDefault,
		string? whatsapp = UseDefault,
		string? email = UseDefault,
		string? instagram = UseDefault,
		string? facebook = UseDefault,
		string? descricao = UseDefault,
		bool? receberEncomendas = null,
		bool? enviaEncomendas = null,
		string? fotoUrl = UseDefault,
		string? nichoAtuacao = UseDefault,
		bool? localFisico = null,
		bool? feiraMunicipal = null,
		string? cep = UseDefault,
		string? estado = UseDefault,
		string? cidade = UseDefault,
		string? rua = UseDefault,
		string? bairro = UseDefault,
		string? complemento = UseDefault,
		string? numero = UseDefault,
		bool? semNumero = null)
	{
		// Mapeia sentinela para valores válidos; deixa explicitamente nulls como estão
		var usuarioIdArg = usuarioId == UseDefault ? ValidUsuarioId : usuarioId;
		var nomeArtesaoArg = nomeArtesao == UseDefault ? ValidNomeArtesao : nomeArtesao;
		var idadeArg = idade ?? ValidIdade;
		var nomeCompletoArg = nomeCompleto == UseDefault ? ValidNomeCompleto : nomeCompleto;
		var telefoneArg = telefone == UseDefault ? ValidTelefone : telefone;
		var whatsappArg = whatsapp == UseDefault ? ValidWhatsapp : whatsapp;
		var emailArg = email == UseDefault ? ValidEmail : email;
		var instagramArg = instagram == UseDefault ? ValidInstagram : instagram;
		var facebookArg = facebook == UseDefault ? ValidFacebook : facebook;
		var descricaoArg = descricao == UseDefault ? ValidDescricao : descricao;
		var receberEncomendasArg = receberEncomendas ?? ValidReceberEncomendas;
		var enviaEncomendasArg = enviaEncomendas ?? ValidEnviaEncomendas;
		var fotoUrlArg = fotoUrl == UseDefault ? ValidFotoUrl : fotoUrl;
		var nichoAtuacaoArg = nichoAtuacao == UseDefault ? ValidNichoAtuacao : nichoAtuacao;
		var localFisicoArg = localFisico ?? ValidLocalFisico;
		var feiraMunicipalArg = feiraMunicipal ?? ValidFeiraMunicipal;
		var cepArg = cep == UseDefault ? ValidCEP : cep;
		var estadoArg = estado == UseDefault ? ValidEstado : estado;
		var cidadeArg = cidade == UseDefault ? ValidCidade : cidade;
		var ruaArg = rua == UseDefault ? ValidRua : rua;
		var bairroArg = bairro == UseDefault ? ValidBairro : bairro;
		var complementoArg = complemento == UseDefault ? ValidComplemento : complemento;
		var numeroArg = numero == UseDefault ? ValidNumero : numero;
		var semNumeroArg = semNumero ?? ValidSemNumero;

		return Artesao.Criar(
			usuarioIdArg!,
			nomeArtesaoArg!,
			idadeArg,
			nomeCompletoArg!,
			telefoneArg!,
			whatsappArg!,
			emailArg!,
			instagramArg!,
			facebookArg!,
			descricaoArg!,
			receberEncomendasArg,
			enviaEncomendasArg,
			fotoUrlArg!,
			nichoAtuacaoArg!,
			localFisicoArg,
			feiraMunicipalArg,
			cepArg!,
			estadoArg!,
			cidadeArg!,
			ruaArg!,
			bairroArg!,
			complementoArg!,
			numeroArg!,
			semNumeroArg);
	}

	#region Validações / Sucesso

	[Fact(DisplayName = "Criar Artesão válido deve popular todas as propriedades")]
	public void CriarArtesao_Valido_DeveCriarComSucesso()
	{
		// Act
		var artesao = CreateDefault();

		// Assert
		artesao.Should().NotBeNull();
		artesao.Id.Should().NotBeNullOrWhiteSpace();
		artesao.UsuarioId.Should().Be(ValidUsuarioId);
		artesao.NomeArtesao.Should().Be(ValidNomeArtesao);
		artesao.NomeCompleto.Should().Be(ValidNomeCompleto);
		artesao.Idade.Should().Be(ValidIdade);
		// telefones são normalizados para dígitos
		artesao.Telefone.Should().Be(new string(ValidTelefone.Where(char.IsDigit).ToArray()));
		artesao.WhatsApp.Should().Be(new string(ValidWhatsapp.Where(char.IsDigit).ToArray()));
		artesao.Email.Should().Be(ValidEmail);
		artesao.Instagram.Should().Be(ValidInstagram);
		artesao.Facebook.Should().Be(ValidFacebook);
		artesao.DescricaoPerfil.Should().Be(ValidDescricao);
		artesao.ReceberEncomendas.Should().BeTrue();
		artesao.EnviaEncomendas.Should().BeFalse();
		artesao.FotoUrl.Should().Be(ValidFotoUrl);
		artesao.NichoAtuacao.Should().Be(ValidNichoAtuacao);
		artesao.LocalFisico.Should().BeTrue();
		artesao.FeiraMunicipal.Should().BeFalse();
		// CEP é normalizado para dígitos
		artesao.CEP.Should().Be(new string(ValidCEP.Where(char.IsDigit).ToArray()));
		artesao.Estado.Should().Be(ValidEstado);
		artesao.Cidade.Should().Be(ValidCidade);
		artesao.Rua.Should().Be(ValidRua);
		artesao.Bairro.Should().Be(ValidBairro);
		artesao.Complemento.Should().Be(ValidComplemento);
		artesao.Numero.Should().Be(ValidNumero);
		artesao.SemNumero.Should().BeFalse();

		artesao.ArtesanatoIds.Should().NotBeNull();
		artesao.ArtesanatoIds.Should().BeEmpty();
		artesao.Artesanatos.Should().BeNull();
	}

	[Fact(DisplayName = "Criar dois Artesãos gera IDs únicos")]
	public void CriarArtesao_DuasChamadas_GeramIdsDiferentes()
	{
		var a1 = CreateDefault(nomeArtesao: "A1", nomeCompleto: "Nome 1", numero: "1");
		var a2 = CreateDefault(nomeArtesao: "A2", nomeCompleto: "Nome 2", numero: "2");

		a1.Id.Should().NotBeNullOrWhiteSpace();
		a2.Id.Should().NotBeNullOrWhiteSpace();
		a1.Id.Should().NotBe(a2.Id);
	}

	#endregion

	#region Validações / Falhas

	[Theory(DisplayName = "NomeCompleto inválido lança ArgumentException")]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void CriarArtesao_NomeCompletoInvalido_DeveLancar(string nomeInvalido)
	{
		Action act = () => CreateDefault(nomeCompleto: nomeInvalido);
		act.Should().Throw<ArgumentException>().WithMessage("*NomeCompleto*");
	}

	[Theory(DisplayName = "NomeArtesao inválido lança ArgumentException")]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void CriarArtesao_NomeArtesaoInvalido_DeveLancar(string nomeArtesaoInvalido)
	{
		Action act = () => CreateDefault(nomeArtesao: nomeArtesaoInvalido);
		act.Should().Throw<ArgumentException>().WithMessage("*NomeArtesao*");
	}

	[Theory(DisplayName = "Idade inválida lança ArgumentOutOfRangeException")]
	[InlineData(-1)]
	[InlineData(121)]
	public void CriarArtesao_IdadeInvalida_DeveLancar(int idadeInvalida)
	{
		Action act = () => CreateDefault(idade: idadeInvalida);
		act.Should().Throw<ArgumentOutOfRangeException>();
	}

	[Theory(DisplayName = "Telefone/WhatsApp inválido lança ArgumentException")]
	[InlineData("", "49999999999")]
	[InlineData("abc", "49999999999")]
	[InlineData("49999999999", "")]
	[InlineData("49999999999", "abc")]
	public void CriarArtesao_TelefoneInvalido_DeveLancar(string tel, string wpp)
	{
		Action act = () => CreateDefault(telefone: tel, whatsapp: wpp);
		act.Should().Throw<ArgumentException>()
			.Where(ex => ex.Message.Contains("Telefone") || ex.Message.Contains("WhatsApp"));
	}

	[Theory(DisplayName = "Email inválido lança ArgumentException")]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("invalid-email")]
	public void CriarArtesao_EmailInvalido_DeveLancar(string emailInvalido)
	{
		Action act = () => CreateDefault(email: emailInvalido);
		act.Should().Throw<ArgumentException>().WithMessage("*Email*");
	}

	[Theory(DisplayName = "FotoUrl inválida lança ArgumentException")]
	[InlineData("not-a-url")]
	[InlineData("ftp://example.com/file")]
	public void CriarArtesao_FotoUrlInvalida_DeveLancar(string urlInvalida)
	{
		Action act = () => CreateDefault(fotoUrl: urlInvalida);
		act.Should().Throw<ArgumentException>().WithMessage("*FotoUrl*");
	}

	[Theory(DisplayName = "NichoAtuacao inválido lança ArgumentException")]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void CriarArtesao_NichoInvalido_DeveLancar(string nichoInvalido)
	{
		Action act = () => CreateDefault(nichoAtuacao: nichoInvalido);
		act.Should().Throw<ArgumentException>().WithMessage("*NichoAtuacao*");
	}

	[Theory(DisplayName = "CEP inválido lança ArgumentException")]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("123")]
	[InlineData("1234-56")]
	public void CriarArtesao_CEPInvalido_DeveLancar(string cepInvalido)
	{
		Action act = () => CreateDefault(cep: cepInvalido);
		act.Should().Throw<ArgumentException>().WithMessage("*CEP*");
	}

	[Theory(DisplayName = "Estado inválido lança ArgumentException")]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("P")]
	[InlineData("PRB")]
	public void CriarArtesao_EstadoInvalido_DeveLancar(string estadoInvalido)
	{
		Action act = () => CreateDefault(estado: estadoInvalido);
		act.Should().Throw<ArgumentException>().WithMessage("*Estado*");
	}

	[Theory(DisplayName = "Cidade/Rua/Bairro inválidos lançam ArgumentException")]
	[InlineData(null, "Rua", "Bairro")]
	[InlineData("Cidade", null, "Bairro")]
	[InlineData("Cidade", "Rua", null)]
	public void CriarArtesao_EnderecoInvalido_DeveLancar(string cidade, string rua, string bairro)
	{
		Action act = () => CreateDefault(cidade: cidade, rua: rua, bairro: bairro);
		act.Should().Throw<ArgumentException>();
	}

	#endregion
}