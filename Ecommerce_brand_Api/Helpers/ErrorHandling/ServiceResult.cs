namespace Ecommerce_brand_Api.Helpers.ErrorHandling
{
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public object? Data { get; set; }

        public static ServiceResult Ok(string successMessage) =>
            new ServiceResult { Success = true, SuccessMessage = successMessage };

        public static ServiceResult OkWithData(object data) =>
            new ServiceResult { Success = true, Data = data };

        public static ServiceResult Fail(string error) =>
            new ServiceResult { Success = false, ErrorMessage = error };
    }

}
