
using Ecommerce_brand_Api.Models.Dtos;
using Ecommerce_brand_Api.Models.Dtos.OrdersDTO;
using Ecommerce_brand_Api.Models.Entities;

namespace Ecommerce_brand_Api.Services
{
    public class CartServices : ICartService
    {
        private readonly IUnitofwork _unitofwork;
        private readonly IMapper mapper;

        public CartServices(IUnitofwork _unitofwork, IMapper mapper)
        {
            this._unitofwork = _unitofwork;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<CartDto>> GetCartAsync()
        {
            try
            {
                var cartRepo = _unitofwork.GetBaseRepository<Cart>();
                var carts = await cartRepo.GetAllAsync();
                return mapper.Map<IEnumerable<CartDto>>(carts);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving all orders.", ex);
            }
        }

        public async Task<CartDto?> GetCartByIdAsync(int Id)
        {
            try
            {
                var cartRepo = _unitofwork.GetBaseRepository<Cart>();
                var carts = await cartRepo.GetByIdAsync(Id);
                return carts == null ? null : mapper.Map<CartDto>(carts);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving order with ID {Id}.", ex);
            }
        }

        public async Task<CartDto> AddNewCartAsync(CartDto cartDto)
        {
            if (cartDto == null)
                throw new ArgumentNullException(nameof(cartDto), "Cart data cannot be null.");

            try
            {
                var cartEntity = mapper.Map<Cart>(cartDto);
                var repo = _unitofwork.GetBaseRepository<Cart>();

                await repo.AddAsync(cartEntity);
                await _unitofwork.SaveChangesAsync();
                return mapper.Map<CartDto>(cartEntity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while adding a new Cart.", ex);
            }
        }

        public async Task<bool> DeleteCartAsync(int Id)
        {
            if (Id <= 0)
                throw new ArgumentException("Invalid Order ID", nameof(Id));

            try
            {
                var repo = _unitofwork.GetBaseRepository<Cart>();

                var deleteCart = await repo.GetByIdAsync(Id);

                if (deleteCart == null)
                {
                    return false;
                }

                await repo.DeleteAsync(Id);
                await _unitofwork.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while deleting the Cart with ID {Id}.", ex);
            }
        }

        /// <summary>
        /// Updates an existing cart with the specified ID using the provided updated cart data.
        /// </summary>
        /// <remarks>This method retrieves the cart by its ID, updates its data using the provided
        /// <paramref name="updatedcart"/>,  and saves the changes to the database. If the cart with the specified ID
        /// does not exist, the method returns <see langword="false"/>.</remarks>
        /// <param name="Id">The unique identifier of the cart to update. Must be greater than 0.</param>
        /// <param name="updatedcart">An object containing the updated cart data. Cannot be null.</param>
        /// <returns><see langword="true"/> if the cart was successfully updated; otherwise, <see langword="false"/> if the cart
        /// with the specified ID does not exist.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="Id"/> is less than or equal to 0.</exception>
        /// <exception cref="ApplicationException">Thrown if an error occurs while updating the cart, with additional details about the failure.</exception> 
        public async Task<bool> UpdateCartAsync(int Id, CartDto updatedcart)
        {

            if (Id <= 0)
                throw new ArgumentException("Invalid Order ID", nameof(Id));

            try
            {
                var repo = _unitofwork.GetBaseRepository<Cart>();
                var cart = await repo.GetByIdAsync(Id);

                if (cart == null)
                    return false;

                var updatedEntity = mapper.Map<Cart>(updatedcart);
                updatedEntity.Id = Id;

                await repo.UpdateAsync(updatedEntity);
                await _unitofwork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while updating the order with ID {Id}.", ex);
            }
        }
    }
}
