using System.Globalization;

namespace RailTicketApp.Dto
{
    public class StandardizedResponse<T>
    {
        public T data;
        public int status;
        public string message;
        public DateTime timestamp;
        public string? errorClass;
        public string? errorMessage;
        public StandardizedResponse(T data, int status, string message)
        {
            this.data = data;
            this.status = status;
            this.message = message;
            this.timestamp = DateTime.Now;
        }
    }

    public static class ResponseFactory
    {
        public static StandardizedResponse<T> Ok<T>(T data, int status, string message)
        {
            return new StandardizedResponse<T>(data, status, message);
        }
    }
}
