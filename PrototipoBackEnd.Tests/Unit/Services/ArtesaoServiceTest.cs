using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Domain.Interfaces.Services;
using PrototipoBackEnd.Application.Services;
using PrototipoBackEnd.Application.Dtos;
using PrototipoBackEnd.Domain.Entities;
using Microsoft.AspNetCore.Http;
using FluentAssertions;
using AutoMapper;
using Xunit;
using Moq;

namespace PrototipoBackEnd.UnitTests.Unit.Services;

public class ArtesaoServiceTest
{
	private readonly Mock<IArtesaoRepository> _repo = new();
	private readonly Mock<IAmazonS3Service> _s3 = new();
	private readonly Mock<IMapper> _mapper = new();

	private ArtesaoService CreateSut() => new(_repo.Object, _s3.Object, _mapper.Object);

	private static Artesao NewArtesao(string id, string usuarioId)
		=> new()
		{
			Id = id,
			UsuarioId = usuarioId,
			NomeArtesao = "Ateliê X",
			NomeCompleto = "Nome X",
			Telefone = "4999999999",
			WhatsApp = "4999999999",
			Email = "x@x.com",
			Idade = 30,
			FotoUrl = "https://old/img.jpg"
		};

	private static ArtesaoDto NewDto(string usuarioId)
		=> new()
		{
			UsuarioId = usuarioId,
			NomeArtesao = "Ateliê X",
			NomeCompleto = "Nome X",
			Telefone = "4999999999",
			WhatsApp = "4999999999",
			Email = "x@x.com",
			Idade = 30,
			FotoUrl = null,
			NichoAtuacao = "Nicho",
			ReceberEncomendas = true,
			EnviaEncomendas = false,
			CEP = "12345678",
			Estado = "PR",
			Cidade = "Cidade",
			Rua = "Rua",
			Bairro = "Bairro",
			Numero = "1",
			SemNumero = false
		};

	private static IFormFile BuildFormFile(string fileName, string contentType, byte[] content)
	{
		var ms = new MemoryStream(content);
		var fileMock = new Mock<IFormFile>();
		fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
		fileMock.SetupGet(f => f.Length).Returns(ms.Length);
		fileMock.SetupGet(f => f.FileName).Returns(fileName);
		fileMock.SetupGet(f => f.ContentType).Returns(contentType);
		return fileMock.Object;
	}

	[Fact(DisplayName = "BuscarTodos retorna dtos")]
	public async Task BuscarTodos_DeveRetornarDtos()
	{
		var artes = new List<Artesao> { NewArtesao(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()) };
		var dtos = new List<ArtesaoDto> { new() { Id = artes[0].Id, UsuarioId = artes[0].UsuarioId } };

		_repo.Setup(r => r.BuscarTodos()).ReturnsAsync(artes);
		_mapper.Setup(m => m.Map<List<ArtesaoDto>>(artes)).Returns(dtos);

		var sut = CreateSut();

		var result = await sut.BuscarTodos();

		result.Should().BeEquivalentTo(dtos);
		_repo.Verify(r => r.BuscarTodos(), Times.Once);
		_mapper.Verify(m => m.Map<List<ArtesaoDto>>(artes), Times.Once);
	}

	[Fact(DisplayName = "BuscarPorId quando encontrado retorna dto")]
	public async Task BuscarPorId_Encontrado_DeveRetornarDto()
	{
		var id = Guid.NewGuid().ToString();
		var artesao = NewArtesao(id, Guid.NewGuid().ToString());
		var dto = new ArtesaoDto { Id = id, UsuarioId = artesao.UsuarioId };

		_repo.Setup(r => r.BuscarPorId(id)).ReturnsAsync(artesao);
		_mapper.Setup(m => m.Map<ArtesaoDto>(artesao)).Returns(dto);

		var sut = CreateSut();

		var result = await sut.BuscarPorId(id);

		result.Should().BeEquivalentTo(dto);
	}

	[Fact(DisplayName = "BuscarPorId quando não encontrado lança exceção")]
	public async Task BuscarPorId_NaoEncontrado_DeveLancar()
	{
		var id = Guid.NewGuid().ToString();
		_repo.Setup(r => r.BuscarPorId(id)).ReturnsAsync((Artesao?)null);

		var sut = CreateSut();

		await Assert.ThrowsAsync<Exception>(() => sut.BuscarPorId(id));
	}

	[Fact(DisplayName = "Adicionar com imagem faz upload, adiciona e retorna dto")]
	public async Task Adicionar_ComImagem_DeveFazerUploadEAdicionar()
	{
		var usuarioId = Guid.NewGuid().ToString();
		var dto = NewDto(usuarioId);
		var file = BuildFormFile("img.jpg", "image/jpeg", new byte[] { 1, 2, 3 });
		var uploadedUrl = "https://s3/img.jpg";
		_s3.Setup(s => s.Upload(It.IsAny<Stream>(), "img.jpg", "image/jpeg")).ReturnsAsync(uploadedUrl);

		// Ajuste: Adicionar no repositório retorna Task<Artesao>, devolva a mesma entidade recebida
		_repo.Setup(r => r.Adicionar(It.IsAny<Artesao>())).ReturnsAsync((Artesao a) => a);
		Moq.Language.Flow.IReturnsResult<IMapper> returnsResult = _mapper.Setup(m => m.Map<ArtesaoDto>(It.IsAny<Artesao>()))
			.Returns(valueFunction: static (Artesao a) => new ArtesaoDto { Id = a.Id, FotoUrl = a.FotoUrl, UsuarioId = a.UsuarioId });

		var sut = CreateSut();

		var result = await sut.Adicionar(dto, file);

		_s3.Verify(s => s.Upload(It.IsAny<Stream>(), "img.jpg", "image/jpeg"), Times.Once);
		_repo.Verify(r => r.Adicionar(It.IsAny<Artesao>()), Times.Once);
		result.FotoUrl.Should().Be(uploadedUrl);
	}

	[Fact(DisplayName = "Adicionar sem imagem não faz upload")]
	public async Task Adicionar_SemImagem_NaoFazUpload()
	{
		var usuarioId = Guid.NewGuid().ToString();
		var dto = NewDto(usuarioId);

		_repo.Setup(r => r.Adicionar(It.IsAny<Artesao>())).ReturnsAsync((Artesao a) => a);
		_mapper.Setup(m => m.Map<ArtesaoDto>(It.IsAny<Artesao>())).Returns(new ArtesaoDto { UsuarioId = Guid.NewGuid().ToString() });

		var sut = CreateSut();

		var result = await sut.Adicionar(dto, null!);

		_s3.Verify(s => s.Upload(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
		_repo.Verify(r => r.Adicionar(It.IsAny<Artesao>()), Times.Once);
	}

	[Fact(DisplayName = "Atualizar quando id nulo lança")]
	public async Task Atualizar_IdNulo_DeveLancar()
	{
		var sut = CreateSut();
		var dto = NewDto(Guid.NewGuid().ToString());

		await Assert.ThrowsAsync<Exception>(() => sut.Atualizar(dto, null!));
	}

	[Fact(DisplayName = "Atualizar quando não encontrado lança")]
	public async Task Atualizar_NaoEncontrado_DeveLancar()
	{
		var id = Guid.NewGuid().ToString();
		_repo.Setup(r => r.BuscarPorId(id)).ReturnsAsync((Artesao?)null);

		var sut = CreateSut();
		var dto = NewDto(Guid.NewGuid().ToString());

		await Assert.ThrowsAsync<Exception>(() => sut.Atualizar(dto, id));
	}

	[Fact(DisplayName = "Atualizar com nova imagem faz upload e atualiza")]
	public async Task Atualizar_ComImagem_DeveFazerUploadEAtualizar()
	{
		var id = Guid.NewGuid().ToString();
		var usuarioId = Guid.NewGuid().ToString();
		var existing = NewArtesao(id, usuarioId);
		var dto = NewDto(usuarioId);
		var file = BuildFormFile("new.jpg", "image/png", new byte[] { 9 });
		var uploadedUrl = "https://s3/new.jpg";

		_repo.Setup(r => r.BuscarPorId(id)).ReturnsAsync(existing);
		_s3.Setup(s => s.Upload(It.IsAny<Stream>(), "new.jpg", "image/png")).ReturnsAsync(uploadedUrl);
		_mapper.Setup(m => m.Map(dto, existing))
			.Callback<ArtesaoDto, Artesao>((d, a) =>
			{
				// simulando o comportamento do mapper: copiar alguns campos
				a.NomeArtesao = d.NomeArtesao;
				a.FotoUrl = d.FotoUrl;
			})
			.Returns(existing);
		// Atualizar retorna Task<Artesao> no repositório — devolva o objeto existente
		_repo.Setup(r => r.Atualizar(existing, id)).ReturnsAsync(existing);
		_mapper.Setup(m => m.Map<ArtesaoDto>(existing)).Returns(new ArtesaoDto { Id = existing.Id, FotoUrl = uploadedUrl, UsuarioId = existing.UsuarioId });

		var sut = CreateSut();

		var result = await sut.Atualizar(dto, id, file);

		_s3.Verify(s => s.Upload(It.IsAny<Stream>(), "new.jpg", "image/png"), Times.Once);
		_repo.Verify(r => r.Atualizar(existing, id), Times.Once);
		result.FotoUrl.Should().Be(uploadedUrl);
	}

	[Fact(DisplayName = "Atualizar sem imagem mantém foto existente")]
	public async Task Atualizar_SemImagem_MantemFotoExistente()
	{
		var id = Guid.NewGuid().ToString();
		var usuarioId = Guid.NewGuid().ToString();
		var existing = NewArtesao(id, usuarioId);
		var dto = NewDto(usuarioId);

		_repo.Setup(r => r.BuscarPorId(id)).ReturnsAsync(existing);
		_mapper.Setup(m => m.Map(dto, existing)).Returns(existing);
		// Corrigido: retorna a entidade atualizada, não Task.CompletedTask
		_repo.Setup(r => r.Atualizar(existing, id)).ReturnsAsync(existing);
		_mapper.Setup(m => m.Map<ArtesaoDto>(existing)).Returns(new ArtesaoDto { Id = existing.Id, FotoUrl = existing.FotoUrl, UsuarioId = existing.UsuarioId });

		var sut = CreateSut();

		var result = await sut.Atualizar(dto, id, null);

		_s3.Verify(s => s.Upload(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
		_repo.Verify(r => r.Atualizar(existing, id), Times.Once);
		result.FotoUrl.Should().Be(existing.FotoUrl);
	}

	[Fact(DisplayName = "Apagar com id nulo lança")]
	public async Task Apagar_IdNulo_DeveLancar()
	{
		var sut = CreateSut();
		await Assert.ThrowsAsync<Exception>(() => sut.Apagar(null!));
	}

	[Fact(DisplayName = "Apagar chama repositório e retorna true")]
	public async Task Apagar_DeveChamarRepositorioERetornarTrue()
	{
		var id = Guid.NewGuid().ToString();
		_repo.Setup(r => r.Deletar(id)).ReturnsAsync(true);

		var sut = CreateSut();

		var result = await sut.Apagar(id);

		result.Should().BeTrue();
		_repo.Verify(r => r.Deletar(id), Times.Once);
	}

	[Fact(DisplayName = "BuscarComFiltro encaminha filtro e retorna dtos")]
	public async Task BuscarComFiltro_DeveEncaminharFiltroERetornarDtos()
	{
		var artes = new List<Artesao> { NewArtesao(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()) };
		var dtos = new List<ArtesaoDto> { new() { Id = artes[0].Id, UsuarioId = artes[0].UsuarioId } };

		_repo.Setup(r => r.BuscarComFiltro(It.IsAny<MongoDB.Driver.FilterDefinition<Artesao>>())).ReturnsAsync(artes);
		_mapper.Setup(m => m.Map<List<ArtesaoDto>>(artes)).Returns(dtos);

		var sut = CreateSut();

		var result = await sut.BuscarComFiltro("nome", "nicho", true, false);

		result.Should().BeEquivalentTo(dtos);
		_repo.Verify(r => r.BuscarComFiltro(It.IsAny<MongoDB.Driver.FilterDefinition<Artesao>>()), Times.Once);
		_mapper.Verify(m => m.Map<List<ArtesaoDto>>(artes), Times.Once);
	}
}