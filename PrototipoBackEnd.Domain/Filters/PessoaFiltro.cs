namespace PrototipoBackEnd.Domain.Filters
{
    public class PessoaFiltro : PaginacaoFiltro
    {
        public static readonly string[] CamposOrdenaveis = { "nome", CampoOrdenacaoPadrao };

        public string? Nome { get; set; }
    }
}
