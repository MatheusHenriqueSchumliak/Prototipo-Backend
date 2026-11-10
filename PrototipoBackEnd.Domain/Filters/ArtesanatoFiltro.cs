namespace PrototipoBackEnd.Domain.Filters
{
    public class ArtesanatoFiltro : PaginacaoFiltro
    {
        public static readonly string[] CamposOrdenaveis = { "titulo", "preco", CampoOrdenacaoPadrao };

        public string? Titulo { get; set; }
        public string? ArtesaoId { get; set; }
        public string? Categoria { get; set; }
        public string? Material { get; set; }
        public decimal? PrecoMin { get; set; }
        public decimal? PrecoMax { get; set; }
        public bool? TemEstoque { get; set; }
        public bool? SomenteEncomenda { get; set; }
        public bool? AceitaEncomenda { get; set; }
    }
}
