using Microsoft.AspNetCore.Http;
using Moq;
using PrototipoBackEnd.Application.Dtos.Artesanato;
using PrototipoBackEnd.Application.Exceptions;
using PrototipoBackEnd.Application.Services;
using PrototipoBackEnd.Domain.Entities;
using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Domain.Interfaces.Services;
using PrototipoBackEnd.Domain.ValueObjects;
using Xunit;

namespace PrototipoBackEnd.Tests.Unit.Services
{
    public class ArtesanatoServiceTests
    {
        private readonly Mock<IArtesanatoRepository> _artesanatos = new();
        private readonly Mock<IArtesaoRepository> _artesaos = new();
        private readonly Mock<IAmazonS3Service> _s3 = new();
        private readonly ArtesanatoService _service;

        public ArtesanatoServiceTests()
        {
            _service = new ArtesanatoService(_artesanatos.Object, _artesaos.Object, _s3.Object);
        }

        private static Artesanato Novo() =>
            Artesanato.Criar(Guid.NewGuid().ToString(), "T", "D", 1, 1, 10m, true, false, true,
                new Categoria(new[] { "c" }), new Material(new[] { "m" }), new Midia(new[] { "img1" }));

        [Fact]
        public async Task Atualizar_SemImagens_PreservaMidiaAtual()
        {
            var artesanato = Novo();
            _artesanatos.Setup(r => r.BuscarPorId(artesanato.Id)).ReturnsAsync(artesanato);
            _artesanatos.Setup(r => r.Atualizar(It.IsAny<Artesanato>(), It.IsAny<string>())).ReturnsAsync((Artesanato a, string _) => a);

            await _service.Atualizar(new ArtesanatoDto { Titulo = "Novo", Descricao = "D", Preco = 5m }, artesanato.Id);

            Assert.Equal("Novo", artesanato.Titulo);
            Assert.Equal(new[] { "img1" }, artesanato.Midia.Imagens);
            _s3.Verify(s => s.Upload(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Atualizar_Inativo_LancaNotFound()
        {
            var artesanato = Novo();
            artesanato.Inativar();
            _artesanatos.Setup(r => r.BuscarPorId(artesanato.Id)).ReturnsAsync(artesanato);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.Atualizar(new ArtesanatoDto(), artesanato.Id));
        }

        [Fact]
        public async Task Adicionar_ComArtesaoInexistente_LancaNotFound()
        {
            var artesaoId = Guid.NewGuid().ToString();
            _artesaos.Setup(r => r.BuscarPorId(artesaoId)).ReturnsAsync((Artesao?)null!);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.Adicionar(new ArtesanatoDto { ArtesaoId = artesaoId }, new List<IFormFile>()));
        }
    }
}
