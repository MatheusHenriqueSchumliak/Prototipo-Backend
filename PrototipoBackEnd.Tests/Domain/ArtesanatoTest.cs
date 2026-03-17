using PrototipoBackEnd.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace PrototipoBackEnd.UnitTests.Domain;

public class ArtesanatoTest
{
	private static string NewId() => Guid.NewGuid().ToString();
	private static string NewUserId() => Guid.NewGuid().ToString();

	#region Validação / sucesso

	[Fact(DisplayName = "Criar Artesanato válido deve retornar objeto com valores normalizados")]
	public void CriarArtesanato_Valido_DeveCriar()
	{
		var id = NewId();
		var usuarioId = NewUserId();
		var artesaoId = NewId();
		var imagens = new[] { " https://ex.com/1.jpg ", "https://ex.com/2.jpg" };
		var tags = new[] { " madeira ", "Decoração", "madeira" }; // duplicata com case diferente
		var titulo = "  Peça X  ";
		var preco = 99.90m;
		var quantidade = 2;
		var descricao = "Desc";
		var materiais = "Madeira";
		var data = DateTime.UtcNow.AddHours(-1);
		var tempo = 10;

		var artesao = new Artesao { Id = artesaoId, UsuarioId = usuarioId };

		var a = Artesanato.Criar(
			id: id,
			usuarioId: usuarioId,
			artesaoId: artesaoId,
			imagemUrl: imagens,
			sobEncomenda: true,
			aceitaEncomenda: false,
			categoriaTags: tags,
			tituloArtesanato: titulo,
			preco: preco,
			quantidadeArtesanato: quantidade,
			descricaoArtesanato: descricao,
			materiaisUtilizados: materiais,
			dataCriacao: data,
			tempoCriacaoHr: tempo,
			artesao: artesao);

		a.Should().NotBeNull();
		a.Id.Should().Be(id);
		a.UsuarioId.Should().Be(usuarioId);
		a.ArtesaoId.Should().Be(artesaoId);
		a.ImagemUrl.Should().BeEquivalentTo(new[] { "https://ex.com/1.jpg", "https://ex.com/2.jpg" });
		a.SobEncomenda.Should().BeTrue();
		a.AceitaEncomenda.Should().BeFalse();
		// tags trimmed, distinct (case-insensitive) and order does not matter
		a.CategoriaTags.Should().BeEquivalentTo(new[] { "madeira", "Decoração" }, options => options.ComparingByMembers<string>());
		a.TituloArtesanato.Should().Be("Peça X");
		a.Preco.Should().Be(preco);
		a.QuantidadeArtesanato.Should().Be(quantidade);
		a.DescricaoArtesanato.Should().Be(descricao);
		a.MateriaisUtilizados.Should().Be(materiais);
		a.DataCriacao.Should().Be(data);
		a.TempoCriacaoHr.Should().Be(tempo);
		a.Artesao.Should().BeSameAs(artesao);
	}

	#endregion Validação / sucesso

	#region Validações / Falhas

	[Theory(DisplayName = "Id/UsuarioId inválidos lançam ArgumentException")]
	[InlineData(null, "u")]
	[InlineData("", "u")]
	[InlineData(" ", "u")]
	[InlineData("i", null)]
	[InlineData("i", "")]
	[InlineData("i", " ")]
	public void CriarArtesanato_IdOuUsuarioIdInvalido_DeveLancar(string id, string usuarioId)
	{
		Action act = () => Artesanato.Criar(
			id: id!,
			usuarioId: usuarioId!,
			artesaoId: null,
			imagemUrl: null,
			sobEncomenda: null,
			aceitaEncomenda: null,
			categoriaTags: null,
			tituloArtesanato: null,
			preco: null,
			quantidadeArtesanato: 0,
			descricaoArtesanato: null,
			materiaisUtilizados: null,
			dataCriacao: null,
			tempoCriacaoHr: null);

		act.Should().Throw<ArgumentException>().Where(e => e.Message.Contains("Id") || e.Message.Contains("UsuarioId"));
	}

	[Fact(DisplayName = "Imagem inválida lança ArgumentException")]
	public void CriarArtesanato_ImagemInvalida_DeveLancar()
	{
		var imagens = new[] { "not-a-url" };
		Action act = () => Artesanato.Criar(
			id: NewId(),
			usuarioId: NewUserId(),
			artesaoId: null,
			imagemUrl: imagens,
			sobEncomenda: null,
			aceitaEncomenda: null,
			categoriaTags: null,
			tituloArtesanato: null,
			preco: null,
			quantidadeArtesanato: 0,
			descricaoArtesanato: null,
			materiaisUtilizados: null,
			dataCriacao: null,
			tempoCriacaoHr: null);

		act.Should().Throw<ArgumentException>().WithMessage("*ImagemUrl*");
	}

	[Fact(DisplayName = "Mais de 20 imagens lança ArgumentException")]
	public void CriarArtesanato_MaisDe20Imagens_DeveLancar()
	{
		var imagens = Enumerable.Range(1, 21).Select(i => $"https://ex/{i}.jpg");
		Action act = () => Artesanato.Criar(
			id: NewId(),
			usuarioId: NewUserId(),
			artesaoId: null,
			imagemUrl: imagens,
			sobEncomenda: null,
			aceitaEncomenda: null,
			categoriaTags: null,
			tituloArtesanato: null,
			preco: null,
			quantidadeArtesanato: 0,
			descricaoArtesanato: null,
			materiaisUtilizados: null,
			dataCriacao: null,
			tempoCriacaoHr: null);

		act.Should().Throw<ArgumentException>().WithMessage("*ImagemUrl*");
	}

	[Fact(DisplayName = "CategoriaTags inválidas lançam ArgumentException")]
	public void CriarArtesanato_CategoriaTagsInvalidas_DeveLancar()
	{
		// tag longa
		var tags = new[] { new string('a', 51) };
		Action act = () => Artesanato.Criar(
			id: NewId(),
			usuarioId: NewUserId(),
			artesaoId: null,
			imagemUrl: null,
			sobEncomenda: null,
			aceitaEncomenda: null,
			categoriaTags: tags,
			tituloArtesanato: null,
			preco: null,
			quantidadeArtesanato: 0,
			descricaoArtesanato: null,
			materiaisUtilizados: null,
			dataCriacao: null,
			tempoCriacaoHr: null);

		act.Should().Throw<ArgumentException>().WithMessage("*CategoriaTags*");
	}

	[Fact(DisplayName = "Mais de 50 tags lança ArgumentException")]
	public void CriarArtesanato_MaisDe50Tags_DeveLancar()
	{
		var tags = Enumerable.Range(1, 51).Select(i => $"t{i}");
		Action act = () => Artesanato.Criar(
			id: NewId(),
			usuarioId: NewUserId(),
			artesaoId: null,
			imagemUrl: null,
			sobEncomenda: null,
			aceitaEncomenda: null,
			categoriaTags: tags,
			tituloArtesanato: null,
			preco: null,
			quantidadeArtesanato: 0,
			descricaoArtesanato: null,
			materiaisUtilizados: null,
			dataCriacao: null,
			tempoCriacaoHr: null);

		act.Should().Throw<ArgumentException>().WithMessage("*CategoriaTags*");
	}

	[Fact(DisplayName = "Título muito longo lança ArgumentException")]
	public void CriarArtesanato_TituloLongo_DeveLancar()
	{
		var titulo = new string('x', 201);
		Action act = () => Artesanato.Criar(
			id: NewId(),
			usuarioId: NewUserId(),
			artesaoId: null,
			imagemUrl: null,
			sobEncomenda: null,
			aceitaEncomenda: null,
			categoriaTags: null,
			tituloArtesanato: titulo,
			preco: null,
			quantidadeArtesanato: 0,
			descricaoArtesanato: null,
			materiaisUtilizados: null,
			dataCriacao: null,
			tempoCriacaoHr: null);

		act.Should().Throw<ArgumentException>().WithMessage("*TituloArtesanato*");
	}

	[Fact(DisplayName = "Preco negativo lança ArgumentOutOfRangeException")]
	public void CriarArtesanato_PrecoNegativo_DeveLancar()
	{
		Action act = () => Artesanato.Criar(
			id: NewId(),
			usuarioId: NewUserId(),
			artesaoId: null,
			imagemUrl: null,
			sobEncomenda: null,
			aceitaEncomenda: null,
			categoriaTags: null,
			tituloArtesanato: null,
			preco: -1m,
			quantidadeArtesanato: 0,
			descricaoArtesanato: null,
			materiaisUtilizados: null,
			dataCriacao: null,
			tempoCriacaoHr: null);

		act.Should().Throw<ArgumentOutOfRangeException>();
	}

	[Fact(DisplayName = "Quantidade negativa lança ArgumentOutOfRangeException")]
	public void CriarArtesanato_QuantidadeNegativa_DeveLancar()
	{
		Action act = () => Artesanato.Criar(
			id: NewId(),
			usuarioId: NewUserId(),
			artesaoId: null,
			imagemUrl: null,
			sobEncomenda: null,
			aceitaEncomenda: null,
			categoriaTags: null,
			tituloArtesanato: null,
			preco: null,
			quantidadeArtesanato: -1,
			descricaoArtesanato: null,
			materiaisUtilizados: null,
			dataCriacao: null,
			tempoCriacaoHr: null);

		act.Should().Throw<ArgumentOutOfRangeException>();
	}

	[Fact(DisplayName = "Descricao muito longa lança ArgumentException")]
	public void CriarArtesanato_DescricaoLonga_DeveLancar()
	{
		var descricao = new string('d', 2001);
		Action act = () => Artesanato.Criar(
			id: NewId(),
			usuarioId: NewUserId(),
			artesaoId: null,
			imagemUrl: null,
			sobEncomenda: null,
			aceitaEncomenda: null,
			categoriaTags: null,
			tituloArtesanato: null,
			preco: null,
			quantidadeArtesanato: 0,
			descricaoArtesanato: descricao,
			materiaisUtilizados: null,
			dataCriacao: null,
			tempoCriacaoHr: null);

		act.Should().Throw<ArgumentException>().WithMessage("*DescricaoArtesanato*");
	}

	[Fact(DisplayName = "Materiais muito longos lançam ArgumentException")]
	public void CriarArtesanato_MateriaisLongos_DeveLancar()
	{
		var mat = new string('m', 1001);
		Action act = () => Artesanato.Criar(
			id: NewId(),
			usuarioId: NewUserId(),
			artesaoId: null,
			imagemUrl: null,
			sobEncomenda: null,
			aceitaEncomenda: null,
			categoriaTags: null,
			tituloArtesanato: null,
			preco: null,
			quantidadeArtesanato: 0,
			descricaoArtesanato: null,
			materiaisUtilizados: mat,
			dataCriacao: null,
			tempoCriacaoHr: null);

		act.Should().Throw<ArgumentException>().WithMessage("*MateriaisUtilizados*");
	}

	[Fact(DisplayName = "Data no futuro lança ArgumentException")]
	public void CriarArtesanato_DataFutura_DeveLancar()
	{
		var futura = DateTime.UtcNow.AddHours(2);
		Action act = () => Artesanato.Criar(
			id: NewId(),
			usuarioId: NewUserId(),
			artesaoId: null,
			imagemUrl: null,
			sobEncomenda: null,
			aceitaEncomenda: null,
			categoriaTags: null,
			tituloArtesanato: null,
			preco: null,
			quantidadeArtesanato: 0,
			descricaoArtesanato: null,
			materiaisUtilizados: null,
			dataCriacao: futura,
			tempoCriacaoHr: null);

		act.Should().Throw<ArgumentException>().WithMessage("*DataCriacao*");
	}

	[Theory(DisplayName = "Tempo de criação inválido lança ArgumentOutOfRangeException")]
	[InlineData(-1)]
	[InlineData(10001)]
	public void CriarArtesanato_TempoInvalido_DeveLancar(int tempo)
	{
		Action act = () => Artesanato.Criar(
			id: NewId(),
			usuarioId: NewUserId(),
			artesaoId: null,
			imagemUrl: null,
			sobEncomenda: null,
			aceitaEncomenda: null,
			categoriaTags: null,
			tituloArtesanato: null,
			preco: null,
			quantidadeArtesanato: 0,
			descricaoArtesanato: null,
			materiaisUtilizados: null,
			dataCriacao: null,
			tempoCriacaoHr: tempo);

		act.Should().Throw<ArgumentOutOfRangeException>();
	}

	[Fact(DisplayName = "Divergência entre ArtesaoId e Artesao.Id lança ArgumentException")]
	public void CriarArtesanato_ArtesaoIdDiverge_DeveLancar()
	{
		var artesao = new Artesao { Id = NewId(), UsuarioId = NewUserId() };
		var outroId = NewId();
		Action act = () => Artesanato.Criar(
			id: NewId(),
			usuarioId: NewUserId(),
			artesaoId: outroId,
			imagemUrl: null,
			sobEncomenda: null,
			aceitaEncomenda: null,
			categoriaTags: null,
			tituloArtesanato: null,
			preco: null,
			quantidadeArtesanato: 0,
			descricaoArtesanato: null,
			materiaisUtilizados: null,
			dataCriacao: null,
			tempoCriacaoHr: null,
			artesao: artesao);

		act.Should().Throw<ArgumentException>().WithMessage("*ArtesaoId*");
	}

	#endregion
}