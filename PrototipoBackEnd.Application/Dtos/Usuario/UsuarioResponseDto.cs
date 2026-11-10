using PrototipoBackEnd.Domain.Enumerables;

namespace PrototipoBackEnd.Application.Dtos.Usuario
{
    public class UsuarioResponseDto
    {
        public string Id { get; set; } = null!;
        public string PessoaId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public UsuarioEnum Role { get; set; }
        public bool IsAtivo { get; set; }
    }
}
