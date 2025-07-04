using Ecommerce_brand_Api.Models.Entities.Pagination;

namespace Ecommerce_brand_Api.Helpers
{
    public class ProductFilterParams : PaginationParams
    {
        public int? CategoryId { get; set; }

        // تقدر تضيف فلاتر تانية هنا بعدين:
        public string? SearchTerm { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? isNewArrival { get; set; }
    }
}
