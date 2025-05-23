namespace UserProviderApi.Models
{
    public class ApiResponse<T>
    {
        public required string Status { get; set; }
        public T? Datas { get; set; }
        public required string Message { get; set; }
    }
}