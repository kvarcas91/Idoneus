namespace Domain.Models
{
    public struct Response
    {

        public bool Success { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $"Success: {Success}; Message: {Message}";
        }
    }
}
