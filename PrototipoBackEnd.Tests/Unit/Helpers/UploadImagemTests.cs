using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Moq;
using PrototipoBackEnd.Application.Common.Helpers;
using PrototipoBackEnd.Application.Exceptions;
using Xunit;

namespace PrototipoBackEnd.Tests.Unit.Helpers
{
    public class UploadImagemTests
    {
        private static IFormFile Arquivo(string nome, string contentType, long tamanho)
        {
            var mock = new Mock<IFormFile>();
            mock.SetupGet(f => f.FileName).Returns(nome);
            mock.SetupGet(f => f.ContentType).Returns(contentType);
            mock.SetupGet(f => f.Length).Returns(tamanho);
            return mock.Object;
        }

        [Theory]
        [InlineData("a.jpg", "image/jpeg")]
        [InlineData("a.JPEG", "image/jpeg")]
        [InlineData("a.png", "image/png")]
        [InlineData("a.webp", "image/webp")]
        public void Validar_ArquivoValido_NaoLanca(string nome, string contentType)
        {
            ImagemUploadValidator.Validar(Arquivo(nome, contentType, 1024));
        }

        [Fact]
        public void Validar_ExatamenteCincoMb_Passa()
        {
            ImagemUploadValidator.Validar(Arquivo("a.png", "image/png", 5 * 1024 * 1024));
        }

        [Theory]
        [InlineData("a.png", "image/png", 5 * 1024 * 1024 + 1)]
        [InlineData("a.png", "image/png", 0)]
        [InlineData("a.gif", "image/gif", 1024)]
        [InlineData("a.png", "application/pdf", 1024)]
        [InlineData("a.exe", "image/png", 1024)]
        [InlineData("semextensao", "image/png", 1024)]
        public void Validar_ArquivoInvalido_LancaValidation(string nome, string contentType, long tamanho)
        {
            Assert.Throws<ValidationException>(() => ImagemUploadValidator.Validar(Arquivo(nome, contentType, tamanho)));
        }

        [Fact]
        public void Validar_Lista_FalhaSeAlgumArquivoForInvalido()
        {
            var arquivos = new[] { Arquivo("a.png", "image/png", 10), Arquivo("b.gif", "image/gif", 10) };

            Assert.Throws<ValidationException>(() => ImagemUploadValidator.Validar(arquivos));
        }

        [Fact]
        public void GerarCaminhoArquivo_UsaGuidEData_EIgnoraNomeOriginal()
        {
            var chave = S3FileHelper.GerarCaminhoArquivo("artesaos", "../../Minha Foto.PNG");

            Assert.Matches(new Regex(@"^artesaos/\d{4}/\d{2}/\d{2}/[0-9a-f]{8}-([0-9a-f]{4}-){3}[0-9a-f]{12}\.png$"), chave);
            Assert.DoesNotContain("Minha", chave);
            Assert.DoesNotContain("..", chave);
        }
    }
}
