using Microsoft.AspNetCore.Http;
using PrototipoBackEnd.Application.Exceptions;

namespace PrototipoBackEnd.Application.Common.Helpers
{
    public static class ImagemUploadValidator
    {
        public const long TamanhoMaximoBytes = 5 * 1024 * 1024;

        private static readonly HashSet<string> ContentTypesPermitidos = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/png", "image/webp"
        };

        private static readonly HashSet<string> ExtensoesPermitidas = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

        public static void Validar(IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
                throw new ValidationException("Arquivo de imagem vazio.");

            if (arquivo.Length > TamanhoMaximoBytes)
                throw new ValidationException("A imagem excede o tamanho máximo de 5 MB.");

            if (!ContentTypesPermitidos.Contains(arquivo.ContentType ?? string.Empty))
                throw new ValidationException("Tipo de imagem não permitido. Use jpeg, png ou webp.");

            var extensao = Path.GetExtension(arquivo.FileName ?? string.Empty);
            if (!ExtensoesPermitidas.Contains(extensao))
                throw new ValidationException("Extensão de imagem não permitida. Use .jpg, .jpeg, .png ou .webp.");
        }

        public static void Validar(IEnumerable<IFormFile> arquivos)
        {
            foreach (var arquivo in arquivos)
                Validar(arquivo);
        }
    }
}
