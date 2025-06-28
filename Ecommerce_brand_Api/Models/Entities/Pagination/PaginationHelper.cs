namespace Ecommerce_brand_Api.Models.Entities.Pagination
{
    public static class PaginationHelper
    {
        public static async Task<PaginatedResult<T>> ToPaginatedResultAsync<T>(
            this IQueryable<T> query,
            int pageIndex,
            int pageSize)
        {
            var count = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PaginatedResult<T>
            {
                Items = items,
                TotalCount = count,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
