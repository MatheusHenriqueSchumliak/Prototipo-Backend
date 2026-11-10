namespace PrototipoBackEnd.Domain.Filters
{
    public class ArtesaoFiltro : PaginacaoFiltro
    {
        public static readonly string[] CamposOrdenaveis = { "nome", CampoOrdenacaoPadrao };

        public string? Nome { get; set; }
        public string[]? Especialidade { get; set; }
        public bool? RecebeEncomenda { get; set; }
        public bool? EnviaEncomenda { get; set; }
        public bool? FeiraMunicipal { get; set; }
        public bool? LocalFisico { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
    }
}
