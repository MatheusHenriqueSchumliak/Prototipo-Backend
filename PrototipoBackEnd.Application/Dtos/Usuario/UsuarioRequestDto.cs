using System.ComponentModel.DataAnnotations;
using PrototipoBackEnd.Domain.Enumerables;

namespace PrototipoBackEnd.Application.Dtos.Usuario
{
    public class UsuarioRequestDto
    {
        [Required]
        public string PessoaId { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(72, MinimumLength = 8)]
        public string Senha { get; set; } = string.Empty;

        [EnumDataType(typeof(UsuarioEnum))]
        public UsuarioEnum Role { get; set; }
    }
}
