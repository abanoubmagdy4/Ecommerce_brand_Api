using Ecommerce_brand_Api.Controllers.ResponseWrapper;
using Ecommerce_brand_Api.Models.Dtos.OrdersDTO;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_brand_Api.Controllers
{
    /// <summary>
    /// Provides endpoints for managing orders in the system.
    /// </summary>
    /// <remarks>This controller handles operations related to orders, including retrieving, creating,
    /// updating,  and deleting orders. It uses the <see cref="IOrderService"/> to perform business logic and data 
    /// access operations. All endpoints are secured and require authorization.</remarks>
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService _orderService)
        {
            this._orderService = _orderService;
        }

        /// <summary>
        /// Retrieves all orders from the system.
        /// </summary>
        /// <remarks>This method fetches all available orders and returns them as a collection of <see
        /// cref="OrderDTO"/> objects. If no orders are found, a 404 response is returned with an appropriate message.
        /// In case of a server error, a 500 response is returned.</remarks>
        /// <returns>An <see cref="IActionResult"/> containing one of the following: <list type="bullet"> <item> <description>A
        /// 200 response with an <see cref="ApiResponse{T}"/> containing a collection of <see cref="OrderDTO"/> objects
        /// if orders are found.</description> </item> <item> <description>A 404 response with an <see
        /// cref="ApiResponse{T}"/> containing an error message if no orders are found.</description> </item> <item>
        /// <description>A 500 response with an <see cref="ApiResponse{T}"/> containing an error message if a server
        /// error occurs.</description> </item> </list></returns> 
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderDTO>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();

                if (orders == null || !orders.Any())
                {
                    return NotFound(new ApiResponse<string>()
                    {
                        Data = null,
                        Message = "No orders found",
                        Success = false
                    });
                }

                return Ok(new ApiResponse<IEnumerable<OrderDTO>>()
                {
                    Data = orders,
                    Message = "Orders fetched successfully",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Message = $"Server error: {ex.Message}",
                    Success = false
                });
            }
        }

        /// <summary>
        /// Retrieves the details of an order by its unique identifier.
        /// </summary>
        /// <remarks>This method uses the <c>_orderService</c> to fetch the order details asynchronously.
        /// If the order does not exist, a 404 response is returned. In case of an unexpected server error, a 500
        /// response is returned with the error details.</remarks>
        /// <param name="OrderId">The unique identifier of the order to retrieve. Must be a positive integer.</param>
        /// <returns>An <see cref="IActionResult"/> containing the result of the operation: <list type="bullet"> <item>
        /// <description>A 200 OK response with an <see cref="ApiResponse{T}"/> containing the order details if the
        /// order is found.</description> </item> <item> <description>A 404 Not Found response with an <see
        /// cref="ApiResponse{T}"/> containing an error message if the order is not found.</description> </item> <item>
        /// <description>A 500 Internal Server Error response with an <see cref="ApiResponse{T}"/> containing an error
        /// message if a server error occurs.</description> </item> </list></returns> 
        [HttpGet("{OrderId:int}")]
        [ProducesResponseType(typeof(ApiResponse<OrderDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> GetOrderById(int OrderId)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(OrderId);

                if (order == null)
                {
                    return NotFound(new ApiResponse<string>()
                    {
                        Message = "Order not found",
                        Success = false
                    });
                }

                return Ok(new ApiResponse<OrderDTO>()
                {
                    Data = order,
                    Message = "Order fetched successfully",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Message = $"Server error: {ex.Message}",
                    Success = false
                });
            }
        }


        /// <summary>
        /// Retrieves a list of orders filtered by their status.
        /// </summary>
        /// <remarks>This method returns a 200 OK response with the filtered orders if successful.  If an
        /// error occurs, a 500 Internal Server Error response is returned with an error message.</remarks>
        /// <param name="orderStatus">The status of the orders to retrieve. Must be a valid <see cref="OrderStatus"/> value.</param>
        /// <returns>An <see cref="IActionResult"/> containing an <see cref="ApiResponse{T}"/> with a collection of <see
        /// cref="OrderDTO"/> objects  if the operation is successful, or an error message if a server error occurs.</returns> 
        [HttpGet("status/{orderStatus}")]
        public async Task<IActionResult> GetOrderByStatus(OrderStatus orderStatus)
        {
            try
            {
                var orders = await _orderService.GetOrdersByStatusAsync(orderStatus);

                return Ok(new ApiResponse<IEnumerable<OrderDTO>>()
                {
                    Data = orders,
                    Message = $"Orders with status '{orderStatus}' retrieved successfully",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Message = $"Server error: {ex.Message}",
                    Success = false
                });
            }
        }


        /// <summary>
        /// Retrieves all orders with a total price greater than the specified value.
        /// </summary>
        /// <remarks>This method returns a 200 OK response with the matching orders if successful, or a
        /// 500 Internal Server Error response if an unexpected error occurs.</remarks>
        /// <param name="totalOrderPrice">The minimum total price of orders to retrieve. Must be a non-negative decimal value.</param>
        /// <returns>An <see cref="IActionResult"/> containing an <see cref="ApiResponse{T}"/> object: - On success, the response
        /// contains a collection of <see cref="OrderDTO"/> objects representing the matching orders. - On failure, the
        /// response contains an error message and a failure status.</returns> 
        [HttpGet("totalPrice/{totalOrderPrice:decimal}")]
        public async Task<IActionResult> GetOrdersOverTotal(decimal totalOrderPrice)
        {
            try
            {
                var orders = await _orderService.GetOrdersOverTotalAsync(totalOrderPrice);

                return Ok(new ApiResponse<IEnumerable<OrderDTO>>
                {
                    Data = orders,
                    Message = $"Orders with total over {totalOrderPrice} retrieved successfully",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Message = $"Server error: {ex.Message}",
                    Success = false
                });
            }
        }

        /// <summary>
        /// Retrieves a list of orders along with their associated customer details.
        /// </summary>
        /// <remarks>This method fetches orders and their corresponding customer information from the
        /// system. The response includes a collection of <see cref="OrderDTO"/> objects, each containing order and
        /// customer details. If an error occurs during processing, a 500 Internal Server Error response is returned
        /// with an appropriate error message.</remarks>
        /// <returns>An <see cref="IActionResult"/> containing an <see cref="ApiResponse{T}"/> object.  On success, the response
        /// contains a collection of <see cref="OrderDTO"/> objects and a success message. On failure, the response
        /// contains an error message and a failure status.</returns> 
        [HttpGet("with-customer")]
        public async Task<IActionResult> GetOrdersWithCustomer()
        {
            try
            {
                var orders = await _orderService.GetOrdersWithCustomerAsync();

                return Ok(new ApiResponse<IEnumerable<OrderDTO>>
                {
                    Data = orders,
                    Message = "Orders with customer details retrieved successfully",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Message = $"Server error: {ex.Message}",
                    Success = false
                });
            }
        }

   

        /// <summary>
        /// Updates an existing order with the specified ID using the provided order details.
        /// </summary>
        /// <remarks>This method validates the input data and checks whether the specified order exists
        /// before attempting to update it. If the update fails due to a server-side issue, a 500 status code is
        /// returned with an appropriate error message.</remarks>
        /// <param name="OrderId">The unique identifier of the order to update. Must be a positive integer.</param>
        /// <param name="updatedOrderDTO">An <see cref="OrderDTO"/> object containing the updated order details.  The object must adhere to the
        /// required validation rules.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation: <list type="bullet">
        /// <item><description>A 200 OK response with a success message if the order is updated
        /// successfully.</description></item> <item><description>A 400 Bad Request response if the provided order data
        /// is invalid.</description></item> <item><description>A 404 Not Found response if no order with the specified
        /// ID exists.</description></item> <item><description>A 500 Internal Server Error response if an unexpected
        /// error occurs during the update process.</description></item> </list></returns> 
        [HttpPut("{OrderId:int}")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> UpdateOrder(int OrderId, [FromBody] OrderDTO updatedOrderDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Message = "Invalid order data",
                        Success = false
                    });
                }

                var getOrderID = await _orderService.GetOrderByIdAsync(OrderId);
                if (getOrderID == null)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Message = "Order not found",
                        Success = false
                    });
                }

                var updateOrder = await _orderService.UpdateOrderAsync(OrderId, updatedOrderDTO);

                if (!updateOrder)
                {
                    return StatusCode(500, new ApiResponse<string>
                    {
                        Message = "Failed to update order",
                        Success = false
                    });
                }

                return Ok(new ApiResponse<string>
                {
                    Message = "Order updated successfully",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Message = $"Server error: {ex.Message}",
                    Success = false
                });
            }
        }

        [HttpPut("update-shipping-order-status")]
        public async Task<IActionResult> UpdateShippingAndOrderStatus([FromBody] UpdateShippingStatusDto dto)
        {
            var result = await _orderService.UpdateOrderWithShippingStatus(dto);

            if (!result.Success)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(new
            {
                message = "Shipping cost updated successfully.",
                order = result.Data
            });
        }

        [HttpGet("Shipping-Status")]
        public IActionResult GetOrderStatuses()
        {
            var statuses = Enum.GetValues(typeof(ShippingStatus))
                .Cast<ShippingStatus>()
                .Select(s => new
                {
                    Id = (int)s,
                    Name = s.ToString()
                });

            return Ok(statuses);
        }


        /// <summary>
        /// Deletes an order with the specified ID.
        /// </summary>
        /// <remarks>This method attempts to delete an order by its ID. If the order does not exist, a 404
        /// response is returned. If the deletion fails due to a server error, a 500 response is returned with an
        /// appropriate error message.</remarks>
        /// <param name="OrderId">The unique identifier of the order to delete. Must be a positive integer.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation: <list type="bullet">
        /// <item><description>A 200 OK response with a success message if the order is deleted
        /// successfully.</description></item> <item><description>A 404 Not Found response if no order with the
        /// specified ID exists.</description></item> <item><description>A 500 Internal Server Error response if an
        /// error occurs during the deletion process.</description></item> </list></returns> 
        [HttpDelete("{OrderId:int}")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> DeleteOrder(int OrderId)
        {
            try
            {
                var orderID = await _orderService.GetOrderByIdAsync(OrderId);
                if (orderID == null)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Message = "Order not found",
                        Success = false
                    });
                }

                var deletedOrder = await _orderService.DeleteOrderAsync(OrderId);

                if (!deletedOrder)
                {
                    return StatusCode(500, new ApiResponse<string>
                    {
                        Message = "Failed to delete the order",
                        Success = false
                    });
                }

                return Ok(new ApiResponse<string>
                {
                    Message = "Order deleted successfully",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Message = $"Server error: {ex.Message}",
                    Success = false
                });
            }
        }

        [HttpGet("pagination-summaries")]
        public async Task<IActionResult> GetSummaries([FromQuery] OrderFilterDto filter)
        {
            var result = await _orderService.GetOrderSummariesAsync(filter);
            return Ok(result);
        }
        [HttpGet("previous-orders")]
        public async Task<IActionResult> GetPreviousOrders()
        { 
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid or missing token." });

            var result = await _orderService.GetListOfPreviousOrderByUserId(userId);

            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(result.Data);
        }

    }
}
