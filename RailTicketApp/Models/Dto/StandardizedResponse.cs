using System.Globalization;

namespace RailTicketApp.Models.Dto
{
    public class StandardizedResponse<T>
    {
        public T? data { get; set; }
        public int status { get; set; }
        public string message { get; set; }
        public DateTime timestamp { get; set; }
        public string? errorClass { get; set; }
        public string? errorMessage { get; set; }
        public StandardizedResponse(T data, int status, string message)
        {
            this.data = data;
            this.status = status;
            this.message = message;
            timestamp = DateTime.Now;
        }
        public StandardizedResponse(T data, int status, string errorClass, string errorMessage)
        {
            if(!(data is string) || !(data as string).Equals(""))
            {
                this.data = data;
            }
            this.status = status;
            this.errorClass = errorClass;
            this.errorMessage = errorMessage;
        }
    }

    public static class ResponseFactory
    {
        public static StandardizedResponse<T> Ok<T>(T data, int status, string message)
        {
            return new StandardizedResponse<T>(data, status, message);
        }

        public static StandardizedResponse<T> Error<T>(T data, int status, string errorClass, string errorMessage)
        {
            return new StandardizedResponse<T>(data, status, errorClass, errorMessage);
        }
    }
}
