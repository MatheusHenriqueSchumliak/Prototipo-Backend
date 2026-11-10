using System.ComponentModel.DataAnnotations;
using PrototipoBackEnd.Domain.Enumerables;

namespace PrototipoBackEnd.Application.Dtos.Usuario
{
    public class UsuarioAtualizarRequestDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [EnumDataType(typeof(UsuarioEnum))]
        public UsuarioEnum Role { get; set; }

        // Opcional: só altera a senha se for informada.
        [StringLength(72, MinimumLength = 8)]
        public string? Senha { get; set; }
    }
}
