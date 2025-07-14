public static class AppDbSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        // Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "T-Shirts", Description = "Cool casual t-shirts", IsDeleted = false },
            new Category { Id = 2, Name = "Hoodies", Description = "Warm and comfy hoodies", IsDeleted = false },
            new Category { Id = 3, Name = "Jeans", Description = "Denim collection", IsDeleted = false }
        );

        // Products
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 6,
                Name = "Proc#1",
                Description = "fantastic product",
                Price = 100,
                DiscountPercentage = 0,
                PriceAfterDiscount = 100,
                AverageRating = 4.5,
                IsDeleted = false,
                CategoryId = 1,
                CreatedAt = DateTime.Now
            }
        );

        // ProductSizes
        modelBuilder.Entity<ProductSizes>().HasData(
            new ProductSizes { Id = 9, Size = "S", Width = 100, Height = 200, ProductId = 6, StockQuantity = 5 },
            new ProductSizes { Id = 10, Size = "M", Width = 100, Height = 200, ProductId = 6, StockQuantity = 1 },
            new ProductSizes { Id = 11, Size = "L", Width = 100, Height = 200, ProductId = 6, StockQuantity = 1 }
        );

        // ProductImagesPaths
        modelBuilder.Entity<ProductImagesPaths>().HasData(
            new ProductImagesPaths
            {
                Id = 7,
                ImagePath = "/uploads/72e0ce42-179b-43ac-a999-afd11af07f44.png",
                Priority = 1,
                ProductId = 6
            },
            new ProductImagesPaths
            {
                Id = 10,
                ImagePath = "/uploads/251c3046-7bf3-4237-9e0f-1ebb0db7e327.png",
                Priority = 3,
                ProductId = 6
            }
        );

        // NewArrivals
        modelBuilder.Entity<NewArrivals>().HasData(
            new NewArrivals { Id = 1, ProductId = 6 }
        );

        modelBuilder.Entity<Discount>().HasData(
                new Discount { Id = 1, Threshold = 0, DicountValue = 0 }
            );
    }
}
