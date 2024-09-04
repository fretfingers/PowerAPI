namespace PowerAPI.Dto
{
    public class ApiResBody
    {
        public ApiResBody(int statusCode, string status, string message, object data)
        {
            StatusCode = statusCode;
            Status = status;
            Message = message;
            Data = data;
            
        }

        public int StatusCode { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
