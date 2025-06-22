namespace Ecommerce_brand_Api.Services.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? Email { get; }

    }
}
