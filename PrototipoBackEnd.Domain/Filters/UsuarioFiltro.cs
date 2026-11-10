using PrototipoBackEnd.Domain.Enumerables;

namespace PrototipoBackEnd.Domain.Filters
{
    public class UsuarioFiltro : PaginacaoFiltro
    {
        public static readonly string[] CamposOrdenaveis = { "email", "role", CampoOrdenacaoPadrao };

        public string? Email { get; set; }
        public UsuarioEnum? Role { get; set; }
    }
}
