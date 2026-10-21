namespace Bilionn_backend.Exceptions
{
    public class AppException: Exception
    {
        public int StatusCode { get; set; }

        public AppException(int statusCode, string mssg) : base(mssg)
        {
            StatusCode = statusCode;
        }

        public  static AppException NotFound(string mssg) => new(404, mssg);
        
        public static AppException Conflict(string mssg) => new(409, mssg);
        
        public static AppException Unauthorized(string message) => new(401, message);
        
        public static AppException Forbidden(string message) => new(403, message);
    }
}
