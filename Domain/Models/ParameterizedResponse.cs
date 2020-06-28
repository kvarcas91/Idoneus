namespace Domain.Models
{
    public struct ParameterizedResponse<T>
    {
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
