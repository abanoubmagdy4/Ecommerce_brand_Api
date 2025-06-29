using Ecommerce_brand_Api.Models.Dtos.OrdersDTO;

namespace Ecommerce_brand_Api.Services.Interfaces
{
    public interface IOrderService : IBaseService<Order>
    {
        /// <summary>
        /// Retrieves all orders asynchronously.
        /// </summary>
        /// <remarks>This method returns a collection of orders in the form of OrderDTO objects. The
        /// collection will be empty if no orders are available.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains an  IEnumerable{T} of OrderDTO
        /// objects representing the orders.</returns> 
        Task<IEnumerable<OrderDTO>> GetAllOrdersAsync();

        /// <summary>
        /// Retrieves the order details for the specified order ID.
        /// </summary>
        /// <remarks>Use this method to fetch detailed information about a specific order. If the order
        /// does not exist,  the method returns <see langword="null"/> instead of throwing an exception.</remarks>
        /// <param name="OrderId">The unique identifier of the order to retrieve. Must be a positive integer.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="OrderDTO"/> 
        /// object with the order details if the order is found; otherwise, <see langword="null"/>.</returns> 
        Task<OrderDTO?> GetOrderByIdAsync(int OrderId);

        /// <summary>
        /// Adds a new order to the system asynchronously.
        /// </summary>
        /// <remarks>This method validates the provided order details before adding the order to the system.
        /// Ensure that all required fields in the <paramref name="order"/> object are populated and valid.</remarks>
        /// <param name="order">The order details to be added. Must not be null and should contain valid data.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added order details,
        /// including any system-generated fields.</returns>
        Task<Order> AddNewOrderAsync(OrderDTO order, Address address, ApplicationUser user);

        /// <summary>
        /// Updates the details of an existing order asynchronously.
        /// </summary>
        /// <remarks>This method performs a validation check on the provided <paramref name="orderId"/>
        /// and <paramref name="updatedOrder"/> before attempting the update. Ensure that the <paramref
        /// name="updatedOrder"/> object contains valid data to avoid update failures.</remarks>
        /// <param name="orderId">The unique identifier of the order to update. Must be a positive integer.</param>
        /// <param name="updatedOrder">An <see cref="OrderDTO"/> object containing the updated order details. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the update
        /// was successful; otherwise, <see langword="false"/>.</returns>
        Task<bool> UpdateOrderAsync(int orderId, OrderDTO updatedOrder);

        /// <summary>
        /// Deletes the order with the specified identifier asynchronously.
        /// </summary>
        /// <remarks>This method performs the deletion operation asynchronously. If the specified order
        /// does not exist,  the method returns <see langword="false"/> without throwing an exception.</remarks>
        /// <param name="OrderId">The unique identifier of the order to delete. Must be a positive integer.</param>
        /// <returns><see langword="true"/> if the order was successfully deleted; otherwise, <see langword="false"/>.</returns>
        Task<bool> DeleteOrderAsync(int OrderId);
        Task<IEnumerable<OrderDTO>> GetOrdersWithCustomerAsync();
        Task<IEnumerable<OrderDTO>> GetOrdersByStatusAsync(OrderStatus status);
        Task<IEnumerable<OrderDTO>> GetOrdersOverTotalAsync(decimal amount);
        Task<ServiceResult> BuildOrderDto(OrderDTO orderDto);
            }
}
