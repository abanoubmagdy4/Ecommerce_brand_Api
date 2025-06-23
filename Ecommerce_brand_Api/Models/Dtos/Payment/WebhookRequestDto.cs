namespace Ecommerce_brand_Api.Models.Dtos.Payment
{
    public class WebhookRequestDto
    {
        public int order_id { get; set; }
        public string currency { get; set; }
        public bool is_payment_locked { get; set; }
        public bool is_return { get; set; }
        public bool is_cancel { get; set; }
        public bool is_returned { get; set; }
        public bool is_canceled { get; set; }
        public int paid_amount_cents { get; set; }
        public string payment_status { get; set; }
        public string payment_method { get; set; }
        public SourceData source_data { get; set; }
        public PaymentKeyClaims payment_key_claims { get; set; }
        public List<Item> items { get; set; }
        public DateTime created_at { get; set; }

        public class SourceData
        {
            public string type { get; set; }
            public string phone_number { get; set; }
        }

        public class PaymentKeyClaims
        {
            public BillingData billing_data { get; set; }

            public class BillingData
            {
                public string first_name { get; set; }
                public string last_name { get; set; }
                public string email { get; set; }
                public string street { get; set; }
                public string building { get; set; }
                public string floor { get; set; }
                public string apartment { get; set; }
                public string city { get; set; }
            }
        }

        public class Item
        {
            public string name { get; set; }
            public string description { get; set; }
            public int amount_cents { get; set; }
            public int quantity { get; set; }
        }
    }

}
