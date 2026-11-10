namespace PrototipoBackEnd.Domain.Common
{
    public class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; }
        public int Page { get; }
        public int PageSize { get; }
        public long TotalItems { get; }
        public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)PageSize);

        public PagedResult(IEnumerable<T> items, int page, int pageSize, long totalItems)
        {
            Items = items.ToList();
            Page = page;
            PageSize = pageSize;
            TotalItems = totalItems;
        }

        public PagedResult<TOut> Map<TOut>(Func<T, TOut> conversor)
            => new(Items.Select(conversor), Page, PageSize, TotalItems);
    }
}
