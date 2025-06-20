namespace Ecommerce_brand_Api.Helpers.ErrorHandling
{
    public class ServiceResult
    {
        public bool Success { get; set; } // الحالة: نجحت ولا لأ
        public string? ErrorMessage { get; set; } // الرسالة في حالة الفشل
        public string? SuccessMessage { get; set; }
        // لو العملية نجحت، بنرجّع كائن ServiceResult ناجح
        public static ServiceResult Ok(string successMessage) => new ServiceResult { Success = true, SuccessMessage = successMessage };

        // لو العملية فشلت، بنرجّع كائن ServiceResult فاشل ومعاه رسالة
        public static ServiceResult Fail(string error) => new ServiceResult { Success = false, ErrorMessage = error };
    }
}
