using Ecommerce_brand_Api.Helpers.Enums;

namespace Ecommerce_brand_Api.Models.Entities
{
    
        public class Payment
        {
           public int Id { get; set; }
           public long TransactionId { get; set; } //paymob
           public PaymentStatus Status { get; set; }


        // الربط بالأوردر
            public int OrderId { get; set; } //DatabaseID
            public string? PaymobOrderId { get; set; }

            // حالة الدفع
            public string PaymentStatus { get; set; }  // UN
                                                       //
                                                       // , PAID, FAILED, CANCELED, REFUNDED
            public bool Success { get; set; }
            public bool Pending { get; set; }
            public bool IsCaptured { get; set; }
            public bool IsRefunded { get; set; }
            public bool IsCanceled { get; set; }

            // الفلوس
            public int AmountCents { get; set; }            // المطلوب دفعه
            public int PaidAmountCents { get; set; }        // اللي اتدفع فعليًا
            public string Currency { get; set; } = "EGP";
            public int DeliveryFeesCents { get; set; }
            public int DeliveryVatCents { get; set; }
            public int CommissionFees { get; set; }

            // وسيلة الدفع
            public string PaymentMethod { get; set; } = "wallet";
            public string PhoneNumber { get; set; } = string.Empty;
            public string SourceType { get; set; } = "wallet";

            // حالة النظام
            public bool IsVoid { get; set; }
            public bool IsReturn { get; set; }
            public bool IsReturned { get; set; }
            public bool IsPaymentLocked { get; set; }
            public bool NotifyUserWithEmail { get; set; }

            // روابط وبيانات إضافية
            public string? RedirectUrl { get; set; }
            public string? Message { get; set; } // مثلاً: Receiver is not registered
            public string? Notes { get; set; }   // من extra.notes
            public string? ApiSource { get; set; } = "OTHER";
            public string? TerminalId { get; set; }

            // التواريخ
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }


        public List<RefundRequest> RefundRequests { get; set; } = new();
        public List<PaymentItem> Items { get; set; } = new();
             public Order Order { get; set; }


    }

}


