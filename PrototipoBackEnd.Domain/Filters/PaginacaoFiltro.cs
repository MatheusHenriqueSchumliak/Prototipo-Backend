namespace PrototipoBackEnd.Domain.Filters
{
    // Base de todas as listagens: paginação, ordenação e visibilidade (IsAtivo).
    public class PaginacaoFiltro
    {
        public const int PageSizePadrao = 10;
        public const int PageSizeMaximo = 50;
        public const string CampoOrdenacaoPadrao = "dataCriacao";

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = PageSizePadrao;
        public string? SortBy { get; set; }
        public string? SortDir { get; set; }

        // Nulo = somente ativos. Apenas administradores podem informar false (o controller descarta o valor dos demais).
        public bool? IsAtivo { get; set; }

        // Sem sortBy: DataCriacao desc. Com sortBy e sem sortDir: asc.
        public bool Descendente => string.IsNullOrWhiteSpace(SortDir)
            ? string.IsNullOrWhiteSpace(SortBy)
            : string.Equals(SortDir.Trim(), "desc", StringComparison.OrdinalIgnoreCase);
    }
}
