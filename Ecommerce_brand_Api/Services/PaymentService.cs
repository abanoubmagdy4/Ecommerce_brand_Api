using Ecommerce_brand_Api.Helpers;
using Ecommerce_brand_Api.Helpers.Enums;
using Ecommerce_brand_Api.Models.Dtos;
using Ecommerce_brand_Api.Models.Entities;
using Ecommerce_brand_Api.Services.Interfaces;
using System.Net.Http;
using static Ecommerce_brand_Api.Models.Dtos.Payment.WebhookRequestDto;

namespace Ecommerce_brand_Api.Services
{
    public class PaymentService : BaseService<Payment>, IPaymentService
    {
        private readonly IUnitofwork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _IUserService;
        private readonly IOrderService _IOrderService;
        private readonly IServiceUnitOfWork _serviceUnitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly ICartService _cartService;
        private readonly IOrderRefundRepository _OrderRefundRepository;
        private readonly HttpClient _httpClient;

        public PaymentService(IUnitofwork unitOfWork, IMapper mapper, IWebHostEnvironment env, 
            IServiceUnitOfWork serviceUnitOfWork , IHttpClientFactory httpClientFactory) : base(unitOfWork.GetBaseRepository<Payment>())
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _env = env;
            _paymentRepository = _unitOfWork.Payment;
            _userRepository = _unitOfWork.User;
            _serviceUnitOfWork = serviceUnitOfWork;
            _IUserService = _serviceUnitOfWork.Users;
            _IOrderService = _serviceUnitOfWork.Orders;
            _httpClient = httpClientFactory.CreateClient();
            _orderRepository=unitOfWork.GetOrderRepository();
            _cartService= _serviceUnitOfWork.Carts;
            _OrderRefundRepository = _unitOfWork.OrderRefund;

        }

        public async Task<Payment?> GetPaymentByTransactionIdAsync(long transactionId)
        {
            if (transactionId <= 0)
                throw new ArgumentException("Transaction ID must be a positive number.");

            return await _paymentRepository.GetPaymentByTransactionIdAsync(transactionId);
        }
        //CheckOut Logic 

        public async Task<ServiceResult> HandleCheckoutAsync(OrderDTO orderDto)
        {


            ApplicationUser existingUser = await _userRepository.FindByEmailAsync(orderDto.CustomerInfo.Email);
            if (existingUser == null)
                return ServiceResult.Fail("Please Complete Your Information !");


            existingUser = await _IUserService.UpdatedUserAsync(existingUser, orderDto.CustomerInfo);
            Address newAddress = new Address();

            if (orderDto.AddressInfo.Id == 0)
            {
                newAddress = await _IUserService.AddNewAddressAsync(orderDto.AddressInfo, existingUser.Id);
            }
            else
            {
                ServiceResult addressResult = await _IUserService.UpdatedAddressAsync(orderDto.AddressInfo);
                if (addressResult.Success == false)
                    return ServiceResult.Fail("Please Complete Your information !");
                newAddress = (Address)addressResult.Data;
            }

            ServiceResult orderDtoServiceResult = await _IOrderService.BuildOrderDto(orderDto);
            if (!orderDtoServiceResult.Success)
                return ServiceResult.Fail(orderDtoServiceResult.ErrorMessage);
            if (orderDtoServiceResult.Data == null)
                return ServiceResult.Fail("Order preparation failed: no data returned.");
            OrderDTO orderDTOAfterPrepare = (OrderDTO)orderDtoServiceResult.Data;


            Order orderCreated = await _IOrderService.AddNewOrderAsync(orderDTOAfterPrepare, newAddress, existingUser);
            if (orderCreated == null)
                return ServiceResult.Fail("An error occurred while creating the order.");
            var paymentResult = await CreatePaymobPaymentIntentionAsync(orderCreated);
     
            return paymentResult; 
        }

        private async Task<ServiceResult> CreatePaymobPaymentIntentionAsync(Order orderCreated) {

            var url = "https://accept.paymob.com/v1/intention/";
            var itemsList = orderCreated.OrderItems
           .Select(i => new
           {
               name = i.OrderItemId.ToString(),
               amount = (int)(i.TotalPrice * 100),
               description = $"OrderId {i.OrderItemId} For Product Id {i.ProductId} In Database",
               quantity = i.Quantity
           })
           .Append(new
           {
               name = "Discount",
               amount = (int)(-orderCreated.DiscountValue * 100),
               description = "Total discount applied on this order",
               quantity = 1
           })
           .Append(new
           {
               name = "Shipping",
               amount = (int)((orderCreated.ShippingCost ?? 0) * 100),
               description = "Shipping cost for this order",
               quantity = 1
           })
           .ToList();
            var payload = new
            {
                amount = orderCreated.TotalOrderPrice * 100,
                currency = "EGP",
                payment_methods = new[] { 5145466, 5145604, 5145468 },
                items = itemsList,

                billing_data = new
                {
                    apartment = orderCreated.ShippingAddress.Apartment,
                    first_name = orderCreated.FirstName,
                    last_name = orderCreated.LastName,
                    street = orderCreated.ShippingAddress.Street,
                    building = orderCreated.ShippingAddress.Building,
                    phone_number = orderCreated.PhoneNumber,
                    city = orderCreated.ShippingAddress.Street,
                    country = orderCreated.ShippingAddress.Country,
                    email = orderCreated.Customer.Email,
                    floor = orderCreated.ShippingAddress.Floor,
                    state = orderCreated.ShippingAddress.City,
                },
                customer = new
                {
                    first_name = orderCreated.FirstName,
                    last_name = orderCreated.LastName,
                    email = orderCreated.Customer.Email,
                    extras = new { order_source = "website" }
                },
                extras = new { notes = "Test order from backend" }
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Token", "egy_sk_test_65c900f7b4dc44aa0c785734f91bc88d77fe23d44b6396eeb1c98e758a6338a9");

            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                var errorDetails = new
                {
                    Message = "Payment order intention call failed.",
                    Status = response.StatusCode,
                    Details = errorBody
                };
                return ServiceResult.FailWithData(errorDetails);

            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;


            int paymobOrderId = 0;

            if (root.TryGetProperty("client_secret", out JsonElement secretElement))
            {
                var clientSecret = secretElement.GetString();
                if (root.TryGetProperty("intention_order_id", out JsonElement orderIdElement))
                {
                    paymobOrderId = orderIdElement.GetInt32();

                }

                if (paymobOrderId > 0)
                {
                    orderCreated.PaymobOrderId = paymobOrderId;

                    await _orderRepository.UpdateAsync(orderCreated);
                    await _unitOfWork.SaveChangesAsync();

                }
                var publicKey = "egy_pk_test_zLrPn85LuwoQSPS4WBigcxB0wVNIHsiC";
                ///add payment model to db
                var checkoutUrl = $"https://accept.paymob.com/unifiedcheckout/?publicKey={publicKey}&clientSecret={clientSecret}";
                return ServiceResult.OkWithData(new
                {
                    Message = "Payment intention created successfully.",
                    intentionUrl = checkoutUrl
                });

            }
            return ServiceResult.FailWithData(new
            {
                Message = "client_secret not found in response.",
                Response = responseBody
            });

        }



        // Webhook logic: save Payment and update order status 
        public async Task HandleIncomingTransactionAsync(Transaction transaction)
        {


            var payment = CreatePaymentFromTransaction(transaction);
            Order? order = null;
            if (transaction.Order != null)
            {
                order = await _unitOfWork.GetOrderRepository().GetOrderByPaymobOrderIdAsync(transaction.Order.id);
            }

            var existingPayment = await _paymentRepository.GetPaymentByTransactionIdAsync(payment.TransactionId);
            if (existingPayment == null)
            {

                if (order != null)
                {
                    var newStatus = GetOrderStatusFromPayment(payment.Status);
                    if (order.OrderStatus != newStatus)
                    {
                        order.OrderStatus = newStatus;
                        await _unitOfWork.GetOrderRepository().UpdateAsync(order);
                    }
                    payment.OrderId = order.OrderId;

                }
                await _paymentRepository.AddAsync(payment);
            }
            else {
                if (existingPayment.OrderId == 0 && order != null)
                {
                    existingPayment.OrderId = order.OrderId;
                }
                UpdatePaymentFromWebhook(existingPayment, payment);

                if (order != null)
                {
                    var newStatus = GetOrderStatusFromPayment(existingPayment.Status);
                    if (order.OrderStatus != newStatus)
                    {
                        order.OrderStatus = newStatus;
                        await _unitOfWork.GetOrderRepository().UpdateAsync(order);
                    }
                }
                await _paymentRepository.UpdateAsync(existingPayment);
            }
            await _unitOfWork.SaveChangesAsync();

            if (order != null && ShouldClearCart(payment.Status))
            {
                List<int> cartProductIds = order.OrderItems
                .Select(item => item.ProductId)
                .ToList();


                await _cartService.RemovePurchasedProductsFromCartAsync(order.CustomerId, cartProductIds);

            }

        }

        private bool ShouldClearCart(PaymentStatus status) =>
              status == PaymentStatus.Authorized || status == PaymentStatus.Success;


        public Payment CreatePaymentFromTransaction(Transaction transaction)
        {
            return new Payment
            {
                TransactionId = transaction.id,
                PaymobOrderId = transaction.Order?.id.ToString(),
                OrderId = 0, //DB
                Status = GetPaymentStatus(
                             transaction.success,
                             transaction.pending,
                             transaction.is_refunded,
                             transaction.Order?.is_canceled ?? false,
                             transaction.is_captured,
                             transaction.is_refunded
                         ),
                PaymentStatus = transaction.Order?.payment_status ?? "UNPAID",
                Success = transaction.success,
                Pending = transaction.pending,
                IsCaptured = transaction.is_captured,
                IsRefunded = transaction.is_refunded,
                IsCanceled = transaction.Order?.is_canceled ?? false,

                AmountCents = transaction.amount_cents,
                PaidAmountCents = transaction.Order?.paid_amount_cents ?? 0,
                Currency = transaction.currency ?? "EGP",
                DeliveryFeesCents = transaction.Order?.delivery_fees_cents ?? 0,
                DeliveryVatCents = transaction.Order?.delivery_vat_cents ?? 0,
                CommissionFees = transaction.Order?.commission_fees ?? 0,

                PaymentMethod = transaction.Order?.payment_method ?? "unknown",
                PhoneNumber = transaction.source_data?.phone_number ?? "",
                SourceType = transaction.source_data?.type ?? "unknown",

                IsVoid = transaction.is_void,
                IsReturn = transaction.Order?.is_return ?? false,
                IsReturned = transaction.Order?.is_returned ?? false,
                IsPaymentLocked = transaction.Order?.is_payment_locked ?? false,
                NotifyUserWithEmail = transaction.Order?.notify_user_with_email ?? false,

                RedirectUrl = transaction.payment_key_claims?.redirect_url,
                Message = transaction.data?.message,
                Notes = transaction.payment_key_claims?.extra?.notes,
                ApiSource = transaction.api_source,
                TerminalId = transaction.terminal_id?.ToString(),

                CreatedAt = transaction.created_at,
                UpdatedAt = transaction.updated_at,

                Items = transaction.Order?.items?.Select(item => new PaymentItem
                {
                    Name = item.name,
                    Description = item.description,
                    AmountCents = item.amount_cents,
                    Quantity = item.quantity
                }).ToList() ?? new List<PaymentItem>()

            };

            }
        public void UpdatePaymentFromWebhook(Payment existing, Payment updated)
        {
            existing.Status = GetPaymentStatus(
                            updated.Success,
                           updated.Pending,
                            updated.IsCaptured,
                            updated.IsCanceled,
                           updated.IsCaptured,
                            updated.IsRefunded
                        );
            existing.PaymentStatus = updated.PaymentStatus;

            existing.Success = updated.Success;
            existing.Pending = updated.Pending;
            existing.IsCaptured = updated.IsCaptured;
            existing.IsRefunded = updated.IsRefunded;
            existing.IsCanceled = updated.IsCanceled;

            existing.PaidAmountCents = updated.PaidAmountCents;
            existing.CommissionFees = updated.CommissionFees;
            existing.DeliveryFeesCents = updated.DeliveryFeesCents;
            existing.DeliveryVatCents = updated.DeliveryVatCents;

            existing.IsVoid = updated.IsVoid;
            existing.IsReturn = updated.IsReturn;
            existing.IsReturned = updated.IsReturned;
            existing.IsPaymentLocked = updated.IsPaymentLocked;
            existing.NotifyUserWithEmail = updated.NotifyUserWithEmail;

            existing.Message = updated.Message;
            existing.Notes = updated.Notes;
            existing.ApiSource = updated.ApiSource;
            existing.TerminalId = updated.TerminalId;

            existing.UpdatedAt = DateTime.UtcNow;

            // مبدأيًا مش بنعدل CreatedAt ولا OrderId ولا Items
        }


        private PaymentStatus GetPaymentStatus(
                bool success,
                bool pending,
                bool isRefunded,
                bool isCanceled,
                bool isCaptured,
                bool isVoided 
 )
        {
            if (isRefunded)
                return PaymentStatus.Refunded;

            if (isVoided)
                return PaymentStatus.Canceled; 

            if (isCanceled)
                return PaymentStatus.Canceled;

            if (success && !pending && isCaptured)
                return PaymentStatus.Success;

            if (success && !pending && !isCaptured)
                return PaymentStatus.Authorized;

            if (!success && pending)
                return PaymentStatus.Pending;

            if (!success && !pending)
                return PaymentStatus.Declined;

            throw new Exception("Invalid status state");
        }

        public OrderStatus GetOrderStatusFromPayment(PaymentStatus paymentStatus)
        {
            return paymentStatus switch
            {
                PaymentStatus.Refunded => OrderStatus.Returned,
                PaymentStatus.Canceled => OrderStatus.Cancelled,
                PaymentStatus.Authorized => OrderStatus.Processing,
                PaymentStatus.Success => OrderStatus.Processing,
                PaymentStatus.Pending => OrderStatus.AwaitingPayment,
                PaymentStatus.Declined => OrderStatus.Cancelled,
                _ => throw new InvalidOperationException("Unsupported payment status")
            };
        }


        ///Refund Handling  <summary>
        /// Refund Handling 

        public async Task<ServiceResult> HandleRequestRefundAsync(OrderRefundDto dto)
        {

            // Get the current user's ID from the token
            var userIdResult = _IUserService.GetCurrentUserId();
            if (userIdResult == null) {

                return ServiceResult.Fail("Failed to retrieve user ID from the current context.");
            }
            // Load the order with the associated payment data
            var order = await _orderRepository.GetOrderWithPaymentAsync(dto.OrderId);
            if (order == null || order.CustomerId != userIdResult)
                return ServiceResult.Fail("Order not found or access denied.");
            // Check if refund or cancel is allowed based on current order, payment, and shipping statuses

            bool canCancel = OrderActionsValidator.CanCancel(order.OrderStatus, order.Payment.Status, order.ShippingStatus);
            bool canRefund = OrderActionsValidator.CanRefund(order.OrderStatus, order.Payment.Status, order.ShippingStatus);

            if (!canCancel && !canRefund)
            {
                return ServiceResult.Fail("Refund or cancellation is not allowed for this order status.");
            }

            var OrderRefund = new OrderRefund
            {
                PaymentId = order.Payment.Id,
                AmountCents = order.Payment.PaidAmountCents,
                Reason = dto.Reason,
                RequestedByUserId = userIdResult,
            };

            await _OrderRefundRepository.AddAsync(OrderRefund);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result > 0)
            {
                return ServiceResult.Ok("Refund request submitted successfully and is awaiting review.");
            }
            else
            {
                return ServiceResult.Fail("Failed to submit refund request. Please try again.");
            }
        }


        public async Task<ServiceResult> HandleApproveRefund(ApproveRefundDto dto) {

            
            var OrderRefund = await _OrderRefundRepository.GetByIdWithOrderAndPaymentAsync(dto.OrderRefundId);
            if (OrderRefund == null)
                return ServiceResult.Fail("Refund request not found.");

            if (OrderRefund.Status != RefundStatus.Pending)
                return ServiceResult.Fail("Refund request has already been processed.");

            if (dto.Approve)
            {
                OrderRefund.Status = RefundStatus.Approved;
                OrderRefund.ApprovedAt = DateTime.UtcNow;
                OrderRefund.ApprovedByAdminId = _IUserService.GetCurrentUserId();
                await _OrderRefundRepository.UpdateAsync(OrderRefund);
                await _unitOfWork.SaveChangesAsync();

            }
            else
            {
                OrderRefund.Status = RefundStatus.Rejected;
                await _OrderRefundRepository.UpdateAsync(OrderRefund);
                await _unitOfWork.SaveChangesAsync();

                return new ServiceResult
                {
                    Success = true,
                    SuccessMessage = "Refund request Rejected.",
                    Data = OrderRefund
                };
            }


            var payment = OrderRefund.Payment;
            if (payment == null)
                return ServiceResult.Fail("Associated payment not found.");

            var orderStatus = OrderRefund.Payment.Order.OrderStatus;
            var paymentStatus = payment.Status;
            var shippingStatus = OrderRefund.Payment.Order.ShippingStatus;

            bool canCancel = OrderActionsValidator.CanCancel(orderStatus, paymentStatus, shippingStatus);
            bool canRefund = OrderActionsValidator.CanRefund(orderStatus, paymentStatus, shippingStatus);

            if (!canCancel && !canRefund)
            {
                return ServiceResult.Fail("Refund or cancellation is not allowed for this order status.");
            }

            string secretKey = "egy_sk_test_65c900f7b4dc44aa0c785734f91bc88d77fe23d44b6396eeb1c98e758a6338a9";

            bool doCancel = canCancel; 

            var resultMessage = await ProcessTransactionCancellationOrRefund(payment.TransactionId, OrderRefund.AmountCents, doCancel, secretKey);

            if (resultMessage.StartsWith("Void failed") || resultMessage.StartsWith("Refund failed"))
            {
                return ServiceResult.Fail(resultMessage);
            }

            if (doCancel)
            {
                payment.Status = PaymentStatus.Canceled;  
                OrderRefund.Status = RefundStatus.Completed;
                OrderRefund.Payment.Order.OrderStatus = OrderStatus.Cancelled;
            }
            else
            {
                payment.Status = PaymentStatus.Refunded;
                OrderRefund.Status = RefundStatus.Completed;
                OrderRefund.Payment.Order.OrderStatus = OrderStatus.Returned;
            }

            await _paymentRepository.UpdateAsync(payment);
            await _orderRepository.UpdateAsync(OrderRefund.Payment.Order);
            await _OrderRefundRepository.UpdateAsync(OrderRefund);
            await _unitOfWork.SaveChangesAsync();


            return new ServiceResult
            {
                Success = true,
                SuccessMessage = "Refund request Compelete.",
                Data = OrderRefund
            };
        }



        public async Task<string> ProcessTransactionCancellationOrRefund(long transactionId, int amountCents, bool canVoid, string secretKey)
        {
            using var client = new HttpClient();

            if (canVoid)
            {
                // عملية الإلغاء (Void)
                var voidRequest = new HttpRequestMessage(HttpMethod.Post, "https://accept.paymob.com/api/acceptance/void_refund/void");
                voidRequest.Headers.Add("Authorization", $"Token {secretKey}");

                var voidPayload = JsonSerializer.Serialize(new { transaction_id = transactionId });
                voidRequest.Content = new StringContent(voidPayload, Encoding.UTF8, "application/json");

                var voidResponse = await client.SendAsync(voidRequest);
                if (!voidResponse.IsSuccessStatusCode)
                {
                    var error = await voidResponse.Content.ReadAsStringAsync();
                    return $"Void failed: {error}";
                }

                return $"Transaction {transactionId} voided successfully.";
            }
            else
            {
                // عملية الاسترجاع (Refund)
                var OrderRefund = new HttpRequestMessage(HttpMethod.Post, "https://accept.paymob.com/api/acceptance/void_refund/refund");
                OrderRefund.Headers.Add("Authorization", $"Token {secretKey}");

                var refundPayload = JsonSerializer.Serialize(new
                {
                    transaction_id = transactionId,
                    amount_cents = amountCents.ToString()
                });
                OrderRefund.Content = new StringContent(refundPayload, Encoding.UTF8, "application/json");

                var refundResponse = await client.SendAsync(OrderRefund);
                if (!refundResponse.IsSuccessStatusCode)
                {
                    var error = await refundResponse.Content.ReadAsStringAsync();
                    return $"Refund failed: {error}";
                }

                return $"Transaction {transactionId} refunded successfully.";
            }
        }




    }

}

