using Ecommerce_brand_Api.Helpers;
using Ecommerce_brand_Api.Helpers.Enums;
using Ecommerce_brand_Api.Models.Entities;
using Ecommerce_brand_Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Ecommerce_brand_Api.Services
{
    public class OrderServices : BaseService<Order>, IOrderService
    {
        private readonly IUnitofwork _unitofwork;
        private readonly IMapper mapper;
        private readonly IGovernrateShippingCostRepository _governrateShippingCostRepository;
        private readonly IDiscountRepository _discountRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderServices"/> class.
        /// </summary>
        /// <remarks>This constructor sets up the dependencies required for the <see
        /// cref="OrderServices"/> class to function. Ensure that valid instances of <paramref name="_unitofwork"/> and
        /// <paramref name="mapper"/> are provided when creating an instance of this class.</remarks>
        /// <param name="_unitofwork">The unit of work instance used to manage database transactions and repositories.</param>
        /// <param name="mapper">The mapper instance used for object-to-object mapping between domain models and DTOs.</param> 
        public OrderServices(IUnitofwork _unitofwork, IMapper mapper) : base(_unitofwork.GetBaseRepository<Order>())
        {
            this._unitofwork = _unitofwork;
            _discountRepository = _unitofwork.Discount;
            this.mapper = mapper;
        }

        /// <summary>
        /// Asynchronously retrieves all orders from the data source.
        /// </summary>
        /// <remarks>This method fetches all orders and maps them to a collection of <see
        /// cref="OrderDTO"/> objects.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IEnumerable{T}"/>
        /// of <see cref="OrderDTO"/> objects representing the retrieved orders.</returns>
        /// <exception cref="ApplicationException">Thrown when an error occurs while retrieving the orders.</exception> 
        public async Task<IEnumerable<OrderDTO>> GetAllOrdersAsync()
        {
            try
            {
                var orderRepo = _unitofwork.GetBaseRepository<Order>();
                var orders = await orderRepo.GetAllAsync();
                return mapper.Map<IEnumerable<OrderDTO>>(orders);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving all orders.", ex);
            }
        }

        /// <summary>
        /// Asynchronously retrieves an order by its unique identifier.
        /// </summary>
        /// <remarks>This method fetches the order from the underlying data source using the provided
        /// <paramref name="OrderId"/>. If no order is found with the specified ID, the method returns <see
        /// langword="null"/>.</remarks>
        /// <param name="OrderId">The unique identifier of the order to retrieve. Must be a positive integer.</param>
        /// <returns>An <see cref="OrderDTO"/> representing the order if found; otherwise, <see langword="null"/>.</returns>
        /// <exception cref="ApplicationException">Thrown when an error occurs while attempting to retrieve the order.</exception> 
        public async Task<OrderDTO?> GetOrderByIdAsync(int OrderId)
        {
            try
            {
                var orderRepo = _unitofwork.GetBaseRepository<Order>();
                var order = await orderRepo.GetByIdAsync(OrderId);
                return order == null ? null : mapper.Map<OrderDTO>(order);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving order with ID {OrderId}.", ex);
            }
        }

        /// <summary>
        /// Adds a new order to the system asynchronously.
        /// </summary>
        /// <remarks>This method maps the provided <paramref name="orderDto"/> to an order entity, adds it
        /// to the repository,  and commits the changes to the database. The returned <see cref="OrderDTO"/> reflects
        /// the state of the  newly added order.</remarks>
        /// <param name="orderDto">The data transfer object containing the details of the order to be added. Cannot be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added order as a data
        /// transfer object.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="orderDto"/> is <see langword="null"/>.</exception>
        /// <exception cref="ApplicationException">Thrown if an error occurs while adding the order.</exception> 
        public async Task<Order> AddNewOrderAsync(OrderDTO orderDto, Address address, ApplicationUser user)
        {
            if (orderDto == null || address == null || user == null)
                throw new ArgumentNullException("Data cannot be null.");

            if (orderDto.OrderItems == null || !orderDto.OrderItems.Any())
                throw new ArgumentException("Order must contain at least one item.");


            try
            {
                var orderItemsAfterMapping = new List<OrderItem>();

                foreach (var dto in orderDto.OrderItems)
                {
                    var item = new OrderItem
                    {
                        ProductId = dto.ProductId,
                        Quantity = dto.Quantity,
                        ProductSizeId = dto.ProductSizeId,  
                        TotalPrice = dto.TotalPrice
                    };

                    orderItemsAfterMapping.Add(item);
                }
                string shippingAreaName = address.GovernorateShippingCost?.Name ?? "Unknown";

                Order order = new Order()
                {
                    OrderItems = orderItemsAfterMapping,
                    OrderStatus = OrderStatus.Created,
                    TotalOrderPrice = orderDto.TotalOrderPrice,
                    CreatedAt = orderDto.CreatedAt,
                    DeliveredAt = orderDto.DeliveredAt,
                    DiscountValue = Math.Max(orderDto.DiscountValue, 0),
                    OrderNumber = $"ORD-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                    // Customer and address cost 
                    CustomerId = user.Id,
                    ShippingAddressId = address.Id,
                    OrderAddressInfo = $"{address.Street}, Apt: {address.Apartment}, Bldg: {address.Building}, Floor: {address.Floor}, " + $"{address.City}, {shippingAreaName}",
                    ShippingCost = orderDto.ShippingCost,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,

                    // Navigation properties
                    Customer = user,
                    ShippingAddress = address,
                };
                var repo = _unitofwork.GetOrderRepository();

                await repo.AddAsync(order);
                await _unitofwork.SaveChangesAsync();

                return order;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while adding a new order.", ex);
            }
        }


        /// <summary>
        /// Deletes the order with the specified ID asynchronously.
        /// </summary>
        /// <remarks>This method retrieves the order by its ID and deletes it if it exists.  If the order
        /// does not exist, the method returns <see langword="false"/> without performing any deletion.</remarks>
        /// <param name="orderId">The unique identifier of the order to delete. Must be greater than zero.</param>
        /// <returns><see langword="true"/> if the order was successfully deleted;  otherwise, <see langword="false"/> if the
        /// order does not exist.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="orderId"/> is less than or equal to zero.</exception>
        /// <exception cref="ApplicationException">Thrown if an error occurs during the deletion process.</exception> 
        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentException("Invalid Order ID", nameof(orderId));

            try
            {
                var repo = _unitofwork.GetBaseRepository<Order>();
                var order = await repo.GetByIdAsync(orderId);

                if (order == null)
                    return false;

                await repo.DeleteAsync(orderId);
                await _unitofwork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while deleting the order with ID {orderId}.", ex);
            }
        }


        /// <summary>
        /// Updates an existing order with the specified details.
        /// </summary>
        /// <remarks>This method retrieves the existing order by its ID, updates its details using the
        /// provided <paramref name="updatedOrder"/>,  and saves the changes to the database. If the order does not
        /// exist, the method returns <see langword="false"/> without making any changes.</remarks>
        /// <param name="orderId">The unique identifier of the order to update. Must be greater than zero.</param>
        /// <param name="updatedOrder">An <see cref="OrderDTO"/> object containing the updated order details.</param>
        /// <returns><see langword="true"/> if the order was successfully updated; otherwise, <see langword="false"/> if the
        /// order with the specified ID does not exist.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="orderId"/> is less than or equal to zero.</exception>
        /// <exception cref="ApplicationException">Thrown if an error occurs during the update process. The exception contains additional details about the
        /// failure.</exception> 
        public async Task<bool> UpdateOrderAsync(int orderId, OrderDTO updatedOrder)
        {
            if (orderId <= 0)
                throw new ArgumentException("Invalid Order ID", nameof(orderId));

            try
            {
                var repo = _unitofwork.GetBaseRepository<Order>();
                var order = await repo.GetByIdAsync(orderId);

                if (order == null)
                    return false;

                var updatedEntity = mapper.Map<Order>(updatedOrder);
                updatedEntity.OrderId = orderId;

                await repo.UpdateAsync(updatedEntity);
                await _unitofwork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while updating the order with ID {orderId}.", ex);
            }
        }

        /// <summary> 
        /// Asynchronously retrieves a collection of orders filtered by their status.
        /// </summary>
        /// <param name="status">The status of the orders to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of  <see
        /// cref="OrderDTO"/> objects representing the orders with the specified status. If no orders  match the
        /// specified status, the collection will be empty.</returns>
        /// <exception cref="ApplicationException">Thrown when an error occurs while retrieving the orders.</exception>
        public async Task<IEnumerable<OrderDTO>> GetOrdersByStatusAsync(OrderStatus status)
        {
            try
            {
                var repo = _unitofwork.GetOrderRepository();
                var orders = await repo.GetOrdersByStatusAsync(status);
                return mapper.Map<IEnumerable<OrderDTO>>(orders);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving orders with status '{status}'.", ex);
            }
        }


        /// <summary>
        /// Retrieves a collection of orders where the total amount exceeds the specified value.
        /// </summary>
        /// <param name="amount">The minimum total amount to filter orders. Must be a positive decimal value.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of 
        /// <see cref="OrderDTO"/> objects representing the orders that meet the specified criteria.</returns>
        /// <exception cref="ApplicationException">Thrown when an error occurs while retrieving the orders.</exception> 
        public async Task<IEnumerable<OrderDTO>> GetOrdersOverTotalAsync(decimal amount)
        {
            try
            {
                var repo = _unitofwork.GetOrderRepository();
                var orders = await repo.GetOrdersOverTotalAsync(amount);
                return mapper.Map<IEnumerable<OrderDTO>>(orders);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving orders over total {amount}.", ex);
            }
        }

        /// <summary>
        /// Retrieves a collection of orders, including associated customer information.
        /// </summary>
        /// <remarks>This method fetches orders along with their related customer details from the data
        /// source. The returned collection is mapped to a DTO format for use in application layers.</remarks>
        /// <returns>A collection of <see cref="OrderDTO"/> objects representing the orders and their associated customer
        /// information.</returns>
        /// <exception cref="ApplicationException">Thrown when an error occurs while retrieving the orders and customer information.</exception> 
        public async Task<IEnumerable<OrderDTO>> GetOrdersWithCustomerAsync()
        {
            try
            {
                var repo = _unitofwork.GetOrderRepository();
                var orders = await repo.GetOrdersWithCustomerAsync();
                return mapper.Map<IEnumerable<OrderDTO>>(orders);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving orders with customer info.", ex);
            }
        }

        public async Task<ServiceResult> BuildOrderDto(OrderDTO orderDto)
        {
            try
            {
                if (orderDto == null)
                    return ServiceResult.Fail("Order data is missing.");

                if (orderDto.OrderItems == null || !orderDto.OrderItems.Any())
                    return ServiceResult.Fail("No order items found.");

                if (string.IsNullOrWhiteSpace(orderDto.AddressInfo?.City))
                    return ServiceResult.Fail("City name is required.");

                // Get discount
                var discount = await _discountRepository.GetActiveDiscountAsync();

                // Calculate total
                foreach (var order in orderDto.OrderItems)
                {
                    if (order.ProductId <= 0)
                        throw new ArgumentException("Invalid Product ID");

                    Product productDb = await _unitofwork.Products.GetByIdAsync(order.ProductId);
                    if (productDb == null)
                        throw new KeyNotFoundException($"Product with ID  not found");

                    if (productDb.PriceAfterDiscount <= 0)
                        throw new InvalidOperationException($"Product has invalid price after discount");

                    if (order.Quantity <= 0)
                        throw new ArgumentException($"Quantity must be greater than 0 for product");

                    order.TotalPrice = productDb.PriceAfterDiscount;
                }

                decimal totalOrderItemsPrice = orderDto.OrderItems.Sum(i => i.TotalPrice * i.Quantity);
                decimal appliedDiscount = 0;

                if (discount != null && totalOrderItemsPrice >= discount.Threshold)
                {
                    appliedDiscount = discount.DicountValue;
                }

                // Get shipping cost
                var governorateShippingCost = await _unitofwork.GovernratesShippingCosts
                    .GetByNameAsync(orderDto.AddressInfo.City);

                if (governorateShippingCost == null)
                    return ServiceResult.Fail("Shipping cost not found for the given city.");

                // Set order properties
                orderDto.CreatedAt = DateTime.UtcNow;
                orderDto.DiscountValue = appliedDiscount;
                orderDto.TotalOrderPrice = totalOrderItemsPrice - appliedDiscount + governorateShippingCost.ShippingCost;
                orderDto.OrderStatus = OrderStatus.Created;
                orderDto.DeliveredAt = orderDto.CreatedAt.AddDays(3);
                orderDto.ShippingCost= governorateShippingCost.ShippingCost;

                return ServiceResult.OkWithData(orderDto);
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Order?> GetOrderByPaymobOrderIdAsync(int paymobOrderId)
        {
            var repo = _unitofwork.GetOrderRepository();
            return await repo.GetOrderByPaymobOrderIdAsync(paymobOrderId);
        }
        private bool CanUpdateShippingStatus(OrderStatus status)
        {
            return status == OrderStatus.Processing
                || status == OrderStatus.AwaitingPayment; 
        }

        public async Task<ServiceResult?> UpdateOrderWithShippingStatus(UpdateShippingStatusDto dto)
        {
            Order order = await _unitofwork.GetOrderRepository().GetByIdAsync(dto.OrderId);

            if (order == null)
                return ServiceResult.Fail("Order not found.");

         
            if (!CanUpdateShippingStatus(order.OrderStatus))
                return ServiceResult.Fail("Cannot update shipping status for the current order state.");

            order.ShippingStatus = dto.NewShippingStatus;


            await _unitofwork.SaveChangesAsync();

            return ServiceResult.OkWithData(order);
        }
        public async Task<PagedResult<OrderSummaryDto>> GetOrderSummariesAsync(OrderFilterDto filter)
        {
            var result = await _unitofwork.GetOrderRepository().GetOrderSummariesAsync(filter);
            return result;  
        }
        public  async Task<ServiceResult> GetListOfPreviousOrderByUserId(string userId)
        {
            try
            {
                List<PreviousOrderDto> previousOrderDtos =
                    await _unitofwork.GetOrderRepository().GetListOfPreviousOrderByUserIdAsync(userId);

                return new ServiceResult
                {
                    Success = true,
                    Data = previousOrderDtos
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }


    }
}
