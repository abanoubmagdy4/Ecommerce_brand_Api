namespace Ecommerce_brand_Api.Models.Entities
{
    public class Category
    { 
        public int Id { get; set; }
       
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; } = false;
        public virtual ICollection<Product>? Products { get; set; }
    }
}
