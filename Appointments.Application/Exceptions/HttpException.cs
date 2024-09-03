namespace Appointments.Application.Exceptions
{
    public abstract class HttpException : Exception
    {
        public int StatusCode { get; }

        protected HttpException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}

