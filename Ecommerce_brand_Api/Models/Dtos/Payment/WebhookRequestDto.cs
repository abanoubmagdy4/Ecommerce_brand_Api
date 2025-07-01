namespace Ecommerce_brand_Api.Models.Dtos.Payment
{
    public class WebhookRequestDto
    {
        public string type { get; set; }
        public TransactionDto obj { get; set; }

        public class TransactionDto
        {
            public long id { get; set; }
            public bool pending { get; set; }
            public int amount_cents { get; set; }
            public bool success { get; set; }
            public bool is_auth { get; set; }
            public bool is_capture { get; set; }
            public bool is_standalone_payment { get; set; }
            public bool is_voided { get; set; }
            public bool is_refunded { get; set; }
            public bool is_3d_secure { get; set; }
            public int integration_id { get; set; }
            public int profile_id { get; set; }
            public bool has_parent_transaction { get; set; }

            public OrderDto Order { get; set; }
            public DateTime created_at { get; set; }
            public List<object> transaction_processed_callback_responses { get; set; }
            public string currency { get; set; }
            public SourceDataDto source_data { get; set; }
            public string api_source { get; set; }
            public object terminal_id { get; set; }
            public int merchant_commission { get; set; }
            public object installment { get; set; }
            public List<object> discount_details { get; set; }
            public bool is_void { get; set; }
            public bool is_refund { get; set; }
            public ExtraDataDto data { get; set; }
            public bool is_hidden { get; set; }
            public PaymentKeyClaimsDto payment_key_claims { get; set; }
            public int accept_fees { get; set; }
            public bool error_occured { get; set; }
            public bool is_live { get; set; }
            public object other_endpoint_reference { get; set; }
            public int refunded_amount_cents { get; set; }
            public int source_id { get; set; }
            public bool is_captured { get; set; }
            public int captured_amount { get; set; }
            public object merchant_staff_tag { get; set; }
            public DateTime updated_at { get; set; }
            public bool is_settled { get; set; }
            public bool bill_balanced { get; set; }
            public bool is_bill { get; set; }
            public int owner { get; set; }
            public object parent_transaction { get; set; }
        }

        public class OrderDto
        {
            public int id { get; set; }
            public DateTime created_at { get; set; }
            public bool delivery_needed { get; set; }
            public MerchantDto merchant { get; set; }
            public object collector { get; set; }
            public int amount_cents { get; set; }
            public ShippingDataDto shipping_data { get; set; }
            public string currency { get; set; }
            public bool is_payment_locked { get; set; }
            public bool is_return { get; set; }
            public bool is_cancel { get; set; }
            public bool is_returned { get; set; }
            public bool is_canceled { get; set; }
            public object merchant_order_id { get; set; }
            public object wallet_notification { get; set; }
            public int paid_amount_cents { get; set; }
            public bool notify_user_with_email { get; set; }
            public List<ItemDto> items { get; set; }
            public string order_url { get; set; }
            public int commission_fees { get; set; }
            public int delivery_fees_cents { get; set; }
            public int delivery_vat_cents { get; set; }
            public string payment_method { get; set; }
            public object merchant_staff_tag { get; set; }
            public string api_source { get; set; }
            public ExtraDataDto data { get; set; }
            public string payment_status { get; set; }
        }

        public class MerchantDto
        {
            public int id { get; set; }
            public DateTime created_at { get; set; }
            public List<string> phones { get; set; }
            public List<string> company_emails { get; set; }
            public string company_name { get; set; }
            public string state { get; set; }
            public string country { get; set; }
            public string city { get; set; }
            public string postal_code { get; set; }
            public string street { get; set; }
        }

        public class ShippingDataDto
        {
            public int id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string street { get; set; }
            public string building { get; set; }
            public string floor { get; set; }
            public string apartment { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string country { get; set; }
            public string email { get; set; }
            public string phone_number { get; set; }
            public string postal_code { get; set; }
            public string extra_description { get; set; }
            public string shipping_method { get; set; }
            public int order_id { get; set; }
            public int order { get; set; }
        }

        public class ItemDto
        {
            public string name { get; set; }
            public string description { get; set; }
            public int amount_cents { get; set; }
            public int quantity { get; set; }
        }

        public class SourceDataDto
        {
            public string type { get; set; }
            public string phone_number { get; set; }
            public string owner_name { get; set; }
            public string sub_type { get; set; }
            public string pan { get; set; }
        }

        public class PaymentKeyClaimsDto
        {
            public int user_id { get; set; }
            public int amount_cents { get; set; }
            public string currency { get; set; }
            public int integration_id { get; set; }
            public int order_id { get; set; }
            public BillingDataDto billing_data { get; set; }
            public bool lock_order_when_paid { get; set; }
            public ExtraDto extra { get; set; }
            public bool single_payment_attempt { get; set; }
            public string next_payment_intention { get; set; }
            public string redirect_url { get; set; }
        }

        public class BillingDataDto
        {
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string street { get; set; }
            public string building { get; set; }
            public string floor { get; set; }
            public string apartment { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string country { get; set; }
            public string email { get; set; }
            public string phone_number { get; set; }
            public string postal_code { get; set; }
            public string extra_description { get; set; }
        }

        public class ExtraDto
        {
            public string notes { get; set; }
            public object merchant_order_id { get; set; }
        }

        public class ExtraDataDto
        {
            public string message { get; set; }
        }
    }

}
