namespace PowerAPI.Dto
{
    public class ApiException
    {
        public ApiException(int statusCode, string status, string message)
        {
            StatusCode = statusCode;
            Status = status;
            Message = message;
        }

        public int StatusCode { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
